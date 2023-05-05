using eSMP.Models;
using eSMP.Services.AutoService;
using eSMP.Services.NotificationRepo;
using eSMP.Services.OrderRepo;
using eSMP.Services.ShipRepo;
using eSMP.Services.StoreRepo;
using eSMP.VModels;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Notification = eSMP.VModels.Notification;

namespace eSMP.Services.MomoRepo
{
    public class MomoRepository : IMomoReposity
    {
        private readonly WebContext _context;
        private readonly Lazy<IShipReposity> _shipReposity;
        private readonly Lazy<IOrderReposity> _orderReposity;
        private readonly Lazy<INotificationReposity> _notification;
        private static readonly object looker = new object();
        string accessKey = "klm05TvNBzhg7h7j";
        string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        string partnerCode = "MOMOBKUN20180529";
        public static string TOKEN = "D1f7d11dFA5D417D35F136A02e0C812EEC613Fcb";

        public MomoRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IOrderReposity> orderReposity, Lazy<INotificationReposity> notification)
        {
            _context = context;
            _shipReposity = shipReposity;
            _orderReposity = orderReposity;
            _notification = notification;
        }
        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }

        public async Task<MomoPayReponse> GetPayAsync(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var firebaseReponse = Getapplink(orderID);

            MomoPayRequest request = new MomoPayRequest();
            //https://esmp.page.link/view
            request.orderInfo = "Thanh toan cho don hang cua eSMP";
            request.partnerCode = partnerCode;
            request.redirectUrl = firebaseReponse.Result.shortLink;
            request.ipnUrl = "https://esmpfree-001-site1.etempurl.com/api/Payment";
            request.amount = Gettotalprice(orderID);
            request.orderId = orderID + "-" + myuuidAsString;
            request.requestId = myuuidAsString;
            request.extraData = "";
            request.lang = "vi";
            request.requestType = "captureWallet";
            request.autoCapture = true;


            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&extraData=" + request.extraData + "&ipnUrl=" + request.ipnUrl + "&orderId=" + request.orderId + "&orderInfo=" + request.orderInfo + "&partnerCode=" + request.partnerCode + "&redirectUrl=" + request.redirectUrl + "&requestId=" + request.requestId + "&requestType=" + request.requestType;
            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<MomoPayReponse>();
            return contents.Result;
        }
        private static string getSignature(string text, string key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        public long Gettotalprice(int orderID)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    var feeShip = 0;
                    var priceitem = _orderReposity.Value.GetPriceItemOrder(orderID);
                    if (order.OrderStatusID != 2)
                    {
                        feeShip = (int)order.FeeShip;
                    }
                    else
                    {
                        feeShip = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, _orderReposity.Value.GetWeightOrder(orderID), priceitem).fee.fee;
                    }

                    if (feeShip != 0 && priceitem != 0)
                    {
                        return (long)(feeShip + priceitem);
                    }
                    return 0;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
        public Sub_Item GetSub_Item(int sub_ItemID)
        {
            try
            {
                return _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == sub_ItemID);
            }
            catch
            {
                return null;
            }
        }
        public bool CheckShip(int orderID)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    var priceitem = _orderReposity.Value.GetPriceItemOrder(orderID);
                    var ship = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, _orderReposity.Value.GetWeightOrder(order.OrderID), priceitem);
                    if (ship != null)
                    {
                        if (ship.success && ship.fee.delivery)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public Result GetPayUrl(int orderID, string paymentMethod)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (_orderReposity.Value.GetPriceItemOrder(order.OrderID) >= 20000000)
                {
                    result.Success = false;
                    result.Message = "Tổng đơn hàng không được quá 20.000.000 VND";
                    result.Data = "";
                    result.TotalPage = 1;
                    return result;
                }
                order.PaymentMethod = paymentMethod;
                _context.SaveChanges();
                if (order != null)
                {
                    if (order.OrderStatusID == 1)
                    {
                        result.Success = false;
                        result.Message = "Đơn hàng đã thành toán";
                        result.Data = "";
                        return result;
                    }
                    var listdetail = _context.OrderDetails.Where(od => od.OrderID == orderID).ToList();
                    var checkout = 0;
                    var itemerro = "";
                    if (listdetail.Count > 0)
                    {
                        foreach (var item in listdetail)
                        {
                            var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == item.Sub_ItemID && si.Amount >= item.Amount && si.SubItem_StatusID == 1 && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID && _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).Store_StatusID == 1).Item_StatusID == 1);
                            if (subItem != null)
                            {
                                item.ReturnAndExchange = subItem.ReturnAndExchange;
                                _context.SaveChanges();
                                checkout++;
                            }
                            else
                            {
                                if (itemerro == "")
                                {
                                    itemerro = GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                                }
                                else
                                    itemerro = itemerro + ", " + GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                            }
                        }
                        var totalweight = _orderReposity.Value.GetWeightOrder(order.OrderID);
                        if (totalweight >= 20000)
                        {
                            result.Success = false;
                            result.Message = "khối lượng của đơn hàng phải dưới 20 kg";
                            result.Data = "";
                            result.TotalPage = 1;
                            return result;
                        }
                        if (checkout == listdetail.Count)
                        {
                            if (CheckShip(orderID))
                            {
                                if (paymentMethod != "COD")
                                {
                                    MomoPayReponse momoPayReponse = GetPayAsync(orderID).Result;
                                    if (momoPayReponse != null)
                                    {
                                        if (momoPayReponse.resultCode == 0)
                                        {
                                            result.Success = true;
                                            result.Message = "Thành công";
                                            result.Data = momoPayReponse;
                                            return result;
                                        }
                                        else
                                        {
                                            result.Success = false;
                                            result.Message = "Hệ thống thanh toán lỗi";
                                            result.Data = momoPayReponse;
                                            return result;
                                        }
                                    }
                                    else
                                    {
                                        result.Success = false;
                                        result.Message = "Hệ thống thanh toán đang bảo trì";
                                        result.Data = "";
                                        return result;
                                    }
                                }
                                else
                                {
                                    lock (looker)
                                    {
                                        checkout = 0;
                                        string itemerrolook = "";
                                        foreach (var item in listdetail)
                                        {
                                            var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == item.Sub_ItemID && si.Amount >= item.Amount && si.SubItem_StatusID == 1 && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID && _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).Store_StatusID == 1).Item_StatusID == 1);
                                            if (subItem != null)
                                            {
                                                item.ReturnAndExchange = subItem.ReturnAndExchange;
                                                _context.SaveChanges();
                                                checkout++;
                                            }
                                            else
                                            {
                                                if (itemerrolook == "")
                                                {
                                                    itemerrolook = GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                                                }
                                                else
                                                    itemerrolook = itemerrolook + ", " + GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                                            }
                                        }
                                        if (checkout != listdetail.Count)
                                        {
                                            result.Success = false;
                                            result.Message = "Số lượng của sản phầm " + itemerrolook + " không đủ";
                                            result.Data = "";
                                            result.TotalPage = 1;
                                            return result;
                                        }
                                        var shipReponse = _shipReposity.Value.CreateOrder(order.OrderID);
                                        if (shipReponse != null)
                                        {
                                            if (shipReponse.success)
                                            {
                                                ShipOrder shipOrder = new ShipOrder();
                                                shipOrder.Status_ID = "-2";
                                                DateTime datetime = GetVnTime();
                                                shipOrder.Create_Date = datetime;
                                                shipOrder.LabelID = shipReponse.order.label;
                                                shipOrder.Reason = "";
                                                shipOrder.OrderID = int.Parse(shipReponse.order.partner_id);
                                                shipOrder.Reason_code = "";
                                                order.Pick_Time = shipReponse.order.estimated_pick_time;
                                                order.Deliver_time = shipReponse.order.estimated_deliver_time;
                                                _context.ShipOrders.Add(shipOrder);
                                                _context.SaveChanges();

                                                order.OrderStatusID = 4;
                                                order.FeeShip = shipReponse.order.fee;
                                                order.Create_Date = GetVnTime();
                                                //tạo status
                                                _context.SaveChanges();
                                                //updateamount
                                                var listdetailnew = _context.OrderDetails.Where(od => od.OrderID == order.OrderID).ToList();
                                                foreach (var detail in listdetailnew)
                                                {
                                                    var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == detail.Sub_ItemID);
                                                    detail.WarrantiesTime = subItem.WarrantiesTime;
                                                    subItem.Amount = subItem.Amount - detail.Amount;
                                                    _context.SaveChanges();
                                                }
                                                //thong bao
                                                _notification.Value.CreateNotifiaction(order.UserID, "Đặt hàng thành công", null, order.OrderID, null);
                                                Notification notification = new Notification
                                                {
                                                    title = "Đặt hàng",
                                                    body = "Đặt hàng " + order.OrderID + " thành công",
                                                };
                                                FirebaseNotification firebaseNotification = new FirebaseNotification
                                                {
                                                    notification = notification,
                                                    to = order.User.FCM_Firebase,
                                                };
                                                _notification.Value.PushUserNotificationAsync(firebaseNotification);
                                                // supplier
                                                var store = GetStoreByorderID(order.OrderID);
                                                _notification.Value.CreateNotifiaction(store.UserID, "Đơn hàng mới: ", null, order.OrderID, null);
                                                Notification notificationOfSup = new Notification
                                                {
                                                    title = "Đơn hàng",
                                                    body = "Đơn hàng mới",
                                                };
                                                FirebaseNotification firebaseNotificationSupp = new FirebaseNotification
                                                {
                                                    notification = notificationOfSup,
                                                    to = store.User.FCM_Firebase,
                                                };
                                                _notification.Value.PushUserNotificationAsync(firebaseNotificationSupp);
                                                result.Success = true;
                                                result.Message = "Đặt hàng thành công";
                                                result.Data = "";
                                                result.TotalPage = 1;
                                                return result;
                                            }
                                            else
                                            {

                                                order.OrderStatusID = 2;
                                                _context.SaveChanges();
                                                result.Success = false;
                                                result.Message = "Hệ thống giao hàng bảo trì, thử lại sau";
                                                result.Data = "";
                                                result.TotalPage = 1;
                                                return result;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                result.Success = false;
                                result.Message = "Địa điểm giao hàng chưa được hỗ trợ";
                                result.Data = "";
                                result.TotalPage = 1;
                                return result;
                            }
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Số lượng của sản phầm " + itemerro + " không đủ";
                            result.Data = "";
                            result.TotalPage = 1;
                            return result;
                        }
                    }
                }
                result.Success = false;
                result.Message = "Đơn hàng không tồn tại";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = 1;
                return result;
            }
        }
        public async Task PayOrderINPAsync(MomoPayINP payINP)
        {
            lock (looker)
            {
                try
                {
                    if (payINP.resultCode == 0)
                    {
                        var orderid = payINP.orderId.Split('-')[0];
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == int.Parse(orderid) && o.OrderStatusID==2);
                        if (order != null)
                        {
                            var listdetail = _context.OrderDetails.Where(od => od.OrderID == order.OrderID && order.OrderStatusID == 2).ToList();
                            var checkout = 0;
                            var itemerro = "";
                            if (listdetail.Count > 0)
                            {
                                foreach (var item in listdetail)
                                {
                                    var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == item.Sub_ItemID && si.Amount >= item.Amount && si.SubItem_StatusID == 1 && _context.Items.SingleOrDefault(i => i.ItemID == si.ItemID && _context.Stores.SingleOrDefault(s => s.StoreID == i.StoreID).Store_StatusID == 1).Item_StatusID == 1);
                                    if (subItem != null)
                                    {
                                        item.ReturnAndExchange = subItem.ReturnAndExchange;
                                        checkout++;
                                    }
                                    else
                                    {
                                        if (itemerro == "")
                                        {
                                            itemerro = GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                                        }
                                        else
                                            itemerro = itemerro + ", " + GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                                    }
                                }

                                _context.SaveChanges();
                            }

                            if (checkout == listdetail.Count)
                            {
                                //create ship
                                var shipReponse = _shipReposity.Value.CreateOrder(order.OrderID);
                                /*ShipReponse  shipReponse=null;
                                try
                                {
                                    var listoderdetai = _orderReposity.Value.GetOrderDetailModels(order.OrderID, 2);
                                    var listproduct = new List<productsShip>();
                                    foreach (var item in listoderdetai)
                                    {
                                        productsShip pro = new productsShip
                                        {
                                            name = item.Sub_ItemName,
                                            price = (int)item.PricePurchase,
                                            product_code = item.Sub_ItemID,
                                            quantity = item.Amount,
                                            weight = GetWeightOfSubItem(item.ItemID) / (double)1000,
                                        };
                                        listproduct.Add(pro);
                                    }
                                    var priceOrder = _orderReposity.Value.GetPriceItemOrder(order.OrderID);
                                    orderrequest shiporder = new orderrequest
                                    {
                                        id = order.OrderID + "",
                                        pick_name = order.Pick_Name,
                                        pick_address = order.Pick_Address,
                                        pick_ward = order.Pick_Ward,
                                        pick_district = order.Pick_District,
                                        pick_province = order.Pick_Province,
                                        pick_street = "",
                                        pick_tel = order.Pick_Tel,
                                        pick_money = 0,
                                        is_freeship = 1,
                                        name = order.Name,
                                        address = order.Address,
                                        district = order.District,
                                        province = order.Province,
                                        hamlet = order.Address,
                                        street = "",
                                        ward = order.Ward,
                                        tel = order.Tel,
                                        value = (int)priceOrder,
                                        transport = "road",
                                        pick_option = "cod",
                                    };
                                    if (order.PaymentMethod == "COD")
                                    {
                                        shiporder.is_freeship = 0;
                                        shiporder.pick_money = (int)GetPriceItemOrder(order.OrderID);
                                    }
                                    var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                                    if (system != null)
                                    {
                                        if (system.Co_Examination)
                                        {
                                            shiporder.tags = new int[1] { 11 };
                                        }
                                    }
                                    if (order != null)
                                    {
                                        ShipOrderRequest request = new ShipOrderRequest
                                        {
                                            order = shiporder,
                                            products = listproduct,
                                        };

                                        var client = new HttpClient();
                                        client.DefaultRequestHeaders.Add("Token", TOKEN);
                                        StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
                                        try
                                        {
                                            using (var httpClient = new HttpClient())
                                            {
                                                using (var response =  client.PostAsync("https://services-staging.ghtklab.com/services/shipment/order/?ver=1.5", httpContent).Result)
                                                {
                                                    var contents = response.Content.ReadFromJsonAsync<ShipReponse>();
                                                    shipReponse = new ShipReponse();
                                                    shipReponse = contents.Result;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                }*/


                                if (shipReponse != null)
                                {
                                    if (shipReponse.success)
                                    {
                                        ShipOrder shipOrder = new ShipOrder();
                                        shipOrder.Status_ID = "-2";
                                        DateTime datetime = GetVnTime();
                                        shipOrder.Create_Date = datetime;
                                        shipOrder.LabelID = shipReponse.order.label;
                                        shipOrder.Reason = "";
                                        shipOrder.OrderID = int.Parse(shipReponse.order.partner_id);
                                        shipOrder.Reason_code = "";
                                        order.Pick_Time = shipReponse.order.estimated_pick_time;
                                        order.Deliver_time = shipReponse.order.estimated_deliver_time;
                                        _context.ShipOrders.Add(shipOrder);
                                        _context.SaveChanges();


                                        OrderBuy_Transacsion transacsion = new OrderBuy_Transacsion();
                                        transacsion.OrderID = order.OrderID;
                                        DateTime dateTime = new DateTime();
                                        dateTime = dateTime.AddYears(1969);
                                        dateTime = dateTime.AddMilliseconds(payINP.responseTime).ToUniversalTime();

                                        string vnTimeZoneKey = "SE Asia Standard Time";
                                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                                        DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, vnTimeZone);
                                        transacsion.Create_Date = VnTime;
                                        transacsion.ResultCode = payINP.resultCode;
                                        transacsion.TransactionID = payINP.transId;
                                        transacsion.OrderIDGateway = payINP.orderId;
                                        transacsion.RequestID = payINP.requestId;
                                        _context.orderBuy_Transacsions.Add(transacsion);
                                        order.OrderStatusID = 1;
                                        order.FeeShip = shipReponse.order.fee;
                                        order.Create_Date = GetVnTime();
                                        _context.SaveChanges();
                                        //updateamount
                                        //var listdetail = _context.OrderDetails.Where(od => od.OrderID == order.OrderID).ToList();
                                        foreach (var detail in listdetail)
                                        {
                                            var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == detail.Sub_ItemID);
                                            subItem.Amount = subItem.Amount - detail.Amount;
                                            _context.SaveChanges();
                                        }
                                        //thong bao
                                        _notification.Value.CreateNotifiaction(order.UserID, "Đặt hàng thành công", null, order.OrderID, null);
                                        Notification notification = new Notification
                                        {
                                            title = "Thanh toán",
                                            body = "Đơn hàng " + order.OrderID + " thành công",
                                        };
                                        FirebaseNotification firebaseNotification = new FirebaseNotification
                                        {
                                            notification = notification,
                                            to = order.User.FCM_Firebase,
                                        };
                                        _notification.Value.PushUserNotificationAsync(firebaseNotification);
                                        // supplier
                                        var store = GetStoreByorderID(order.OrderID);
                                        _notification.Value.CreateNotifiaction(store.UserID, "Đơn hàng mới: ", null, order.OrderID, null);
                                        Notification notificationOfSup = new Notification
                                        {
                                            title = "Đơn hàng",
                                            body = "Đơn hàng mới",
                                        };
                                        FirebaseNotification firebaseNotificationSupp = new FirebaseNotification
                                        {
                                            notification = notificationOfSup,
                                            to = store.User.FCM_Firebase,
                                        };
                                        _notification.Value.PushUserNotificationAsync(firebaseNotificationSupp);
                                        return;
                                    }
                                }
                                refundnow(order.OrderID, payINP.amount, payINP.transId, order.UserID, order.User.FirebaseID);
                                order.OrderStatusID = 3;
                                ShipOrder model = new ShipOrder();
                                model.OrderID = order.OrderID;
                                model.Create_Date = GetVnTime();
                                model.LabelID = order.OrderID+"";
                                model.Reason = "";
                                model.Reason_code = "";
                                model.Status_ID = "-3";
                                _context.ShipOrders.Add(model);
                                _context.SaveChanges();
                                _notification.Value.CreateNotifiaction(order.UserID, "Đặt hàng thất bại", null, null, null);
                                Notification notifications = new Notification
                                {
                                    title = "Đơn hàng",
                                    body = "Đặt hàng thất bại do dịch vụ vận chuyển có sự số",
                                };
                                FirebaseNotification firebaseNotifications = new FirebaseNotification
                                {
                                    notification = notifications,
                                    to = order.User.FCM_Firebase,
                                };
                                _notification.Value.PushUserNotificationAsync(firebaseNotifications);
                                return;
                            }
                            else
                            {
                                _notification.Value.CreateNotifiaction(order.UserID, "Đặt hàng thất bại", null, null, null);
                                Notification notification = new Notification
                                {
                                    title = "Đơn hàng",
                                    body = itemerro + " Không đủ hàng",
                                };
                                FirebaseNotification firebaseNotification = new FirebaseNotification
                                {
                                    notification = notification,
                                    to = order.User.FCM_Firebase,
                                };
                                _notification.Value.PushUserNotificationAsync(firebaseNotification);
                                order.OrderStatusID = 3;
                                ShipOrder model = new ShipOrder();
                                model.OrderID = order.OrderID;
                                model.Create_Date = GetVnTime();
                                model.LabelID = order.OrderID + "";
                                model.Reason = "";
                                model.Reason_code = "";
                                model.Status_ID = "-3";
                                _context.ShipOrders.Add(model);
                                _context.SaveChanges();
                                refundnow(order.OrderID, payINP.amount, payINP.transId, order.UserID, order.User.FirebaseID);
                                return;
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
                    role.RoleName = ex.Message;
                    _context.SaveChanges();
                    return;
                }
            }
                
            
        }
        public double GetPriceItemOrder(int orderID)
        {
            try
            {
                double total = 0;
                var listorderdetail = _context.OrderDetails.Where(od => od.OrderID == orderID);

                if (_context.Orders.SingleOrDefault(o => o.OrderID == orderID).OrderStatusID == 1)
                {
                    if (listorderdetail.Count() > 0)
                    {
                        foreach (var item in listorderdetail.ToList())
                        {
                            total = total + item.PricePurchase * item.Amount * (1 - item.DiscountPurchase);
                        }
                    }
                }
                else
                {
                    if (listorderdetail.Count() > 0)
                    {
                        foreach (var item in listorderdetail.ToList())
                        {
                            total = total + item.Sub_Item.Price * item.Amount * (1 - item.Sub_Item.Discount);
                        }
                    }
                }
                return total;
            }
            catch
            {
                return 0;
            }
        }
        public int GetWeightOfSubItem(int itemID)
        {
            var weight = _context.Specification_Values.SingleOrDefault(sv => sv.ItemID == itemID && sv.SpecificationID == 2).Value;
            return int.Parse(weight);
        }

        public void refundnow(int orderID, long amount, long transId, int userID, string FCM_Firebase)
        {
            RefundRequest request = new RefundRequest();
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            request.orderId = orderID + myuuidAsString;
            request.requestId = myuuidAsString;
            request.partnerCode = partnerCode;
            request.lang = "vi";
            request.amount = amount;
            request.description = "";
            request.transId = transId;

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&transId=" + request.transId;

            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse =  client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", httpContent).Result;
            var contents = quickPayResponse.Content.ReadFromJsonAsync<RefundReponse>().Result;
            if (contents != null)
            {
                if (contents.resultCode != 0)
                {
                    DataExchangeUser exchangeUser = new DataExchangeUser();
                    exchangeUser.Create_date = GetVnTime();
                    exchangeUser.ExchangePrice = amount;
                    exchangeUser.ExchangeStatusID = 3;
                    exchangeUser.ExchangeUserName = "Hoàn tiền thanh toán đơn hàng thất bại";
                    _context.DataExchangeUsers.Add(exchangeUser);
                    _context.SaveChanges();

                    _notification.Value.CreateNotifiaction(userID, "Hoàn tiền thanh toán thất bại", null, null, null);
                    Notification notification = new Notification
                    {
                        title = "Hoàn tiền",
                        body = "Hoàn tiền thanh toán thất bại",
                    };
                    FirebaseNotification firebaseNotification = new FirebaseNotification
                    {
                        notification = notification,
                        to = FCM_Firebase,
                    };
                    _notification.Value.PushUserNotificationAsync(firebaseNotification);
                }
            }
        }
        public async Task<ConfirmReponse> confirmCancel(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
            ConfirmRequest request = new ConfirmRequest();
            request.orderId = ordertransaction.OrderIDGateway;
            request.requestId = myuuidAsString;
            request.partnerCode = partnerCode;
            request.lang = "vi";
            request.amount = Gettotalprice(orderID);
            request.description = "";
            request.requestType = "cancel";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&requestType=" + request.requestType;
            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/confirm", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<ConfirmReponse>();
            if (contents.Result.resultCode == 0)
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                order.OrderStatusID = 3;
                ordertransaction.ResultCode = -1;
                _context.SaveChanges();
            }
            return contents.Result;
        }
        public async Task<ConfirmReponse> confirm(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
            ConfirmRequest request = new ConfirmRequest();
            request.orderId = ordertransaction.OrderIDGateway;
            request.requestId = ordertransaction.RequestID;
            request.partnerCode = partnerCode;
            request.lang = "vi";
            request.amount = Gettotalprice(orderID);
            request.description = "";
            request.requestType = "capture";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&requestType=" + request.requestType;

            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/confirm", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<ConfirmReponse>();
            if (contents.Result.resultCode == 0)
            {
                ordertransaction.ResultCode = 0;
                _context.SaveChanges();
            }
            return contents.Result;
        }
        public async Task<RefundReponse> refundtransaction(int orderID, double numrefund)
        {
            RefundRequest request = new RefundRequest();
            Guid myuuid = Guid.NewGuid();
            var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
            var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
            long price = 0;
            if (numrefund == 1)
            {
                price = (long)Gettotalprice(orderID);
            }
            else
            {
                price = (long)((Gettotalprice(orderID) - order.FeeShip) * numrefund);
            }
            string myuuidAsString = myuuid.ToString();
            request.orderId = orderID + myuuidAsString;
            request.requestId = myuuidAsString;
            request.partnerCode = partnerCode;
            request.lang = "vi";
            request.amount = price;
            request.description = "";
            request.transId = ordertransaction.TransactionID;

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&transId=" + request.transId;

            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", httpContent).Result;
            var contents = quickPayResponse.Content.ReadFromJsonAsync<RefundReponse>();
            if (contents.Result.resultCode == 0)
            {
                OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                if (store_Transaction != null)
                {
                    if (numrefund == 1)
                    {
                        store_Transaction.IsActive = false;
                    }
                    else
                    {
                        store_Transaction.Price = (Gettotalprice(orderID) - order.FeeShip) * (1 - numrefund) + order.FeeShip;
                    }
                }
                OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                if (system_Transaction != null)
                {
                    system_Transaction.IsActive = false;
                }
                order.RefundPrice = price;
                ordertransaction.ResultCode = -1;
                _context.SaveChanges();
                return contents.Result;
            }//đối soat hoan thât bại
            DataExchangeUser exchangeUser = new DataExchangeUser();
            exchangeUser.Create_date = GetVnTime();
            exchangeUser.ExchangePrice = price;
            exchangeUser.OrderID = order.OrderID;
            exchangeUser.ExchangeStatusID = 3;
            exchangeUser.ExchangeUserName = "Hoàn tiền thất bại";
            _context.DataExchangeUsers.Add(exchangeUser);
            _context.SaveChanges();
            _notification.Value.CreateNotifiaction(order.UserID, "Hoàn tiền đơn hàng " + orderID, null, order.OrderID, null);
            Notification notificationOfuser = new Notification
            {
                title = "Hoàn tiền",
                body = "Đối soát đơn hàng " + order.OrderID + ", hoàn tiền thất bại",
            };
            FirebaseNotification firebaseNotificationuser = new FirebaseNotification
            {
                notification = notificationOfuser,
                to = order.User.FCM_Firebase,
            };
            _notification.Value.PushUserNotificationAsync(firebaseNotificationuser);
            //admin
            var admin = _context.Users.FirstOrDefault(u => u.RoleID == 1);
            _notification.Value.CreateNotifiaction(admin.UserID, "Hoàn tiền đơn hàng " + orderID + " thất bại", null, order.OrderID, null);
            Notification notificationOfAD = new Notification
            {
                title = "Đối soát - Huỷ đơn",
                body = "Đối soát đơn hàng " + order.OrderID + ", hoàn tiền thất bại",
            };
            FirebaseNotification firebaseNotificationAD = new FirebaseNotification
            {
                notification = notificationOfAD,
                to = admin.FCM_Firebase,
            };
            _notification.Value.PushUserNotificationAsync(firebaseNotificationAD);
            return contents.Result;
        }
        public Result CancelOrder(int orderID, string reason)
        {
            Result result = new Result();
            try
            {
                var check = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 3);
                if (check != null)
                {
                    result.Success = false;
                    result.Message = "Đơn hàn đã hủy trước đó";
                    result.Data = "";
                    return result;
                }
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && (o.OrderStatusID == 1 || o.OrderStatusID == 4) && _context.ShipOrders.OrderByDescending(so => so.Create_Date).LastOrDefault(so => so.OrderID == orderID).Status_ID == "-2" || _context.ShipOrders.OrderByDescending(so => so.Create_Date).LastOrDefault(so => so.OrderID == orderID).Status_ID == "1");
                if (order != null)
                {
                    var shipReponse = _shipReposity.Value.CancelOrder(orderID);
                    if (shipReponse != null)
                    {
                        if (shipReponse.Success)
                        {
                            var comfim = true;
                            if (order.PaymentMethod != "COD")
                            {
                                comfim = RefundOrder(orderID, 1).Success;
                            }
                            var labelID = _context.ShipOrders.FirstOrDefault(so => so.OrderID == orderID).LabelID;
                            order.Reason = reason;
                            order.OrderStatusID = 3;
                            _orderReposity.Value.CancelOrder(orderID);
                            ShipOrder model = new ShipOrder();
                            model.OrderID = order.OrderID;
                            model.Create_Date = GetVnTime();
                            model.LabelID = labelID;
                            model.Reason = "";
                            model.Reason_code = "";
                            model.Status_ID = "-3";
                            _context.ShipOrders.Add(model);
                            _context.SaveChanges();
                            //thong bao
                            var store = GetStoreByorderID(orderID);
                            _notification.Value.CreateNotifiaction(store.UserID, "Huỷ đơn hàng " + orderID, null, order.OrderID, null);
                            Notification notification = new Notification
                            {
                                title = "Huỷ đơn",
                                body = "Huỷ đơn hàng " + order.OrderID,
                            };
                            FirebaseNotification firebaseNotification = new FirebaseNotification
                            {
                                notification = notification,
                                to = store.User.FCM_Firebase,
                            };
                            _notification.Value.PushUserNotificationAsync(firebaseNotification);
                            // user
                            _notification.Value.CreateNotifiaction(order.UserID, "Huỷ đơn hàng " + orderID, null, order.OrderID, null);
                            Notification notificationOfU = new Notification
                            {
                                title = "Huỷ đơn",
                                body = "Huỷ đơn hàng " + order.OrderID + " thành công",
                            };
                            FirebaseNotification firebaseNotificationU = new FirebaseNotification
                            {
                                notification = notificationOfU,
                                to = order.User.FCM_Firebase,
                            };
                            _notification.Value.PushUserNotificationAsync(firebaseNotificationU);

                            result.Success = true;
                            result.Message = "Hủy đơn hàng thành công";
                            result.Data = "";
                            return result;
                        }
                        result.Success = false;
                        result.Message = "Không thể huỷ cho hoá đơn này(shíp service)";
                        result.Data = "";
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Không thể huỷ cho hoá đơn này";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public double GetFeeship(int orderID)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 1);
            return order.FeeShip;
        }
        public Store GetStoreByorderID(int orderID)
        {
            var subitemID = _context.OrderDetails.FirstOrDefault(od => od.OrderID == orderID).Sub_ItemID;
            return GetStoreBySubItemID(subitemID);
        }
        public Store GetStoreBySubItemID(int sub_itemID)
        {
            try
            {
                return _context.Stores.SingleOrDefault(s => s.StoreID == _context.Items.SingleOrDefault(i => _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == sub_itemID).ItemID == i.ItemID).StoreID);
            }
            catch
            {
                return null;
            }
        }
        public Result ConfimOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                var confirmReponse = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
                if (confirmReponse.ResultCode == 0)
                {
                    //ghi nhan doanh thu
                    //store
                    var store = GetStoreByorderID(orderID);
                    var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                    var orderprice = _orderReposity.Value.GetPriceItemOrder(orderID);
                    var shipprice = GetFeeship(orderID);
                    OrderStore_Transaction orderStore_Transaction = new OrderStore_Transaction();
                    orderStore_Transaction.OrderID = orderID;
                    orderStore_Transaction.StoreID = store.StoreID;
                    orderStore_Transaction.IsActive = true;
                    orderStore_Transaction.Price = orderprice * (1 - system.Commission_Precent) + shipprice;
                    orderStore_Transaction.Create_Date = GetVnTime();
                    //store.Asset = store.Asset + orderStore_Transaction.Price; chưa công vô tai khoan
                    //sys
                    OrderSystem_Transaction orderSystem_Transaction = new OrderSystem_Transaction();
                    orderSystem_Transaction.OrderStore_Transaction = orderStore_Transaction;
                    orderSystem_Transaction.SystemID = system.SystemID;
                    orderSystem_Transaction.Create_Date = GetVnTime();
                    orderSystem_Transaction.Price = orderprice * system.Commission_Precent;
                    orderSystem_Transaction.IsActive = true;
                    //system.Asset = system.Asset + orderSystem_Transaction.Price; chưa công vo tai khoan
                    _context.OrderSystem_Transactions.Add(orderSystem_Transaction);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    return result;
                }
                result.Success = false;
                result.Message = "Thất bại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public Result ConfimStoreShipOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                var confirmReponse = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
                var esystem = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                var orderprivce = _orderReposity.Value.GetPriceItemOrder(orderID);
                var storeprice = orderprivce * (1 - esystem.Refund_Precent);
                if (confirmReponse.ResultCode == 0)
                {
                    //ghi nhan tien ship
                    //store
                    var store = GetStoreByorderID(orderID);
                    var shipprice = GetFeeship(orderID);
                    OrderStore_Transaction orderStore_Transaction = new OrderStore_Transaction();
                    orderStore_Transaction.OrderID = orderID;
                    orderStore_Transaction.StoreID = store.StoreID;
                    orderStore_Transaction.IsActive = true;
                    orderStore_Transaction.Price = shipprice + storeprice;
                    orderStore_Transaction.Create_Date = GetVnTime();
                    //store.Asset = store.Asset + orderStore_Transaction.Price;
                    _context.OrderStore_Transactions.Add(orderStore_Transaction);
                    _context.SaveChanges();

                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    return result;
                }
                result.Success = false;
                result.Message = "Thất bại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public Result ConfimCancelOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                ConfirmReponse confirmReponse = confirmCancel(orderID).Result;
                if (confirmReponse.resultCode == 0)
                {
                    result.Success = true;
                    result.Message = confirmReponse.message;
                    result.Data = confirmReponse.resultCode;
                    return result;
                }
                result.Success = false;
                result.Message = confirmReponse.message;
                result.Data = confirmReponse.resultCode;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public async Task<MomoPayReponse> GetPayStoreAsync(int storeID)
        {
            var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
            MomoPayRequest request = new MomoPayRequest();
            //https://esmp.page.link/view
            request.orderInfo = "Thanh toan mo gian hang tren eSMP";
            request.partnerCode = partnerCode;
            request.redirectUrl = "https://gsp23-se-37-supplier.vercel.app/#/";
            request.ipnUrl = "http://esmpfree-001-site1.etempurl.com/api/Payment/store";
            request.amount = (long)system.AmountActive;
            request.orderId = store.StoreID + "-" + myuuidAsString;
            request.requestId = myuuidAsString;
            request.extraData = "";
            request.lang = "vi";
            request.requestType = "captureWallet";
            request.autoCapture = true;


            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&extraData=" + request.extraData + "&ipnUrl=" + request.ipnUrl + "&orderId=" + request.orderId + "&orderInfo=" + request.orderInfo + "&partnerCode=" + request.partnerCode + "&redirectUrl=" + request.redirectUrl + "&requestId=" + request.requestId + "&requestType=" + request.requestType;
            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/create", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<MomoPayReponse>();
            return contents.Result;
        }
        public Result GetStorePayUrl(int storeID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Stores.SingleOrDefault(o => o.StoreID == storeID);
                if (order != null)
                {
                    if (order.Store_StatusID == 1)
                    {
                        result.Success = false;
                        result.Message = "Cửa hàng đã thành toán";
                        result.Data = "";
                        return result;
                    }
                    MomoPayReponse momoPayReponse = GetPayStoreAsync(storeID).Result;
                    if (momoPayReponse != null)
                    {
                        if (momoPayReponse.resultCode == 0)
                        {
                            result.Success = true;
                            result.Message = "Thành công";
                            result.Data = momoPayReponse.payUrl;
                            return result;
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Hệ thống thanh toán lỗi";
                            result.Data = momoPayReponse;
                            return result;
                        }

                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Hệ thống thanh toán đang bảo trì";
                        result.Data = "";
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Đơn hàng không tồn tại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public void PayStoreINP(MomoPayINP payINP)
        {
            try
            {

                if (payINP.resultCode == 0)
                {
                    var storeID = payINP.orderId.Split('-')[0];
                    var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                    var store = _context.Stores.SingleOrDefault(s => s.StoreID == int.Parse(storeID));
                    if (store != null)
                    {
                        store.Store_StatusID = 1;
                        store.MomoTransactionID = payINP.transId;
                        store.Actice_Date = GetVnTime();
                        store.AmountActive = payINP.amount;
                        system.Asset = system.Asset + payINP.amount;
                        _context.SaveChanges();
                        //thong bao
                        _notification.Value.CreateNotifiaction(store.UserID, "Thanh toán thành công", store.StoreID, null, null);
                        Notification notification = new Notification
                        {
                            title = "Thanh toán",
                            body = "Thanh toán kich hoạt cửa hàng thành công ",
                        };
                        var user = _context.Users.SingleOrDefault(u => u.UserID == store.UserID);
                        FirebaseNotification firebaseNotification = new FirebaseNotification
                        {
                            notification = notification,
                            to = user.FCM_Firebase,
                        };
                        _notification.Value.PushUserNotificationAsync(firebaseNotification);
                        //admin
                        var u = _context.Users.FirstOrDefault(u => u.RoleID == 1);
                        _notification.Value.CreateNotifiaction(u.UserID, "Thanh toán thành công", store.StoreID, null, null);
                        Notification notificationadmin = new Notification
                        {
                            title = "Thanh toán",
                            body = "Thanh toán kich hoạt cửa hàng thành công ",
                        };
                        FirebaseNotification firebaseNotificationadmin = new FirebaseNotification
                        {
                            notification = notificationadmin,
                            to = u.FCM_Firebase,
                        };
                        _notification.Value.PushUserNotificationAsync(firebaseNotificationadmin);
                    }
                }
            }
            catch (Exception ex)
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
                role.RoleName = ex.Message;
                _context.SaveChanges();
                return;
            }
        }
        public Result RefundOrder(int orderID, double numrefund)
        {
            Result result = new Result();
            try
            {
                RefundReponse refundReponse = refundtransaction(orderID, numrefund).Result;
                if (refundReponse.resultCode == 0)
                {
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = refundReponse.resultCode;
                    return result;
                }
                result.Success = false;
                result.Message = refundReponse.message;
                result.Data = refundReponse.resultCode;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public async Task<FirebaseReponse> Getapplink(int orderID)
        {
            FirebaseRequest request = new FirebaseRequest();
            //https://esmp.page.link/view
            request.longDynamicLink = "https://esmp.page.link/?link=http://www.google.com?orderID=" + orderID + "&apn=com.example.esmp_project";
            Suffix suffix = new Suffix();
            suffix.option = "UNGUESSABLE";
            request.suffix = suffix;
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

            var quickPayResponse = await client.PostAsync("https://firebasedynamiclinks.googleapis.com/v1/shortLinks?key=AIzaSyBaUaNJe050MkvaSfL2LOw24AnXKN2Sl60", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<FirebaseReponse>();
            return contents.Result;
        }

        public Result ConfimStoreShipLostOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                var confirmReponse = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
                var esystem = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                var orderprivce = _orderReposity.Value.GetPriceItemOrder(orderID);
                if (confirmReponse.ResultCode == 0)
                {
                    //ghi nhan tien ship + tiền hàng
                    //store
                    var store = GetStoreByorderID(orderID);
                    var shipprice = GetFeeship(orderID);
                    OrderStore_Transaction orderStore_Transaction = new OrderStore_Transaction();
                    orderStore_Transaction.OrderID = orderID;
                    orderStore_Transaction.StoreID = store.StoreID;
                    orderStore_Transaction.IsActive = true;
                    orderStore_Transaction.Price = shipprice + orderprivce;
                    orderStore_Transaction.Create_Date = GetVnTime();
                    //store.Asset = store.Asset + orderStore_Transaction.Price;
                    _context.OrderStore_Transactions.Add(orderStore_Transaction);
                    _context.SaveChanges();

                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = "";
                    return result;
                }
                result.Success = false;
                result.Message = "Thất bại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
        public async Task<RefundReponse> refundServicetransaction(int ServiceID, double numrefund)
        {
            RefundRequest request = new RefundRequest();
            Guid myuuid = Guid.NewGuid();
            var afterService = _context.AfterBuyServices.SingleOrDefault(af => af.AfterBuyServiceID == ServiceID);
            var order = _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == ServiceID).OrderDetail.Order;
            var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);

            long price = 0;
            var listServiceDetai = _context.ServiceDetails.Where(sd => sd.AfterBuyServiceID == ServiceID);
            foreach (var item in listServiceDetai.ToList())
            {
                price = (long)(price + item.OrderDetail.PricePurchase * (1 - item.OrderDetail.DiscountPurchase) * item.Amount * numrefund);
            }
            string myuuidAsString = myuuid.ToString();
            request.orderId = afterService.AfterBuyServiceID + myuuidAsString;
            request.requestId = myuuidAsString;
            request.partnerCode = partnerCode;
            request.lang = "vi";
            request.amount = price;
            request.description = "";
            request.transId = ordertransaction.TransactionID;

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&transId=" + request.transId;

            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<RefundReponse>();

            afterService.RefundPrice = price;
            OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
            store_Transaction.Price = store_Transaction.Price - price;
            OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
            system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);
            _context.SaveChanges();

            if (contents.Result.resultCode == 0)
            {
                return contents.Result;
            }//đối soat hoan thât bại
            DataExchangeUser exchangeUser = new DataExchangeUser();
            exchangeUser.Create_date = GetVnTime();
            exchangeUser.ExchangePrice = price;
            exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
            exchangeUser.ExchangeStatusID = 3;
            exchangeUser.ExchangeUserName = "Hoàn tiền thất bại cho don boi hoan";
            _context.DataExchangeUsers.Add(exchangeUser);
            _context.SaveChanges();
            return contents.Result;
        }
        public Result RefundService(int serviceID, double numrefund)
        {
            Result result = new Result();
            try
            {
                RefundReponse refundReponse = refundServicetransaction(serviceID, numrefund).Result;
                if (refundReponse.resultCode == 0)
                {
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = refundReponse.resultCode;
                    return result;
                }
                result.Success = false;
                result.Message = refundReponse.message;
                result.Data = refundReponse.resultCode;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }

        public async Task<string> GetInfoAsync(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();

            var order = _context.orderBuy_Transacsions.SingleOrDefault(ob => ob.OrderID == orderID);

            infoRequest request = new infoRequest();
            //https://esmp.page.link/view
            request.partnerCode = partnerCode;
            request.orderId = order.OrderIDGateway;
            request.requestId = myuuidAsString;
            request.lang = "vi";


            var rawSignature = "accessKey=" + accessKey + "&orderId=" + request.orderId + "&partnerCode=" + partnerCode + "&requestId=" + request.requestId;
            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/query", httpContent);
            var contentss = quickPayResponse.Content.ReadAsStringAsync();
            var contents = quickPayResponse.Content.ReadFromJsonAsync<InfoReponse>();
            return contentss.Result;
        }
        public Result InfoPay(int orderID)
        {
            Result result = new Result();
            try
            {
                var info = GetInfoAsync(orderID).Result;
                /* if (info.resultCode == 0)
                 {
                     result.Success = true;
                     result.Message = "Thành công";
                     result.Data = info.resultCode;
                     return result;
                 }*/
                result.Success = true;
                result.Message = "";
                result.Data = info;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                return result;
            }
        }
    }
}

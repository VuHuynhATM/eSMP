using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.Services.ShipRepo;
using eSMP.Services.StoreRepo;
using eSMP.VModels;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace eSMP.Services.MomoRepo
{
    public class MomoRepository : IMomoReposity
    {
        private readonly WebContext _context;
        private readonly Lazy<IShipReposity> _shipReposity;
        private readonly Lazy<IOrderReposity> _orderReposity;
        string accessKey = "klm05TvNBzhg7h7j";
        string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        string partnerCode = "MOMOBKUN20180529";

        public MomoRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IOrderReposity> orderReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _orderReposity = orderReposity;
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
            request.orderInfo = "Thanh Toan";
            request.partnerCode = partnerCode;
            request.redirectUrl = firebaseReponse.Result.shortLink;
            request.ipnUrl = "http://esmpfree-001-site1.etempurl.com/api/Payment";
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
                    if (order.OrderStatusID != 2)
                    {
                        feeShip = (int)order.FeeShip;
                    }
                    else
                    {
                        feeShip = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, _orderReposity.Value.GetWeightOrder(orderID)).fee.fee;
                    }
                    var priceitem = _orderReposity.Value.GetPriceItemOrder(orderID);
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
                    var ship = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, _orderReposity.Value.GetWeightOrder(order.OrderID));
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
        public Result GetPayUrl(int orderID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
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
                                checkout++;
                            }
                            else
                            {
                                itemerro = itemerro + ", " + GetSub_Item(item.Sub_ItemID).Sub_ItemName;
                            }
                        }
                        if (checkout == listdetail.Count)
                        {
                            if (CheckShip(orderID))
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
                                result.Success = false;
                                result.Message = "Địa điểm giao hàng chưa được hỗ trợ";
                                result.Data = "";
                                return result;
                            }
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Số lượng của sản phầm " + itemerro + " không đủ";
                            result.Data = "";
                            return result;
                        }
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
        public void PayOrderINP(MomoPayINP payINP)
        {
            try
            {

                if (payINP.resultCode == 0)
                {
                    var orderid = payINP.orderId.Split('-')[0];
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == int.Parse(orderid));
                    if (order != null)
                    {
                        OrderBuy_Transacsion transacsion = new OrderBuy_Transacsion();
                        transacsion.OrderID = order.OrderID;
                        DateTime dateTime = new DateTime();
                        dateTime = dateTime.AddMilliseconds(payINP.responseTime).ToUniversalTime();

                        string vnTimeZoneKey = "SE Asia Standard Time";
                        TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                        DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, vnTimeZone);

                        transacsion.Create_Date = VnTime;
                        transacsion.ResultCode = payINP.resultCode;
                        transacsion.MomoTransactionID = payINP.transId;
                        transacsion.OrderIDMOMO = payINP.orderId;
                        transacsion.RequestID = payINP.requestId;
                        _context.orderBuy_Transacsions.Add(transacsion);
                        _context.SaveChanges();
                        //create ship
                        var shipReponse = _shipReposity.Value.CreateOrder(order.OrderID);
                        if (shipReponse != null)
                        {
                            if (shipReponse.success)
                            {
                                //lay tien
                                //ConfirmReponse confirmReponse = confirm(order.OrderID).Result;
                                order.OrderStatusID = 1;
                                order.FeeShip = shipReponse.order.fee;
                                order.Create_Date = GetVnTime();
                                //tạo status
                                _context.SaveChanges();
                                //updateamount
                                var listdetail = _context.OrderDetails.Where(od => od.OrderID == order.OrderID).ToList();
                                foreach (var detail in listdetail)
                                {
                                    var subItem = _context.Sub_Items.SingleOrDefault(si => si.Sub_ItemID == detail.Sub_ItemID);
                                    subItem.Amount = subItem.Amount - detail.Amount;
                                    _context.SaveChanges();
                                }
                            }
                            else
                            {
                                //ConfimCancelOrder(order.OrderID);
                                order.OrderStatusID = 3;
                                order.FeeShip = shipReponse.order.fee;
                                order.Create_Date = GetVnTime();
                                _context.SaveChanges();
                                RefundOrder(order.OrderID);
                                _context.orderBuy_Transacsions.Remove(transacsion);
                            }
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
        public async Task<ConfirmReponse> confirmCancel(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
            ConfirmRequest request = new ConfirmRequest();
            request.orderId = ordertransaction.OrderIDMOMO;
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
            request.orderId = ordertransaction.OrderIDMOMO;
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
        public async Task<RefundReponse> refundtransaction(int orderID)
        {
            RefundRequest request = new RefundRequest();
            Guid myuuid = Guid.NewGuid();
            var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == orderID);
            string myuuidAsString = myuuid.ToString();
            request.orderId = orderID + myuuidAsString;
            request.requestId = myuuidAsString;
            request.partnerCode = partnerCode;
            request.lang = "vi";
            request.amount = Gettotalprice(orderID);
            request.description = "";
            request.transId = ordertransaction.MomoTransactionID;

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&transId=" + request.transId;

            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<RefundReponse>();
            if (contents.Result.resultCode == 0)
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                order.OrderStatusID = 3;
                ordertransaction.ResultCode = -1;
                _context.SaveChanges();
            }
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
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 1 && _context.ShipOrders.OrderByDescending(so => so.Create_Date).LastOrDefault(so => so.OrderID == orderID).Status_ID == "-2" || _context.ShipOrders.OrderByDescending(so => so.Create_Date).LastOrDefault(so => so.OrderID == orderID).Status_ID == "1");
                if (order != null)
                {
                    var shipReponse = _shipReposity.Value.CancelOrder(orderID);
                    if (shipReponse != null)
                    {
                        if (shipReponse.Success)
                        {
                            var comfim = RefundOrder(orderID);
                            if (comfim.Success)
                            {
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

                                result.Success = true;
                                result.Message = "Hủy đơn hàng thành công";
                                result.Data = "";
                                return result;
                            }
                            result.Success = false;
                            result.Message = "Thất bại do không thể hoàn tiền";
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
                    store.Asset = store.Asset + orderStore_Transaction.Price;
                    //sys
                    OrderSystem_Transaction orderSystem_Transaction = new OrderSystem_Transaction();
                    orderSystem_Transaction.OrderStore_Transaction = orderStore_Transaction;
                    orderSystem_Transaction.SystemID = system.SystemID;
                    orderSystem_Transaction.Create_Date = GetVnTime();
                    orderSystem_Transaction.Price = orderprice * system.Commission_Precent;
                    orderSystem_Transaction.IsActive = true;
                    system.Asset = system.Asset + orderSystem_Transaction.Price;
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
            request.orderInfo = "Thanh Toan";
            request.partnerCode = partnerCode;
            request.redirectUrl = "https://esmp.page.link/view";
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
                        system.Asset=system.Asset+payINP.amount;
                        _context.SaveChanges();
                        //updateamount
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
        public Result RefundOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                RefundReponse refundReponse = refundtransaction(orderID).Result;
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
    }
}

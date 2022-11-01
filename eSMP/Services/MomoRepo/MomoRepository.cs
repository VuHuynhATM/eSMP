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
        private readonly Lazy<IStoreReposity> _storeReposity;
        string accessKey = "klm05TvNBzhg7h7j";
        string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        string partnerCode = "MOMOBKUN20180529";

        public MomoRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IOrderReposity> orderReposity, Lazy<IStoreReposity> storeReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _orderReposity = orderReposity;
            _storeReposity = storeReposity;
        }
        public async Task<MomoPayReponse> GetPayAsync(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();


            MomoPayRequest request = new MomoPayRequest();
            //https://esmp.page.link/view
            request.orderInfo = "Thanh Toan";
            request.partnerCode = partnerCode;
            request.redirectUrl = "https://esmp.page.link/view";
            request.ipnUrl = "https://1b64-2405-4802-9117-740-601a-1df3-421a-2ef7.ap.ngrok.io/api/Payment";
            request.amount = Gettotalprice(orderID);
            request.orderId = orderID + "-" + myuuidAsString;
            request.requestId = myuuidAsString;
            request.extraData = "";
            request.lang = "vi";
            request.requestType = "captureWallet";
            request.autoCapture = false;


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
                    var feeShip = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, _orderReposity.Value.GetWeightOrder(orderID)).fee.fee;
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
                                /*order.OrderStatusID = 1;
                                _context.SaveChanges();
                                result.Success = true;
                                result.Message = "Thành Công";
                                result.Data = "";
                                return result;
                                var shipReponse = _shipReposity.Value.CreateOrder(orderID);
                                if (shipReponse != null)
                                {
                                    if (shipReponse.success)
                                    {
                                        order.OrderStatusID = 1;
                                        _context.SaveChanges();
                                        result.Success = true;
                                        result.Message = "Thành Công";
                                        result.Data = shipReponse.order;
                                        return result;
                                    }
                                    result.Success = shipReponse.success;
                                    result.Message = shipReponse.message;
                                    result.Data = shipReponse.order;
                                    return result;
                                }*/
                                MomoPayReponse momoPayReponse = GetPayAsync(orderID).Result;
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
                
                if (payINP.resultCode == 9000)
                {
                    var orderid = payINP.orderId.Split('-')[0];
                    
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == int.Parse(orderid));
                    if (order != null)
                    {
                        order.OrderStatusID = 1;
                        _context.SaveChanges();
                        OrderBuy_Transacsion transacsion = new OrderBuy_Transacsion();
                        transacsion.OrderID = order.OrderID;
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddMilliseconds(payINP.responseTime).ToLocalTime();
                        transacsion.Create_Date = dateTime;
                        transacsion.ResultCode = payINP.resultCode;
                        transacsion.MomoTransactionID = payINP.transId;
                        transacsion.OrderIDMOMO = payINP.orderId;
                        transacsion.RequestID = payINP.requestId;
                        _context.orderBuy_Transacsions.Add(transacsion);
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
            var ordertransaction=_context.orderBuy_Transacsions.SingleOrDefault(obt=>obt.OrderID==orderID);
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
            if (contents.Result.resultCode ==0)
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
            request.transId = long.Parse(ordertransaction.OrderIDMOMO);

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&transId=" + request.transId;

            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<RefundReponse>();
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName = contents.Result.message + "-" + contents.Result.resultCode + "-" + contents.Result.amount;
            _context.SaveChanges();
            return contents.Result;
        }
        public Result CancelOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 1 && int.Parse(_context.ShipOrders.LastOrDefault(so => so.OrderID == orderID).Status_ID) < 2 && int.Parse(_context.ShipOrders.LastOrDefault(so => so.OrderID == orderID).Status_ID) != -1);
                if (order != null)
                {
                    var shipReponse=_shipReposity.Value.CancelOrder(orderID);
                    if(shipReponse != null)
                    {
                        if (shipReponse.Success)
                        {
                            if (_orderReposity.Value.CancelOrder(orderID))
                            {
                                order.OrderStatusID = 3;
                                _context.SaveChanges();
                            }
                        }
                        result.Success = shipReponse.Success;
                        result.Message = shipReponse.Message;
                        result.Data = shipReponse.Data;
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
        public Result ConfimOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                ConfirmReponse confirmReponse = confirm(orderID).Result;
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
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var store = _context.Stores.SingleOrDefault(s => s.StoreID == storeID);
            MomoPayRequest request = new MomoPayRequest();
            //https://esmp.page.link/view
            request.orderInfo = "Thanh Toan";
            request.partnerCode = partnerCode;
            request.redirectUrl = "https://esmp.page.link/view";
            request.ipnUrl = "https://1b64-2405-4802-9117-740-601a-1df3-421a-2ef7.ap.ngrok.io/api/Payment/store";
            request.amount = 1000000;
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

                    var store = _context.Stores.SingleOrDefault(s => s.StoreID == int.Parse(storeID));
                    if (store != null)
                    {
                        store.Store_StatusID = 1;
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
    }
}

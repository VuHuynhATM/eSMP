using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.Services.ShipRepo;
using eSMP.VModels;
using System.Collections;
using System.Net;
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
        string accessKey = "iPXneGmrJH0G8FOP";
        string secretKey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
        string partnerCode = "MOMOOJOI20210710";

        public MomoRepository(WebContext context, Lazy<IShipReposity> shipReposity, Lazy<IOrderReposity> orderReposity)
        {
            _context = context;
            _shipReposity = shipReposity;
            _orderReposity = orderReposity;
        }
        public async Task<MomoPayReponse> GetPayAsync(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            

            MomoPayRequest request = new MomoPayRequest();
            //https://esmp.page.link/view
            request.orderInfo = "Thanh Toan";
            request.partnerCode = "MOMOOJOI20210710";
            request.redirectUrl = "https://esmp.page.link/view";
            request.ipnUrl = "http://20.235.113.50/api/Payment";
            request.amount = Gettotalprice(orderID);
            request.orderId = orderID+"-"+ myuuidAsString;
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
                if(order != null)
                {
                    var feeShip = _shipReposity.Value.GetFeeAsync(order.Province, order.District, order.Pick_Province, order.Pick_District, _orderReposity.Value.GetWeightOrder(orderID)).fee.fee;
                    var priceitem = _orderReposity.Value.GetPriceItemOrder(orderID);
                    if(feeShip!=0 && priceitem != 0)
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
                    if(ship != null)
                    {
                        if(ship.success && ship.fee.delivery)
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
                    if (order.IsPay)
                    {
                        result.Success = false;
                        result.Message = "Đơn hàng đã thành toán";
                        result.Data = "";
                        return result;
                    }
                    var listdetail = _context.OrderDetails.Where(od => od.OrderID == orderID).ToList();
                    var checkout = 0;
                    var itemerro= "";
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
                                if(momoPayReponse != null)
                                {
                                    if(momoPayReponse.resultCode == 0)
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
                            result.Message = "Số lượng của sản phầm "+ itemerro + " không đủ";
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
                ConfirmReponse confirmReponse = confirm(payINP).Result;
                if (confirmReponse.resultCode == 0)
                {
                    var orderid = confirmReponse.orderId.Split('-')[0];
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == int.Parse(orderid));
                    if (order != null)
                    {
                        order.IsPay = true;
                        _context.SaveChanges();

                        OrderBuy_Transacsion transacsion = new OrderBuy_Transacsion();
                        transacsion.OrderID = order.OrderID;
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        dateTime = dateTime.AddMilliseconds(confirmReponse.responseTime).ToLocalTime();
                        transacsion.Create_Date = dateTime;
                        transacsion.ResultCode = confirmReponse.resultCode;
                        transacsion.MomoTransactionID = confirmReponse.transId;
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
            catch
            {
                return;
            }
        }

        public async Task<ConfirmReponse> confirmCancel(MomoPayINP inp)
        {
            ConfirmRequest request = new ConfirmRequest();
            request.orderId = inp.orderId;
            request.requestId = inp.requestId;
            request.partnerCode = inp.partnerCode;
            request.lang = "vi";
            request.amount = inp.amount;
            request.description = "";
            request.requestType = "cancel";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode  + "&requestId=" + request.requestId + "&requestType="+request.requestType;
            request.signature = getSignature(rawSignature, secretKey);
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName = request.orderId+ request.requestId + request.partnerCode+ request.lang+ request.amount+ request.description+ request.requestType+ rawSignature;
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/confirm", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<ConfirmReponse>();
            return contents.Result;
        }

        public async Task<ConfirmReponse> confirm(MomoPayINP inp)
        {
            ConfirmRequest request = new ConfirmRequest();
            request.orderId = inp.orderId;
            request.requestId = inp.requestId;
            request.partnerCode = inp.partnerCode;
            request.lang = "vi";
            request.amount = inp.amount;
            request.description = "";
            request.requestType = "capture";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId + "&partnerCode=" + request.partnerCode + "&requestId=" + request.requestId + "&requestType=" + request.requestType;
            request.signature = getSignature(rawSignature, secretKey);
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName = request.orderId + request.requestId + request.partnerCode + request.lang + request.amount + request.description + request.requestType + rawSignature;
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/confirm", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<ConfirmReponse>();
            return contents.Result;
        }

        public Result RefundOrder(int orderID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.IsPay);
                if(order != null)
                {
                    RefundReponse rp = RefundOrderAsync(orderID).Result;
                    if(rp != null)
                    {
                        if (rp.resultCode == 0)
                        {
                            OrderRefund_Transaction orderRefund_Transaction = new OrderRefund_Transaction();
                            orderRefund_Transaction.OrderID = orderID;
                            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                            dateTime = dateTime.AddMilliseconds(rp.responseTime).ToLocalTime();
                            orderRefund_Transaction.Create_Date = dateTime;
                            orderRefund_Transaction.ResultCode = rp.resultCode ;
                            orderRefund_Transaction.MomoTransactionID = rp.transId;
                            _context.orderRefund_Transactions.Add(orderRefund_Transaction);
                            _context.SaveChanges();
                            result.Success = true;
                            result.Message = "Thành công";
                            result.Data = "";
                            return result;
                        }
                    }
                }
                result.Success = false;
                result.Message = "Không thể hoàn tiền cho hoá đơn này";
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
        public async Task<RefundReponse> RefundOrderAsync(int orderID)
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            var ordertransaction=_context.orderBuy_Transacsions.SingleOrDefault(r => r.OrderID == orderID);
            RefundRequest request = new RefundRequest();
            request.partnerCode = partnerCode;
            request.orderId = ordertransaction.OrderID+"-"+ myuuidAsString;
            request.requestId = myuuidAsString;
            request.amount =(long)(GetFeeship(orderID)+_orderReposity.Value.GetPriceItemOrder(orderID));
            request.description = "";
            request.transId = ordertransaction.MomoTransactionID;
            request.lang = "vi";

            var rawSignature = "accessKey=" + accessKey + "&amount=" + request.amount + "&description=" + request.description + "&orderId=" + request.orderId+ "&partnerCode=" + request.partnerCode  + "&requestId=" + request.requestId + "&transId=" + request.transId;
            request.signature = getSignature(rawSignature, secretKey);
            var client = new HttpClient();
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");

            var quickPayResponse = await client.PostAsync("https://test-payment.momo.vn/v2/gateway/api/refund", httpContent);
            var contents = quickPayResponse.Content.ReadFromJsonAsync<RefundReponse>();
            var content = quickPayResponse.Content.ReadAsStringAsync().Result;
            return contents.Result;
        }
        public double GetFeeship(int orderID)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.IsPay);
            return order.FeeShip;
        }
       
    }
}

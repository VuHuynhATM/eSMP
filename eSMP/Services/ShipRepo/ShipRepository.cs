using eSMP.Models;
using eSMP.Services.AutoService;
using eSMP.Services.MomoRepo;
using eSMP.Services.NotificationRepo;
using eSMP.Services.OrderRepo;
using eSMP.Services.StoreRepo;
using eSMP.VModels;
using System;
using System.IO;
using System.Net.Mime;
using System.Text.Json;
using static Google.Apis.Requests.BatchRequest;
using Notification = eSMP.VModels.Notification;

namespace eSMP.Services.ShipRepo
{
    //https://khachhang-staging.ghtklab.com
    public class ShipRepository : IShipReposity
    {
        public static string TOKEN = "D1f7d11dFA5D417D35F136A02e0C812EEC613Fcb";
        public static string deliver_option = "none";

        private readonly WebContext _context;
        private readonly Lazy<IOrderReposity> _orderReposity;
        private readonly Lazy<IMomoReposity> _momoReposity;
        private readonly Lazy<INotificationReposity> _notification;

        public ShipRepository(WebContext context, Lazy<IOrderReposity> orderReposity, Lazy<IMomoReposity> momoReposity, Lazy<INotificationReposity> notification)
        {
            _context = context;
            _orderReposity = orderReposity;
            _momoReposity = momoReposity;
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

        public async Task<FeeReponse> GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight, string deliver_option)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            var shipResponse = await client.GetAsync("https://services-staging.ghtklab.com/services/shipment/fee?pick_province=" + pick_province + "&pick_district=" + pick_district + "&province=" + province + "&district=" + district + "&weight=" + weight + "&deliver_option=" + deliver_option + "&transport=road");
            var content = shipResponse.Content.ReadFromJsonAsync<FeeReponse>();
            return content.Result;
        }

        public FeeReponse GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight)
        {
            return GetFeeAsync(province, district, pick_province, pick_district, weight, deliver_option).Result;
        }

        public bool CallBackAsync(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason)
        {
            try
            {
                //check service or order
                bool checkorder = false;
                var orderID = -1;
                var afterServiceID = -1;
                if (partner_id.IndexOf('_') < 0)
                {
                    checkorder = true;
                    orderID = int.Parse(partner_id);
                }
                else
                {
                    afterServiceID = int.Parse(partner_id.Split('_')[0]);
                    orderID = _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == afterServiceID).OrderDetail.Order.OrderID;
                }

                DateTime datetime = DateTime.Parse(action_time);
                DateTime cstTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(datetime, TimeZoneInfo.Local.Id, "SE Asia Standard Time");
                if (checkorder)
                {
                    var shipdb = _context.ShipOrders.SingleOrDefault(so => so.OrderID == orderID && so.Status_ID == status_id + "" && so.Create_Date == cstTime);
                    if (shipdb != null)
                    {
                        return false;
                    }
                }
                else
                {
                    var shipdb = _context.ShipOrders.SingleOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Status_ID == status_id + "" && so.Create_Date == cstTime);
                    if (shipdb != null)
                    {
                        return false;
                    }
                }

                ShipOrder shipOrder = new ShipOrder();
                shipOrder.Status_ID = status_id + "";
                shipOrder.Create_Date = cstTime;
                shipOrder.LabelID = label_id;
                shipOrder.Reason = reason;
                if (checkorder)
                    shipOrder.OrderID = orderID;
                else
                    shipOrder.AfterBuyServiceID = afterServiceID;
                shipOrder.Reason_code = reason_code;
                _context.ShipOrders.Add(shipOrder);
                _context.SaveChanges();
                var afterService = _context.AfterBuyServices.SingleOrDefault(af => af.AfterBuyServiceID == afterServiceID);
                //dối soát gh tk
                if (status_id == 6)
                {
                    if (checkorder)
                    {
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID!=6);
                        order.OrderStatusID = 1;
                        _context.SaveChanges();
                    }
                    else
                    {
                        var order = _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == afterServiceID).OrderDetail.Order;

                        if (partner_id.Split('_')[1] == "user" && afterService.ServiceType == 2)
                        {
                            if (order.PaymentMethod != "COD")
                            {
                                var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                            }
                            else
                            {
                                DataExchangeUser exchangeUser = new DataExchangeUser();
                                exchangeUser.Create_date = GetVnTime();
                                exchangeUser.ExchangePrice = GetPriceItemOrderService(afterServiceID);
                                exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                exchangeUser.ExchangeStatusID = 3;
                                exchangeUser.ExchangeUserName = "Đối soát cho đơn đổi, trả hoàn";
                                _context.DataExchangeUsers.Add(exchangeUser);
                                //giam tien khi giair ngan
                                double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                store_Transaction.Price = store_Transaction.Price - price;
                                OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);
                                _context.SaveChanges();
                            }
                            afterService.ServicestatusID = 1;
                            order.OrderStatusID = 1;
                            _context.SaveChanges();
                        }
                        else if (partner_id.Split('_')[1] == "user" && afterService.ServiceType == 1)
                        {
                            ShipReponse shipReponse = CreateOrderService(afterService.AfterBuyServiceID, "store");
                            if(shipReponse != null)// tao doi hang khac gui cho nguoi mua
                            {
                                if (!shipReponse.success)
                                {
                                    if (order.PaymentMethod != "COD")
                                    {
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                    else
                                    {
                                        DataExchangeUser exchangeUser = new DataExchangeUser();
                                        exchangeUser.Create_date = GetVnTime();
                                        exchangeUser.ExchangePrice = GetPriceItemOrderService(afterServiceID);
                                        exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                        exchangeUser.ExchangeStatusID = 3;
                                        exchangeUser.ExchangeUserName = "Đối soát cho đơn đổi, trả hoàn";
                                        afterService.FeeShipSercond = shipReponse.order.fee;
                                        _context.DataExchangeUsers.Add(exchangeUser);
                                        _context.SaveChanges();
                                    }
                                    afterService.ServicestatusID = 1;
                                    order.OrderStatusID = 1;
                                    _context.SaveChanges();
                                }
                            }
                        }
                        else if(partner_id.Split('_')[1] == "store")
                        {
                            afterService.ServicestatusID = 1;
                            order.OrderStatusID = 1;
                            _context.SaveChanges();
                        }
                    }

                }
                //giao hang thanh cong
                if (status_id == 5)
                {
                    if (checkorder)
                    {
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                        if (order.PaymentMethod == "COD")
                        {
                            order.OrderStatusID = 1;
                            _context.SaveChanges();
                        }
                        if (shipOrder.OrderID != null)
                        {
                            var comfim = _momoReposity.Value.ConfimOrder(shipOrder.OrderID.Value);
                        }
                    }
                }
                //giao hang that bai
                if (status_id == 11)
                {
                    if (checkorder)
                    {
                        var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                        var refundpre = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1).Refund_Precent;
                        //kt hàng đã trả cho supplier chưa
                        var statusshipcheck = _context.ShipOrders.FirstOrDefault(so => so.OrderID == orderID && so.Status_ID == "21");
                        var status130 = _context.ShipOrders.FirstOrDefault(so => so.OrderID == orderID && so.Reason_code == "130");
                        var status131 = _context.ShipOrders.FirstOrDefault(so => so.OrderID == orderID && so.Reason_code == "131");
                        var status132 = _context.ShipOrders.FirstOrDefault(so => so.OrderID == orderID && so.Reason_code == "132");
                        var status13 = _context.ShipOrders.FirstOrDefault(so => so.OrderID == orderID && so.Status_ID == "13");

                        if (status130 != null && statusshipcheck != null)
                        {
                            if (shipOrder.OrderID != null)
                            {
                                var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID.Value, 1);
                            }
                        }
                        //doi soat khi giao hang that bai
                        else if ((status131 != null || status132 != null) && statusshipcheck != null)
                        {
                            if (shipOrder.OrderID != null)
                            {
                                var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID.Value, refundpre);
                                if (comfim.Success)
                                {
                                    _momoReposity.Value.ConfimStoreShipOrder(shipOrder.OrderID.Value);
                                }
                            }
                        }
                        else if (statusshipcheck == null && status13 != null)// mat hang va da doi soat vs ghtk
                        {
                            /*var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID, 1);
                            if (comfim.Success)
                            {
                                _momoReposity.Value.ConfimStoreShipLostOrder(shipOrder.OrderID);
                            }*/
                            //tao doi soat vs shop
                            DataExchangeStore exchangeStore = new DataExchangeStore();
                            exchangeStore.Create_date = GetVnTime();
                            exchangeStore.ExchangePrice = GetPriceItemOrder(order.OrderID) + order.FeeShip;
                            exchangeStore.OrderID = order.OrderID;
                            exchangeStore.ExchangeStatusID = 3;
                            exchangeStore.ExchangeStoreName = "Đơn hàng";
                            _context.DataExchangeStores.Add(exchangeStore);
                            _context.SaveChanges();
                            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 6);
                            role.RoleName = "tao doisoatvs store e";
                            _context.SaveChangesAsync();
                            // hoan tien cho nguoi mua
                            if (order.PaymentMethod != "COD")
                            {
                                if (shipOrder.OrderID != null)
                                {
                                    var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID.Value, 1);
                                }
                            }
                        }
                        else if (statusshipcheck != null)
                        {
                            if (shipOrder.OrderID != null)
                            {
                                var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID.Value, 1);
                            }
                        }
                    }
                    else//service
                    {
                        if (partner_id.Split('_')[1] == "user")// nguoiw mua gui hang di
                        {
                            var refundpre = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1).Refund_Precent;
                            var order = _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == afterServiceID).OrderDetail.Order;

                            //kt hàng đã trả cho supplier chưa
                            var statusshipcheck = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Status_ID == "21");
                            var status130 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Reason_code == "130");
                            var status131 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Reason_code == "131");
                            var status132 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Reason_code == "132");
                            var status13 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Status_ID == "13");

                            if (status130 != null && statusshipcheck != null)//nguoi mua tra hang ve nhung shop ko chap nhan ---> dich vu vo hieu
                            {
                                if (shipOrder.AfterBuyServiceID != null)
                                {
                                    // giam tien don hang se giai ngan
                                    //var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    
                                }
                            }
                            else if ((status131 != null || status132 != null) && statusshipcheck != null)// người mua tra hàng ve nhung ko lien lac duoc vs shop
                            {
                                if (order.PaymentMethod != "COD")// tra tien dich vu cho nguoi thanh toan, tao doi soat cho người shipCOD giam tien doanh thu tu dơn hàng 
                                {
                                    if(shipOrder.AfterBuyServiceID != null)
                                {
                                        //giam tien don hang se giai ngan
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                }
                                else
                                {
                                    double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);

                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = price;
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Bồi hoàn đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);

                                    //giam tien khi giair ngan
                                    var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                    OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                    store_Transaction.Price = store_Transaction.Price - price;
                                    OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                    system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);

                                    _context.SaveChanges();
                                }
                                
                            }
                            else if (statusshipcheck == null && status13 != null)// mat hang va da doi soat vs ghtk
                            {
                                //tao doi soat vs user
                                // hoan tien cho nguoi mua
                                double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                if (order.PaymentMethod != "COD")//tra tien dich vu cho nguoi thanh toan, tao doi soat cho người shipCOD giam tien doanh thu tu dơn hàng, tao đối sot vs shop
                                {
                                    if (shipOrder.AfterBuyServiceID != null)
                                    {
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                }
                                else
                                {
                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = price;
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Mất hàng đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);
                                    //_context.SaveChanges();
                                    //giam tien khi giair ngan
                                    var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                    OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                    store_Transaction.Price = store_Transaction.Price - price;
                                    OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                    system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);
                                    _context.SaveChanges();
                                }
                                
                                //bt cho shop
                                DataExchangeStore exchangeStore = new DataExchangeStore();
                                exchangeStore.Create_date = GetVnTime();
                                exchangeStore.ExchangePrice = price;
                                exchangeStore.OrderID = order.OrderID;
                                exchangeStore.ExchangeStatusID = 3;
                                exchangeStore.ExchangeStoreName = "Mất hàng đơn đổi, trả hoàn";
                                _context.DataExchangeStores.Add(exchangeStore);
                                _context.SaveChanges();

                            }
                            else if (statusshipcheck != null)//ngoai lỗi 2 lỗi của shop thì con lại là của người mua(người mua gủi di)
                            {
                                /*double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                if (order.PaymentMethod != "COD")
                                {
                                    if (shipOrder.AfterBuyServiceID != null)
                                    {
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                }
                                else
                                {
                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = price + afterService.FeeShip.Value;
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);
                                    _context.SaveChanges();
                                }
                                //giam tien khi giair ngan
                                var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                store_Transaction.Price = store_Transaction.Price - price;
                                OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);
*/
                            }
                            afterService.ServicestatusID = 1;
                            _context.SaveChanges();
                        }
                        else//shop gui hang di
                        {
                            var refundpre = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1).Refund_Precent;
                            var order = _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == afterServiceID).OrderDetail.Order;
                            //kt hàng đã trả cho supplier chưa
                            var statusshipcheck = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Status_ID == "21");
                            var status130 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Reason_code == "130");
                            var status131 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Reason_code == "131");
                            var status132 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Reason_code == "132");
                            var status13 = _context.ShipOrders.FirstOrDefault(so => so.AfterBuyServiceID == afterServiceID && so.Status_ID == "13");

                            if (status130 != null && statusshipcheck != null)//Người mua không đồng ý nhận sản phẩm-> hoan tiền cho người mua,nếu ship cod tạo đối soát, giảm tiền lời đơn hàng
                            {
                                if (order.PaymentMethod != "COD")
                                {
                                    if (shipOrder.AfterBuyServiceID != null)
                                    {
                                        // giam tien don hang se giai ngan
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                }
                                else
                                {
                                    double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = price;
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Bồi hoàn đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);

                                    var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                    OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                    store_Transaction.Price = store_Transaction.Price - price;
                                    OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                    system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);

                                    _context.SaveChanges();
                                }
                                
                            }
                            else if ((status131 != null || status132 != null) && statusshipcheck != null)// lien hệ khong được người mua , hoàn tiền cho người mua , tạo đối soát cho ship cod, giảm lời đơn hàng chỉ hoản lại refundpre%%% 
                            {
                                if (order.PaymentMethod != "COD")
                                {
                                    if (shipOrder.AfterBuyServiceID != null)
                                    {
                                        //giam tien don hang se giai ngan
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, refundpre);
                                    }
                                }
                                else
                                {
                                    double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = price * refundpre;
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Bồi hoàn đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);

                                    var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                    OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                    store_Transaction.Price = store_Transaction.Price - price * refundpre;
                                    OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                    system_Transaction.Price = system_Transaction.Price - (price * refundpre * system_Transaction.eSMP_System.Commission_Precent);

                                    _context.SaveChanges();
                                }
                                
                            }
                            else if (statusshipcheck == null && status13 != null)// mat hang va da doi soat vs ghtk
                            {
                                // hoan tien cho nguoi mua, tạo đối soát cho shipcod,tao đối soat có shop, giảm tiền lời
                                double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                if (order.PaymentMethod != "COD")
                                {
                                    if (shipOrder.AfterBuyServiceID != null)
                                    {
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                }
                                else
                                {
                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Bồi hoàn đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);

                                    var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                    OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                    store_Transaction.Price = store_Transaction.Price - price;
                                    OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                    system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);
                                    
                                    _context.SaveChanges();
                                }
                                
                                //bt cho shop
                                DataExchangeStore exchangeStore = new DataExchangeStore();
                                exchangeStore.Create_date = GetVnTime();
                                exchangeStore.ExchangePrice = price;
                                exchangeStore.OrderID = order.OrderID;
                                exchangeStore.ExchangeStatusID = 3;
                                exchangeStore.ExchangeStoreName = "Bồi hoàn đơn đổi, trả hoàn";
                                _context.DataExchangeStores.Add(exchangeStore);
                                _context.SaveChanges();
                            }
                            else if (statusshipcheck != null)// ngoài trừ 2 lỗi của người mua(shop gửi đi) hoàn lại tiền cho người mua, tao doi soát cod
                            {
                                double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                                if (order.PaymentMethod != "COD")
                                {
                                    if (shipOrder.AfterBuyServiceID != null)
                                    {
                                        var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                                    }
                                }
                                else
                                {
                                    DataExchangeUser exchangeUser = new DataExchangeUser();
                                    exchangeUser.Create_date = GetVnTime();
                                    exchangeUser.ExchangePrice = price ;
                                    exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                    exchangeUser.ExchangeStatusID = 3;
                                    exchangeUser.ExchangeUserName = "Đơn đổi, trả hoàn";
                                    _context.DataExchangeUsers.Add(exchangeUser);
                                    //giam tien khi giair ngan
                                    var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                    OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                    store_Transaction.Price = store_Transaction.Price - price;
                                    OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                    system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);
                                    _context.SaveChanges();
                                }
                                
                            }
                            afterService.ServicestatusID = 1;
                            _context.SaveChanges();
                        }
                    }
                }
                // không lấy được hàng
                if (status_id == 7)
                {
                    if (checkorder)
                    {
                        if (shipOrder.OrderID != null)
                        {
                            var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID.Value, 1);
                        }
                    }
                    else
                    {
                        if (partner_id.Split('_')[1] == "store")//ko lấy được hàng tại shop> hoàn tiền
                        {
                            
                            var order = _context.ServiceDetails.FirstOrDefault(sd => sd.AfterBuyServiceID == afterServiceID).OrderDetail.Order;
                            double price = GetPriceItemOrderService(afterService.AfterBuyServiceID);
                            if (order.PaymentMethod == "COD")
                            {
                                DataExchangeUser exchangeUser = new DataExchangeUser();
                                exchangeUser.Create_date = GetVnTime();
                                exchangeUser.ExchangePrice = GetPriceItemOrderService(afterService.AfterBuyServiceID) + afterService.FeeShipSercond.Value;
                                exchangeUser.AfterBuyServiceID = afterService.AfterBuyServiceID;
                                exchangeUser.ExchangeStatusID = 3;
                                exchangeUser.ExchangeUserName = "Bồi hoàn đơn đổi, trả hoàn";
                                _context.DataExchangeUsers.Add(exchangeUser);

                                var ordertransaction = _context.orderBuy_Transacsions.SingleOrDefault(obt => obt.OrderID == order.OrderID);
                                OrderStore_Transaction store_Transaction = _context.OrderStore_Transactions.SingleOrDefault(os => os.OrderStore_TransactionID == ordertransaction.TransactionID);
                                store_Transaction.Price = store_Transaction.Price - price;
                                OrderSystem_Transaction system_Transaction = _context.OrderSystem_Transactions.SingleOrDefault(so => so.OrderStore_TransactionID == store_Transaction.OrderStore_TransactionID);
                                system_Transaction.Price = system_Transaction.Price - (price * system_Transaction.eSMP_System.Commission_Precent);

                                _context.SaveChanges();
                            }
                            else
                            {
                                var comfim = _momoReposity.Value.RefundService(shipOrder.AfterBuyServiceID.Value, 1);
                            }
                        }
                        // cac truong hop còn lại là ko lấy được hàng tại người mua coi như sẻvicẻ đã hoan thành
                        afterService.ServicestatusID = 1;
                        _context.SaveChanges();
                    }
                }
                //thhong bao
                var statustext = "";
                var status = _context.ShipStatuses.SingleOrDefault(s => s.Status_ID == status_id + "");
                if (status != null)
                {
                    statustext = status.Status_Name;
                }
                var ordernoty = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                _notification.Value.CreateNotifiaction(ordernoty.UserID, "cập nhập trạng thái đơn hàng: " + statustext, null, orderID, null);
                Notification notification = new Notification
                {
                    title = "Cập nhập đơn hàng " + orderID,
                    body = "Đơn hàng " + orderID + " đang trong trạng thái: " + shipOrder.ShipStatus.Status_Name,
                };
                var user = _context.Users.SingleOrDefault(u => u.UserID == ordernoty.UserID);
                FirebaseNotification firebaseNotification = new FirebaseNotification
                {
                    notification = notification,
                    to = user.FCM_Firebase,
                };
                _notification.Value.PushUserNotificationAsync(firebaseNotification);
                //supplier
                var store = GetStore(orderID);
                _notification.Value.CreateNotifiaction(store.UserID, "cập nhập trạng thái đơn hàng: " + statustext, null, orderID, null);
                Notification notificationsup = new Notification
                {
                    title = "Cập nhập đơn hàng " + orderID,
                    body = "Đơn hàng " + orderID + " đang trong trạng thái: " + shipOrder.ShipStatus.Status_Name,
                };
                var userspu = _context.Users.SingleOrDefault(u => u.UserID == store.UserID);
                FirebaseNotification firebaseNotificationsup = new FirebaseNotification
                {
                    notification = notificationsup,
                    to = userspu.FCM_Firebase,
                };
                _notification.Value.PushUserNotificationAsync(firebaseNotificationsup);
                return true;
            }
            catch (Exception ex)
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 6);
                role.RoleName = ex.Message;
                _context.SaveChangesAsync();
                return false;
            }
        }
        public Store GetStore(int orderID)
        {
            try
            {
                var orderdetail = _context.OrderDetails.FirstOrDefault(od => od.OrderID == orderID);
                var store = GetStoreBySubItemID(orderdetail.Sub_ItemID);
                return store;
            }
            catch
            {
                return null;
            }
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
        public async Task<ShipReponse> CreateOrderAsync(ShipOrderRequest request)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var ghtkreponde = await client.PostAsync("https://services-staging.ghtklab.com/services/shipment/order/?ver=1.5", httpContent);
            var contents = ghtkreponde.Content.ReadFromJsonAsync<ShipReponse>().Result;
            return contents;
        }
        public int GetWeightOfSubItem(int itemID)
        {
            var weight = _context.Specification_Values.SingleOrDefault(sv => sv.ItemID == itemID && sv.SpecificationID == 2).Value;
            return int.Parse(weight);
        }

        public ShipReponse CreateOrder(int orderID)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID == 2);
                var listoderdetai = _orderReposity.Value.GetOrderDetailModels(orderID, 2);
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
                var priceOrder = _orderReposity.Value.GetPriceItemOrder(orderID);
                orderrequest shiporder = new orderrequest
                {
                    id = orderID + "",
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
                    shiporder.pick_money = (int)GetPriceItemOrder(orderID);
                }
                var system = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1);
                if (system != null){
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
                    var Shipreponse = CreateOrderAsync(request).Result;

                    if (Shipreponse.success)
                    {
                        ShipOrder shipOrder = new ShipOrder();
                        shipOrder.Status_ID = "-2";
                        DateTime datetime = GetVnTime();
                        shipOrder.Create_Date = datetime;
                        shipOrder.LabelID = Shipreponse.order.label;
                        shipOrder.Reason = "";
                        shipOrder.OrderID = int.Parse(Shipreponse.order.partner_id);
                        shipOrder.Reason_code = "";
                        order.Pick_Time = Shipreponse.order.estimated_pick_time;
                        order.Deliver_time = Shipreponse.order.estimated_deliver_time;
                        _context.ShipOrders.Add(shipOrder);
                        _context.SaveChanges();
                    }
                    else
                    {
                        ShipOrder shipOrder = new ShipOrder();
                        shipOrder.Status_ID = "-1";
                        DateTime datetime = GetVnTime();
                        shipOrder.Create_Date = datetime;
                        shipOrder.LabelID = orderID + "";
                        shipOrder.Reason = Shipreponse.message;
                        shipOrder.OrderID = orderID;
                        shipOrder.Reason_code = "";

                        _context.ShipOrders.Add(shipOrder);
                        _context.SaveChanges();
                    }

                    return Shipreponse;
                }
                return null;
            }
            catch (Exception ex)
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
                role.RoleName = role.RoleName + ex.Message;
                _context.SaveChanges();
                return null;
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
        public ShipStatus GetShipStatus(string statusID)
        {
            return _context.ShipStatuses.SingleOrDefault(ss => ss.Status_ID.Equals(statusID));
        }

        public Result GetShipstatus(int? orderID, int? serviceID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                if (orderID.HasValue)
                {
                    var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID != 2);
                    if (order != null)
                    {
                        var liststatus = _context.ShipOrders.AsQueryable();
                        liststatus = liststatus.Where(so => so.OrderID == orderID);
                        liststatus = liststatus.OrderBy(so => so.Create_Date);
                        ShipModel model = new ShipModel();
                        List<ShipStatusModel> list = new List<ShipStatusModel>();
                        if (liststatus.Count() > 0)
                        {
                            foreach (var item in liststatus.ToList())
                            {
                                if (list.Count() == 0)
                                {
                                    model.orderID = item.OrderID;
                                    model.LabelID = item.LabelID;
                                }
                                ShipStatusModel statusModel = new ShipStatusModel
                                {
                                    status = GetShipStatus(item.Status_ID).Status_Name,
                                    Create_Date = item.Create_Date,
                                    Reason = item.Reason,
                                    Reason_code = item.Reason_code
                                };
                                list.Add(statusModel);
                            }
                            model.shipStatusModels = list;
                            result.Success = true;
                            result.Message = "Thành công";
                            result.Data = model;
                            result.TotalPage = numpage;
                            return result;
                        }
                    }
                }
                if (serviceID.HasValue)
                {
                    var service = _context.AfterBuyServices.SingleOrDefault(o => o.AfterBuyServiceID == serviceID);
                    if (service != null)
                    {
                        var liststatus = _context.ShipOrders.AsQueryable();
                        liststatus = liststatus.Where(so => so.AfterBuyServiceID == serviceID);
                        liststatus = liststatus.OrderBy(so => so.Create_Date);
                        ShipModel model = new ShipModel();
                        List<ShipStatusModel> list = new List<ShipStatusModel>();
                        if (liststatus.Count() > 0)
                        {
                            foreach (var item in liststatus.ToList())
                            {
                                if (list.Count() == 0)
                                {
                                    model.orderID = item.OrderID;
                                    model.LabelID = item.LabelID;
                                }
                                ShipStatusModel statusModel = new ShipStatusModel
                                {
                                    status = GetShipStatus(item.Status_ID).Status_Name,
                                    Create_Date = item.Create_Date,
                                    Reason = item.Reason,
                                    Reason_code = item.Reason_code
                                };
                                list.Add(statusModel);
                            }
                            model.shipStatusModels = list;
                            result.Success = true;
                            result.Message = "Thành công";
                            result.Data = model;
                            result.TotalPage = numpage;
                            return result;
                        }
                    }
                }
                result.Success = false;
                result.Message = "Đơn hàng không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public async Task<ShipReponse> CancelOrderAsync(int orderID)
        {
            ShipCancel request = new ShipCancel();
            request.partner_id = orderID + "";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            StringContent httpContent = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
            var ghtkreponde = await client.PostAsync("https://services-staging.ghtklab.com/services/shipment/cancel/partner_id:" + orderID, httpContent);
            var contents = ghtkreponde.Content.ReadFromJsonAsync<ShipReponse>().Result;
            return contents;
        }

        public Result CancelOrder(int orderID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    var reponse = CancelOrderAsync(orderID).Result;
                    result.Success = reponse.success;
                    result.Message = reponse.message;
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Mã Đơn hàng không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }

        }

        public async Task<System.IO.Stream> GetTicketAsync(string labelID)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            HttpResponseMessage shipResponse = await client.GetAsync("https://services-staging.ghtklab.com/services/label/" + labelID);
            //var jsonreponse = shipResponse.Content.ReadAsStream();
            if (shipResponse.IsSuccessStatusCode)
            {
                System.Net.Http.HttpContent content = shipResponse.Content;
                var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
                return contentStream;
            }
            return null;
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        public object GetTicket(int orderID)
        {
            Result result = new Result();
            try
            {
                var orderStatus = _context.ShipOrders.FirstOrDefault(os => os.OrderID == orderID);
                if (orderStatus != null)
                {
                    Stream stream = GetTicketAsync(orderStatus.LabelID).Result;
                    if (stream != null)
                    {
                        byte[] m_Bytes = ReadToEnd(stream);

                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data = m_Bytes;
                        result.TotalPage = 1;
                        //return GetTicketAsync(orderStatus.LabelID).Result;
                        return result;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "GHTK gặp sự cố thử lại sau";
                        result.Data = "";
                        result.TotalPage = 1;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
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


        public ShipReponse CreateOrderService(int ServiceID, string type)
        {
            try
            {
                var afterBuyService = _context.AfterBuyServices.SingleOrDefault(o => o.AfterBuyServiceID == ServiceID && o.ServicestatusID == 5);
                var listoderdetai = _context.ServiceDetails.Where(sd => sd.AfterBuyServiceID == ServiceID);
                var listproduct = new List<productsShip>();
                foreach (var item in listoderdetai.ToList())
                {
                    productsShip pro = new productsShip
                    {
                        name = item.OrderDetail.Sub_Item.Sub_ItemName,
                        price = (int)item.OrderDetail.PricePurchase,
                        product_code = item.OrderDetail.Sub_ItemID,
                        quantity = item.Amount,
                        weight = GetWeightOfSubItem(item.OrderDetail.Sub_Item.ItemID) / (double)1000,
                    };
                    listproduct.Add(pro);
                }
                var priceOrder = GetPriceItemOrderService(ServiceID);
                orderrequest shiporderuser = new orderrequest();
                if (type == "user")
                {
                    shiporderuser = new orderrequest
                    {
                        id = afterBuyService.AfterBuyServiceID + "_" + type + "_" + afterBuyService.ServiceType,
                        pick_name = afterBuyService.User_Name,
                        pick_address = afterBuyService.User_Address,
                        pick_ward = afterBuyService.User_Ward,
                        pick_district = afterBuyService.User_District,
                        pick_province = afterBuyService.User_Province,
                        pick_street = "",
                        pick_tel = afterBuyService.User_Tel,
                        pick_money = 0,
                        is_freeship = 1,
                        name = afterBuyService.Store_Name,
                        address = afterBuyService.Store_Address,
                        district = afterBuyService.Store_District,
                        province = afterBuyService.Store_Province,
                        hamlet = afterBuyService.Store_Address,
                        street = "",
                        ward = afterBuyService.Store_Ward,
                        tel = afterBuyService.Store_Tel,
                        value = (int)priceOrder,
                        transport = "road",
                        pick_option = "cod",
                    };
                }
                else
                {
                    shiporderuser = new orderrequest
                    {
                        id = afterBuyService.AfterBuyServiceID + "_" + type + "_" + afterBuyService.ServiceType,
                        pick_name = afterBuyService.Store_Name,
                        pick_address = afterBuyService.Store_Address,
                        pick_ward = afterBuyService.Store_Ward,
                        pick_district = afterBuyService.Store_District,
                        pick_province = afterBuyService.Store_Province,
                        pick_street = "",
                        pick_tel = afterBuyService.Store_Tel,
                        pick_money = 0,
                        is_freeship = 1,
                        name = afterBuyService.User_Name,
                        address = afterBuyService.User_Address,
                        district = afterBuyService.User_District,
                        province = afterBuyService.User_Province,
                        hamlet = afterBuyService.User_Address,
                        street = "",
                        ward = afterBuyService.User_Ward,
                        tel = afterBuyService.User_Tel,
                        value = (int)priceOrder,
                        transport = "road",
                        pick_option = "cod",
                    };
                }
                ShipOrderRequest request = new ShipOrderRequest
                {
                    order = shiporderuser,
                    products = listproduct,
                };
                var Shipreponse = CreateOrderAsync(request).Result;

                if (Shipreponse.success)
                {
                    ShipOrder shipOrder = new ShipOrder();
                    shipOrder.Status_ID = "-2";
                    DateTime datetime = GetVnTime();
                    shipOrder.Create_Date = datetime;
                    shipOrder.LabelID = Shipreponse.order.label;
                    shipOrder.Reason = "";
                    shipOrder.AfterBuyServiceID = afterBuyService.AfterBuyServiceID;
                    shipOrder.Reason_code = "";
                    afterBuyService.estimated_pick_time = Shipreponse.order.estimated_pick_time;
                    afterBuyService.estimated_deliver_time = Shipreponse.order.estimated_deliver_time;
                    _context.ShipOrders.Add(shipOrder);
                    _context.SaveChanges();
                }
                return Shipreponse;
            }
            catch (Exception ex)
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
                role.RoleName = role.RoleName + ex.Message;
                _context.SaveChanges();
                return null;
            }
        }
        public double GetPriceItemOrderService(int serviceID)
        {
            double price = _context.ServiceDetails.Where(s => s.AfterBuyServiceID == serviceID).Sum(s => s.Amount * s.OrderDetail.PricePurchase);
            return price;
        }
    }
}

using eSMP.Models;
using eSMP.Services.MomoRepo;
using eSMP.Services.NotificationRepo;
using eSMP.Services.OrderRepo;
using eSMP.VModels;
using System;
using System.Text.Json;
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
            HttpResponseMessage shipResponse = await client.GetAsync(string.Format("https://services.giaohangtietkiem.vn/services/shipment/fee?province={0}&district={1}&pick_province={2}&pick_district={3}&weight={4}&deliver_option={5}&transport=road", province, district, pick_province, pick_district, weight, deliver_option));
            var jsonreponse = await shipResponse.Content.ReadFromJsonAsync<FeeReponse>();
            return jsonreponse;
        }

        public FeeReponse GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight)
        {
            return GetFeeAsync(province, district, pick_province, pick_district, weight, deliver_option).Result;
        }

        public bool CallBackAsync(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason)
        {
            try
            {
                var orderID = int.Parse(partner_id);
                DateTime datetime = DateTime.Parse(action_time);
                DateTime cstTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(datetime, TimeZoneInfo.Local.Id, "SE Asia Standard Time");
                var shipdb = _context.ShipOrders.SingleOrDefault(so => so.OrderID == orderID && so.Status_ID==status_id+"" && so.Create_Date==cstTime );
                if (shipdb != null)
                {
                    return false;
                }
                ShipOrder shipOrder = new ShipOrder();
                shipOrder.Status_ID = status_id + "";
                shipOrder.Create_Date = cstTime;
                shipOrder.LabelID = label_id;
                shipOrder.Reason= reason;
                shipOrder.OrderID=int.Parse(partner_id);
                shipOrder.Reason_code= reason_code;
                _context.ShipOrders.Add(shipOrder);
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                
                //giao hang thanh cong
                if (status_id == 5)
                {
                    var comfim = _momoReposity.Value.ConfimOrder(shipOrder.OrderID);
                }
                //giao hang that bai
                if(status_id ==9)
                {
                    var refundpre = _context.eSMP_Systems.SingleOrDefault(s => s.SystemID == 1).Refund_Precent;
                    if (reason_code == "130")
                    {
                        var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID, 1);
                    }
                    else if (reason_code == "131" || reason_code == "132")
                    {
                        var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID, refundpre);
                        if (comfim.Success)
                        {
                            _momoReposity.Value.ConfimStoreShipOrder(shipOrder.OrderID);
                        }
                    }
                    else
                    {
                        var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID, 1);
                    }
                }
                // không lấy được hàng
                if (status_id == 7)
                {
                    var comfim = _momoReposity.Value.RefundOrder(shipOrder.OrderID, 1);
                }
                _context.SaveChanges();
                //thhong bao
                var statustext = "";
                var status = _context.ShipStatuses.SingleOrDefault(s => s.Status_ID == status_id+"");
                if (status != null)
                {
                    statustext=status.Status_Name;
                }
                _notification.Value.CreateNotifiaction(order.UserID, "cập nhập trạng thái đơn hàng: "+ statustext, null, orderID, null);
                Notification notification = new Notification
                {
                    title = "Cập nhập đơn hàng " + orderID,
                    body = "Đơn hàng " + orderID + " đang trong trạng thái: " + shipOrder.ShipStatus.Status_Name,
                };
                var user = _context.Users.SingleOrDefault(u => u.UserID == order.UserID);
                FirebaseNotification firebaseNotification = new FirebaseNotification
                {
                    notification = notification,
                    to = user.FCM_Firebase,
                };
                _notification.Value.PushUserNotificationAsync(firebaseNotification);
                return true;
            }
            catch (Exception ex)
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
                role.RoleName = ex.Message;
                _context.SaveChanges();
                return false;
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
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID==2);
                var listoderdetai= _orderReposity.Value.GetOrderDetailModels(orderID, 2);
                var listproduct = new List<productsShip>();
                foreach (var item in listoderdetai)
                {
                    productsShip pro = new productsShip
                    {
                        name = item.Sub_ItemName,
                        price = (int)item.PricePurchase,
                        product_code = item.Sub_ItemID,
                        quantity = item.Amount,
                        weight = GetWeightOfSubItem(item.ItemID)/(double)1000,
                    };
                    listproduct.Add(pro);
                }
                var priceOrder= _orderReposity.Value.GetPriceItemOrder(orderID);
                orderrequest shiporder = new orderrequest
                {
                    id=orderID+"",
                    pick_name=order.Pick_Name,
                    pick_address=order.Pick_Address,
                    pick_ward=order.Pick_Ward,
                    pick_district=order.Pick_District,
                    pick_province=order.Pick_Province,
                    pick_street="",
                    pick_tel=order.Pick_Tel,
                    pick_money=0,
                    is_freeship=1,
                    name=order.Name,
                    address=order.Address,
                    district=order.District,
                    province=order.Province,
                    hamlet="Khác",
                    street="",
                    ward=order.Ward,
                    tel=order.Tel,
                    value=(int)priceOrder,
                    transport= "road",
                };
                if (order != null)
                {
                    ShipOrderRequest request = new ShipOrderRequest
                    {
                        order=shiporder,
                        products=listproduct,
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
                        order.Pick_Time=Shipreponse.order.estimated_pick_time;
                        _context.ShipOrders.Add(shipOrder);
                        _context.SaveChanges();
                    }
                    else
                    {
                        ShipOrder shipOrder = new ShipOrder();
                        shipOrder.Status_ID = "-1";
                        DateTime datetime = GetVnTime();
                        shipOrder.Create_Date = datetime;
                        shipOrder.LabelID = orderID+"";
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
            catch(Exception ex)
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
                role.RoleName = role.RoleName+ex.Message;
                _context.SaveChanges();
                return null;
            }
        }

        public ShipStatus GetShipStatus(string statusID)
        {
            return _context.ShipStatuses.SingleOrDefault(ss => ss.Status_ID.Equals(statusID));
        }

        public Result GetShipstatus(int orderID)
        {
            Result result = new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID!=2);
                if(order != null)
                {
                    var liststatus = _context.ShipOrders.AsQueryable();
                    liststatus = liststatus.Where(so => so.OrderID == orderID);
                    liststatus=liststatus.OrderBy(so => so.Create_Date);
                    ShipModel model = new ShipModel();
                    List<ShipStatusModel> list = new List<ShipStatusModel>();
                    if (liststatus.Count() > 0)
                    {
                        foreach(var item in liststatus.ToList())
                        {
                            if(list.Count() == 0)
                            {
                                model.orderID = item.OrderID;
                                model.LabelID = item.LabelID;
                            }
                            ShipStatusModel statusModel = new ShipStatusModel
                            {
                                status=GetShipStatus(item.Status_ID).Status_Name,
                                Create_Date = item.Create_Date,
                                Reason=item.Reason,
                                Reason_code = item.Reason_code
                            };
                            list.Add(statusModel);
                        }
                        model.shipStatusModels = list;
                        result.Success = true;
                        result.Message = "Thành công";
                        result.Data=model;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Đơn hàng không tồn tại, hoặc đơn hàng không tồn tại";
                result.Data = "";
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "lỗi hệ thống";
                result.Data = "";
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
            var ghtkreponde = await client.PostAsync("https://services-staging.ghtklab.com/services/shipment/cancel/partner_id:"+orderID, httpContent);
            var contents = ghtkreponde.Content.ReadFromJsonAsync<ShipReponse>().Result;
            return contents;
        }

        public Result CancelOrder(int orderID)
        {
            Result result=new Result();
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID);
                if (order != null)
                {
                    var reponse = CancelOrderAsync(orderID).Result;
                    result.Success = reponse.success;
                    result.Message = reponse.message;
                    result.Data = "";
                    return result;
                }
                result.Success = false;
                result.Message = "Mã Đơn hàng không tồn tại";
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

        public async Task<Object> GetTicketAsync(string labelID)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            HttpResponseMessage shipResponse = await client.GetAsync("https://services-staging.ghtklab.com/services/label/"+labelID);
            var jsonreponse = await shipResponse.Content.ReadAsStreamAsync();
            return jsonreponse;
        }

        public object GetTicket(int orderID)
        {
            Result result = new Result();
            try
            {
                var orderStatus = _context.ShipOrders.FirstOrDefault(os => os.OrderID == orderID);
                if (orderStatus != null)
                {
                    return GetTicketAsync(orderStatus.LabelID).Result;
                }
                result.Success = false;
                result.Message = "đơn hàng không tồn tại";
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
    }
}

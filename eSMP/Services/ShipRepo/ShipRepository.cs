using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.Json;

namespace eSMP.Services.ShipRepo
{
    //https://khachhang-staging.ghtklab.com
    public class ShipRepository : IShipReposity
    {
        public static string TOKEN = "D1f7d11dFA5D417D35F136A02e0C812EEC613Fcb";
        public static string deliver_option = "none";

        private readonly WebContext _context;
        private readonly Lazy<IOrderReposity> _orderReposity;

        public ShipRepository(WebContext context, Lazy<IOrderReposity> orderReposity)
        {
            _context = context;
            _orderReposity = orderReposity;
        }
        public async Task<FeeReponse> GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight, string deliver_option)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Token", TOKEN);
            HttpResponseMessage shipResponse = await client.GetAsync(string.Format("https://services.giaohangtietkiem.vn/services/shipment/fee?province={0}&district={1}&pick_province={2}&pick_district={3}&weight={4}&deliver_option={5}", province, district, pick_province, pick_district, weight, deliver_option));
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
                var shipdb = _context.ShipOrders.SingleOrDefault(so => so.OrderID == int.Parse(partner_id) && so.Status_ID.Equals(status_id));
                if (shipdb != null)
                {
                    return true;
                }
                ShipOrder shipOrder = new ShipOrder();
                shipOrder.Status_ID = status_id + "";
                DateTime datetime = DateTime.Parse(action_time, null, System.Globalization.DateTimeStyles.RoundtripKind);
                shipOrder.Create_Date = datetime;
                shipOrder.LabelID = label_id;
                shipOrder.Reason= reason;
                shipOrder.OrderID=int.Parse(partner_id);
                shipOrder.Reason_code= reason_code;

                _context.ShipOrders.Add(shipOrder);
                _context.SaveChanges();
                return true;
            }
            catch
            {
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
                };
                if (order != null)
                {
                    ShipOrderRequest request = new ShipOrderRequest
                    {
                        order=shiporder,
                        products=listproduct,
                    };
                    var Shipreponse = CreateOrderAsync(request).Result;

                    ShipOrder shipOrder = new ShipOrder();
                    shipOrder.Status_ID = "-2";
                    DateTime datetime = DateTime.UtcNow;
                    shipOrder.Create_Date = datetime;
                    shipOrder.LabelID = Shipreponse.order.label;
                    shipOrder.Reason = "";
                    shipOrder.OrderID = int.Parse(Shipreponse.order.partner_id);
                    shipOrder.Reason_code = "";

                    _context.ShipOrders.Add(shipOrder);
                    _context.SaveChanges();

                    return Shipreponse;
                }
                return null;
            }
            catch
            {
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
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.OrderStatusID==1);
                if(order != null)
                {
                    var liststatus = _context.ShipOrders.AsQueryable();
                    liststatus = liststatus.Where(so => so.OrderID == orderID);
                    liststatus=liststatus.OrderByDescending(so => so.Create_Date);
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
            FileStreamResult fileStream = new FileStreamResult(jsonreponse, "application/pdf");
            return fileStream;
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

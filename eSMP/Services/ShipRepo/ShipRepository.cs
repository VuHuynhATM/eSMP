using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.VModels;
using System.Text.Json;

namespace eSMP.Services.ShipRepo
{
    //https://khachhang-staging.ghtklab.com
    public class ShipRepository : IShipReposity
    {
        public static string TOKEN = "D1f7d11dFA5D417D35F136A02e0C812EEC613Fcb";
        public static string deliver_option = "none";
        private readonly WebContext _context;
        private readonly IOrderReposity _orderReposity;

        public ShipRepository(WebContext context, IOrderReposity orderReposity)
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

        FeeReponse IShipReposity.GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight)
        {
            return GetFeeAsync(province, district, pick_province, pick_district, weight, deliver_option).Result;
        }

        public bool CallBackAsync(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason)
        {
            try
            {
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
            var ghtkreponde = await client.PostAsync("https://khachhang-staging.ghtklab.com/services/shipment/order/?ver=1.5", httpContent);
            var contents = ghtkreponde.Content.ReadFromJsonAsync<ShipReponse>();
            return contents.Result;
        }
        public int GetWeightOfSubItem(int subItemID)
        {
            var weight = _context.Specification_Values.SingleOrDefault(sv => sv.ItemID == subItemID && sv.SpecificationID == 2).Value;
            return int.Parse(weight);
        }
        void IShipReposity.CreateOrder(int orderID)
        {
            try
            {
                var order = _context.Orders.SingleOrDefault(o => o.OrderID == orderID && o.IsPay);
                var listoderdetai= _orderReposity.GetOrderDetailModels(orderID, true);
                var listproduct = new List<productsShip>();
                foreach (var item in listoderdetai)
                {
                    productsShip pro = new productsShip
                    {
                        name = item.Sub_ItemName,
                        price = (int)item.PricePurchase,
                        product_code = item.Sub_ItemID,
                        quantity = item.Amount,
                        weight = GetWeightOfSubItem(item.Sub_ItemID),
                    };
                    listproduct.Add(pro);
                }
                var priceOrder= _orderReposity.GetPriceItemOrder(orderID);
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
                    tags =new int[1],
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
                   
                }

            }
            catch
            {
                return;
            }
        }
    }
}

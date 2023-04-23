using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Services.ShipRepo
{
    public interface IShipReposity
    {
        public FeeReponse GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight, double price);
        public bool CallBackAsync(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason);
        public ShipReponse CreateOrder(int orderID);
        public Result GetShipstatus(int? orderID, int? serviceID);
        public Result CancelOrder(int orderID);
        public Object GetTicket(int orderID);
        public ShipReponse CreateOrderService(int ServiceID, string type);
    }
}

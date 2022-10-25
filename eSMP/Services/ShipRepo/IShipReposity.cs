using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Services.ShipRepo
{
    public interface IShipReposity
    {
        public FeeReponse GetFeeAsync(string province, string district, string pick_province, string pick_district, int weight);
        public bool CallBackAsync(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason);
        public ShipReponse CreateOrder(int orderID);
        public Result GetShipstatus(int orderID);
        public Result CancelOrder(int orderID);
        public Object GetTicket(int orderID);
    }
}

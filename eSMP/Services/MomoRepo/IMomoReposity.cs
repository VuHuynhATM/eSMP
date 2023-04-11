using eSMP.VModels;

namespace eSMP.Services.MomoRepo
{
    public interface IMomoReposity
    {
        public Result GetPayUrl(int orderID, string paymentMethod);   
        public void PayOrderINPAsync(MomoPayINP payINP);
        public Result CancelOrder(int orderID, string reason);
        public Result ConfimOrder(int orderID);
        public void PayStoreINP(MomoPayINP payINP);
        public Result GetStorePayUrl(int storeID);
        public Result RefundOrder(int orderID, double numrefund);
        public Result ConfimStoreShipOrder(int orderID);
        public Result RefundService(int serviceID, double numrefund);
        public Result InfoPay(int orderID);
    }
}

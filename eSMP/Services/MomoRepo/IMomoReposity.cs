using eSMP.VModels;

namespace eSMP.Services.MomoRepo
{
    public interface IMomoReposity
    {
        public Result GetPayUrl(int orderID);   
        public void PayOrderINP(MomoPayINP payINP);
        public Result CancelOrder(int orderID);
        public Result ConfimOrder(int orderID);
        public Result ConfimCancelOrder(int orderID);
        public void PayStoreINP(MomoPayINP payINP);
        public Result GetStorePayUrl(int storeID);
    }
}

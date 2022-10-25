using eSMP.VModels;

namespace eSMP.Services.MomoRepo
{
    public interface IMomoReposity
    {
        public Result GetPayUrl(int orderID);   
        public void PayOrderINP(MomoPayINP payINP);
        public Result CancelOrder(int orderID);
    }
}

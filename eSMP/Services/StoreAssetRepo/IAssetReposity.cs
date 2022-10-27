using eSMP.VModels;

namespace eSMP.Services.StoreAssetRepo
{
    public interface IAssetReposity
    {
        public bool SendPriceTostore(int OrderID);
        public Result SystemWithdrawal(SystemWithdrawalM request);
        public Result GetALlSystemWitdrawl(int? page, DateTime? From, DateTime? To);
        public Result GetALlReveneu(int? page, DateTime? From, DateTime? To);
        public Result GetSystemInfo();
    }
}

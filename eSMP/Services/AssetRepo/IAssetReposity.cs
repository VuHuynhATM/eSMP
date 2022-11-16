using eSMP.VModels;

namespace eSMP.Services.StoreAssetRepo
{
    public interface IAssetReposity
    {
        public bool SendPriceTostore(int OrderID);
        public Result SystemWithdrawal(SystemWithdrawalM request);
        public Result GetALlSystemWitdrawl(int? page, DateTime? From, DateTime? To);
        public Result GetALlReveneu(int? page, DateTime? From, DateTime? To, int? orderID);
        public Result GetSystemInfo();
        public Result GetStoreReveneu(int storeID, int? page, DateTime? From, DateTime? To, int? orderID);
        public Result CreateStoreWithdrawal(StoreWithdrawalRequest request);
        public Result ProcessStoreWithdrawal(int storeWithhdrawalID);
        public Result CancelStoreWithdrawal(int storeWithhdrawalID, string reason);
        public Result SuccessStoreWithdrawal(StoreWithdrawalSuccessRequest request);
        public Result GetStoreWithdrawal(int? storeID, int?page, int? statusID);
        public Result GetBankSupport();
        public Result GetStoreReveneuForChart(int storeID,int? year);
        public Result GetSystemReveneuForChart(int? year, string Cate);
        public Result GetStoreSystemReveneu(int? page, DateTime? From, DateTime? To);
    }
}

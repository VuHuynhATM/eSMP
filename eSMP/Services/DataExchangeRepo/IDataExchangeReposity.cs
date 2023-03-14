using eSMP.VModels;

namespace eSMP.Services.DataExchangeRepo
{
    public interface IDataExchangeReposity
    {
        public Result GetStoreDataExchanges(int? storeID, int? orderID, DateTime? from, DateTime? to, int? page);
        public Result FinishStoreDataExchange(DataExchangeStoreFinish DataExchange);
    }
}

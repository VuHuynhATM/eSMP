using eSMP.VModels;

namespace eSMP.Services.DataExchangeRepo
{
    public interface IDataExchangeReposity
    {
        public Result GetStoreDataExchanges(int? storeID, int? orderID, int? serviceID, DateTime? from, DateTime? to, int? page);
        public Result FinishStoreDataExchange(DataExchangeStoreFinish DataExchange);

        public Result GetUserDataExchanges(int? userID, int? orderID, int? serviceID, DateTime? from, DateTime? to, int? page);
        public Result FinishUserDataExchange(DataExchangeUserFinish DataExchange);
        public Result AddCardUserDataExchange(DataExchangeUserAddCard DataExchange);
    }
}

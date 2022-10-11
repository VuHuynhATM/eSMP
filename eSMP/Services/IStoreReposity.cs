using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services
{
    public interface IStoreReposity
    {
        public Result GetAllStore();
        public Result CteateStore(StoreRegister store);
        public Result StoreDetail(int storeID);
        public Result StoreUpdateInfo(StoreUpdateInfo info);
        public StoreViewModel GetStoreModel(int storeID);
        public Address GetAddressByStoreID(int StoreID);
        public Result ActiveStore(int storeID);
        public Result HiddenStore(int storeID);
        public Result BlockStore(int storeID);
        public Result UnHiddenStore(int storeID);
    }
}

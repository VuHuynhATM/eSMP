using eSMP.VModels;

namespace eSMP.Services
{
    public interface IItemReposity
    {
        public Result GetAllItem();
        public Result GetItemDetail(int itemID);
        public Result CreateItem(ItemRegister item);
        public Result RemoveItem(int itemID);
    }
}

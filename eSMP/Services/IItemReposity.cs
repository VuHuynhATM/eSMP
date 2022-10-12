using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services
{
    public interface IItemReposity
    {
        public Result GetAllItem(int? statusID, int page);
        public Result GetItemDetail(int itemID);
        public Result CreateItem(ItemRegister item);
        public Result RemoveItem(int itemID);
        public Result SearchItem(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? la, double? lo, int? storeID, int page);
        public Result UpdatesubItem(SubItemUpdate subItem);
        public Result AddsubItem(int itemID, Sub_ItemRegister subItem);
        public Result ActivesubItem(int subitemID);
        public Result ActiveItem(int itemID);
        public Result BlocksubItem(int subitemID);
        public Result BlockItem(int itemID);
        public Result HiddensubItem(int subitemID);
        public Result HiddenItem(int itemID);
        public Result UnHiddensubItem(int subitemID);
        public Result UnHiddenItem(int itemID);
        public Result SearchItemForSupplier(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? la, double? lo, int? storeID, int page);
    }
}

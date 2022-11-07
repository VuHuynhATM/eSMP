﻿using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.ItemRepo
{
    public interface IItemReposity
    {
        public Result GetItemWithStatusID(int? statusID, int? page);
        public Result GetItemWithStatusIDS(int storeID, int? statusID, int? page);
        public Result GetItemDetail(int itemID);
        public Result CreateItem(ItemRegister item);
        public Result RemoveItem(int itemID);
        public Result SearchItem(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? la, double? lo, int? storeID, int? page);
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
        public Result SearchItemForAdmin(string? search, double? min, double? max, double? rate, int? cateID, int? subCateID, int? brandID, int? brandModelID, string? sortBy, double? lat, double? lot, int? storeID, int? page, bool? isSupplier);
        public Result UpdateBrandModel(int itemID, int[] brandmodelIDs);
        public Result RemoveBrandModel(int itemID, int[] brandmodelIDs);
        public Result UpdateDiscount(int itemID, double dícsount);
        public Result GetListFeedback(int itemID, int? page);
    }
}

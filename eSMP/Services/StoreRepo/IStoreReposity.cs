﻿using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.StoreRepo
{
    public interface IStoreReposity
    {
        public Result GetAllStore(string? search);
        public Result CteateStore(StoreRegister store);
        public Result StoreDetail(int storeID);
        public Result GetStorebyuserID(int userID);
        public Result StoreUpdateInfo(StoreUpdateInfo info);
        public StoreViewModel GetStoreModel(int storeID);
        public Address GetAddressByStoreID(int StoreID);
        public Result ActiveStore(int storeID);
        public Result HiddenStore(int storeID);
        public Result BlockStore(int storeID);
        public Result UnHiddenStore(int storeID);
        public Result UpdateAddress(int storeID, Address address);
        public Boolean CheckStore(string firebaseID);
    }
}

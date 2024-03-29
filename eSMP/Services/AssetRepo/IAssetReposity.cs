﻿using eSMP.VModels;

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
        public Result GetStoreWithdrawal(int? storeID, int?page, int? statusID, DateTime? from, DateTime? to);
        //public Result GetBankSupport();
        public Result GetStoreReveneuForChart(int storeID,int? year);
        public Result GetSystemReveneuForChart(int? year);
        public Result GetStoreSystemReveneu(int? page, DateTime? From, DateTime? To);
        public Result UpdateCommission_Precent(double commission_Precent);
        public Result UpdateAmountActive(double amount_Active);
        public Result UpdateRefund_Precent(double refund_Precent);
        public Result UpdateCo_Examination(Boolean Co_Examination);
    }
}

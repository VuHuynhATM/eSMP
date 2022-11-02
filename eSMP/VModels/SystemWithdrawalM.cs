using eSMP.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class SystemWithdrawalM
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Context { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class SystemWithdrawalView
    {
        public int System_WithdrawalID { get; set; }
        public Image Image { get; set; }
        public string Context { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class OrderSystemTransactionView
    {
        public int OrderSystem_TransactionID { get; set; }
        public int OrderStore_TransactionID { get; set; }
        public int orderID { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class StoreSystemReveneuView
    {
        public long MomoTransaction { get; set; }
        public int StoreID { get; set; }
        public DateTime ActiveDate { get; set; }
        public double Amount { get; set; }
    }

    public class OrderStore_TransactionModel
    {
        public int OrderStore_TransactionID { get; set; }
        public int OrderID { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
    public class StoreWithdrawalRequest
    {
        public int StoreID { get; set; }
        public double Price { get; set; }
        public string NumBankCart { get; set; }
        public string OwnerBankCart { get; set; }
        public int BankID { get; set; }

    }
    public class StoreWithdrawalSuccessRequest
    {
        public int Store_WithdrawalID { get; set; }
        public string Filename { get; set; }
        public string Path { get; set; }

    }
    public class Store_WithdrawalModel
    {
        public int Store_WithdrawalID { get; set; }
        public int StoreID { get; set; }
        public Image? Image { get; set; }
        public string NumBankCart { get; set; }
        public string OwnerBankCart { get; set; }
        public BankSupport Bank { get; set; }
        public DateTime Create_Date { get; set; }
        public string Reason { get; set; }
        public double Price { get; set; }
        public Withdrawal_Status Withdrawal_Status { get; set; }
    }
}

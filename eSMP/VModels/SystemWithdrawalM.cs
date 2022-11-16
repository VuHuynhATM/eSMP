using eSMP.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class SystemWithdrawalM
    {
        public IFormFile File { get; set; }
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
        public int StoreID { get; set; }
        public OrderStore_TransactionModel OrderStore_TransactionModel { get; set; }
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
        public long MomoTransaction { get; set; }
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
        public IFormFile File { get; set; }

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

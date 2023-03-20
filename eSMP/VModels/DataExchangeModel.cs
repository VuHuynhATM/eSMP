using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class DataExchangeStoreModel
    {
        public int ExchangeStoreID { get; set; }
        public string ExchangeStoreName { get; set; }
        public double ExchangePrice { get; set; }
        public DateTime Create_date { get; set; }
        public Status ExchangeStatus { get; set; }
        public Image? Image { get; set; }
        public int? OrderID { get; set; }
        public int? AfterBuyServiceID { get; set; }
    }
    public class DataExchangeStoreFinish
    {
        public int ExchangeStoreID { get; set; }
        public IFormFile File { get; set; }
    }
    public class DataExchangeUserModel
    {
        public int ExchangeUserID { get; set; }
        public string ExchangeUserName { get; set; }
        public double ExchangePrice { get; set; }
        public DateTime Create_date { get; set; }
        public Status ExchangeStatus { get; set; }
        public Image? Image { get; set; }
        public int? OrderID { get; set; }
        public string? BankName { get; set; }
        public string? CardNum { get; set; }
        public string? CardHoldName { get; set; }
        public int? AfterBuyServiceID { get; set; }//FK
    }
    public class DataExchangeUserFinish
    {
        public int ExchangeUserID { get; set; }
        public IFormFile File { get; set; }
    }
    public class DataExchangeUserAddCard
    {
        public int ExchangeUserID { get; set; }
        public string BankName { get; set; }
        public string CardNum { get; set; }
        public string CardHoldName { get; set; }
    }
}

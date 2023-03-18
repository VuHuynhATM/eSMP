using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("DataExchangeUser")]
    public class DataExchangeUser
    {
        [Key]
        public int ExchangeUserID { get; set; }
        public string ExchangeUserName { get; set; }
        public double ExchangePrice { get; set; }
        public DateTime Create_date { get; set; }
        public int ExchangeStatusID { get; set; }
        public string? BankName { get; set; }
        public string? CardNum { get; set; }
        public string? CardHoldName { get; set; }
        public int? ImageID { get; set; }//Fk
        [ForeignKey("ImageID")]
        public virtual Image? Image { get; set; }
        public int? OrderID { get; set; }//fk
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
        public int? AfterBuyServiceID { get; set; }//FK
        [ForeignKey("AfterBuyServiceID")]
        public virtual AfterBuyService? AfterBuyService { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("DataExchangeStore")]
    public class DataExchangeStore
    {
        [Key]
        public int ExchangeStoreID { get; set; }
        public string ExchangeStoreName { get; set; }
        public double ExchangePrice { get; set; }
        public DateTime Create_date { get; set; }
        public int ExchangeStatusID { get; set; }
        public int? ImageID { get; set; }//Fk
        [ForeignKey("ImageID")]
        public virtual Image? Image { get; set; }
        public int? OrderID { get; set; }//fk
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }

    }
}

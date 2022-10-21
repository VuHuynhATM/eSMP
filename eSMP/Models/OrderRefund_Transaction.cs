using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace eSMP.Models
{
    [Table("OrderRefund_Transaction")]
    public class OrderRefund_Transaction
    {
        [Key]
        public int ID { get; set; }
        public DateTime Create_Date { get; set; }
        public int ResultCode { get; set; }
        public long MomoTransactionID { get; set; }
        public int OrderID { get; set; }//FK
        [ForeignKey("OrderID")]
        public Order Order { get; set; }
    }
}

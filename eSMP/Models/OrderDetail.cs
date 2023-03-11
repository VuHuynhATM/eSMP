using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        public double PricePurchase { get; set; }
        public double DiscountPurchase { get; set; }
        public int WarrantiesTime { get; set; }
        public int Amount { get; set; }
        public string? Feedback_Title { get; set; }
        public double? Feedback_Rate { get; set; }
        public DateTime? FeedBack_Date { get; set; }
        public int Sub_ItemID { get; set; }//FK
        [ForeignKey("Sub_ItemID")]
        public virtual Sub_Item Sub_Item { get; set; }
        public int OrderID { get; set; }//Fk
        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }
        public int? Feedback_StatusID { get; set; }
    }
}

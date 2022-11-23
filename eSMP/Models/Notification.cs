using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }
        public DateTime Create_Date { get; set; }
        public string Title { get; set; }
        public int? OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
        public int? StoreID { get; set; }
        [ForeignKey("StoreID")]
        public virtual Store? Store { get; set; }
        public int? ItemID { get; set; }
        [ForeignKey("ItemID")]
        public virtual Item? Item { get; set; }
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; } 
        public Boolean IsActive { get; set; }
    }
}

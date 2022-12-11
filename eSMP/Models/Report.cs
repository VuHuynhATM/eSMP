using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Report")]
    public class Report
    {
        [Key]
        public int ReportID { get; set; }
        public string Text { get; set; }
        public DateTime Create_Date { get; set; }
        public int ReportStatusID { get; set; }
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int? ItemID { get; set; }//Fk
        [ForeignKey("ItemID")]
        public virtual Item? Item { get; set; }
        public int? StoreID { get; set; }//Fk
        [ForeignKey("StoreID")]
        public virtual Store? Store { get; set; }
        public int? OrderDetaiID { get; set; }//Fk
        [ForeignKey("OrderDetaiID")]
        public virtual OrderDetail? OrderDetail { get; set; }
    }
}

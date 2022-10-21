using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("ShipOrder")]
    public class ShipOrder
    {
        [Key]
        public int ShipOrderID { get; set; }
        public int OrderID { get; set; }//Fk
        [ForeignKey("OrderID")]
        public Order Order { get; set; }
        public string Status_ID { get; set; }//FK
        [ForeignKey("Status_ID")]
        public ShipStatus ShipStatus { get; set; }
        public string Reason_code { get; set; }
        public string Reason { get; set; }
        public string LabelID { get; set; }
        public DateTime Create_Date { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("ServiceDetail")]
    public class ServiceDetail
    {
        [Key]
        public int ServiceDetailID { get; set; }
        public int OrderDetailID { get; set; }//FK
        [ForeignKey("OrderDetailID")]
        public virtual OrderDetail OrderDetail { get; set; }
        public int Amount { get; set; }
        public int AfterBuyServiceID { get; set; }//FK
        [ForeignKey("AfterBuyServiceID")]
        public virtual AfterBuyService AfterBuyService { get; set; }
    }
}

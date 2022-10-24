using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("OrderStatus")]
    public class OrderStatus
    {
        [Key]
        public int OrderStatusID { get; set; }
        public string StausName { get; set; }
        public Boolean IsActive { get; set; }
    }
}

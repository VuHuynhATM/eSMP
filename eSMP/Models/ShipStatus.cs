using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("ShipStatus")]
    public class ShipStatus
    {
        [Key]
        public string Status_ID { get; set; }
        public string Status_Name { get; set; }
    }
}

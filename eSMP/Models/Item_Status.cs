using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("ItemStatus")]
    public class Item_Status
    {
        [Key]
        public int Item_StatusID { get; set; }
        public string StatusName { get; set; }
        public Boolean IsActive { get; set; }
    }
}

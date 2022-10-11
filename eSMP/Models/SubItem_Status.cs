using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("SubItem_Status")]
    public class SubItem_Status
    {
        [Key]
        public int SubItem_StatusID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}

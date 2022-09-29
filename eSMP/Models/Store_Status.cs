using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Store_Status")]
    public class Store_Status
    {
        [Key]
        public int Store_StatusID { get; set; }
        public string StatusName { get; set; }
        public Boolean IsActive { get; set; }

    }
}

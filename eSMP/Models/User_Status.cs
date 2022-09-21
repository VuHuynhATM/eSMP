using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("User_Status")]
    public class User_Status
    {
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }
        public Boolean IsActive { get; set; }
    }
}

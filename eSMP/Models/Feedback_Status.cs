using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Feedback_Status")]
    public class Feedback_Status
    {
        [Key]
        public int Feedback_StatusID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}

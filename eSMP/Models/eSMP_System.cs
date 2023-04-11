using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("eSMP_System")]
    public class eSMP_System
    {
        [Key]
        public int SystemID { get; set; }
        public double Commission_Precent { get; set; }
        public double Asset { get; set; }
        public double AmountActive { get; set; }
        public double Refund_Precent { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean Co_Examination { get; set; }
    }
}

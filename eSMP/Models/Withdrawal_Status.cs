using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("Withdrawal_Status")]
    public class Withdrawal_Status
    {
        [Key]
        public int Withdrawal_StatusID { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
    }
}

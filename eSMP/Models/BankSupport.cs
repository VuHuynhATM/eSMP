using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("BankSupport")]
    public class BankSupport
    {
        [Key]
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BankAvatar { get; set; }
        public Boolean IsActive { get; set; }
    }
}

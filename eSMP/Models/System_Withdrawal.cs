using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("System_Withdrawal")]
    public class System_Withdrawal
    {
        [Key]
        public int System_WithdrawalID { get; set; }
        public int SystemID { get; set; }//fk
        [ForeignKey("SystemID")]
        public virtual eSMP_System eSMP_System { get; set; }
        public int ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public virtual Image Image { get; set; }
        public string Context { get; set; }
        public DateTime Create_Date { get; set; }
        public double Price { get; set; }
        public Boolean IsActive { get; set; }
    }
}

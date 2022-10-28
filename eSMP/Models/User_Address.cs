using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("User_Address")]
    public class User_Address
    {
        [Key]
        public int User_AddressID { get; set; }
        public int UserID { get; set; }//FK
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int AddressID { get; set; }//FK
        [ForeignKey("AddressID")]
        public virtual Address Address { get; set; }
        public Boolean IsActive { get; set; }
    }
}

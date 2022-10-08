using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace eSMP.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; }
        public DateTime Crete_date { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Token { get; set; }
        public Boolean isActive { get; set; } 
        public int RoleID { get; set; }//FK
        [ForeignKey("RoleID")]
        public Role Role { get; set; }
        public int ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public Image Image { get; set; }


    }
}

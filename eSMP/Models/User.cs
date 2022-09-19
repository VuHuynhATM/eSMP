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
        public Boolean IsActive { get; set; }
        public string Token { get; set; }
        public int RoleID { get; set; }//FK
        [ForeignKey("RoleID")]
        public Role Role { get; set; }
        public int? ImageID { get; set; }//FK
        [ForeignKey("ImageID")]
        public Image Image { get; set; }
        public int? AddressID { get; set; }//FK
        [ForeignKey("AddressID")]
        public Address Address { get; set; }


    }
}

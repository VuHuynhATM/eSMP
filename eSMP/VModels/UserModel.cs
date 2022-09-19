using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; }
        public Boolean IsActive { get; set; }
        public string? Token { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }

        public int AddressesID { get; set; }
    }
    public class UserLogin
    {
        public string Phone { get; set; }
    }
}

using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public Boolean IsActive { get; set; }
        public string Token { get; set; }
        public int RoleID { get; set; }//FK
        public int ImageID { get; set; }//FK
    }
}

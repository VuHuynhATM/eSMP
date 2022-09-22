using eSMP.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }

    }
    public class UserLogin
    {
        public string Phone { get; set; }
    }
    public class AdminLogin
    {
        public string Email { get; set; }
        public string Pasword { get; set; }
    }

    public class UserRegister
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImageName { get; set; }
        public string Imagepath { get; set; }
        public string contextAddress { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}

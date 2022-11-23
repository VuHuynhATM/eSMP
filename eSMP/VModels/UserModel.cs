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
        public string? Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime Crete_date { get; set; }
        public Boolean IsActive { get; set; }
        public string Token { get; set; }
        public Role Role { get; set; }
        public Image Image { get; set; }
        public int? StoreID { get; set; }
        public List<Address> addresses { get; set; }
        public string? FirebaseID { get; set; }
        public string? FCM_Firebase { get; set; }

    }
    public class UserLogin
    {
        public string Phone { get; set; }
        public string? FCM_Firebase { get; set; }
    }
    public class AdminLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FCM_Firebase { get; set; }
    }
    public class UserRegister
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImageName { get; set; }
        public string Imagepath { get; set; }
        public string contextAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string? FirebaseID { get; set; }
        public string? FCM_Firebase { get; set; }
    }
    public class UserAddAddress
    {
        public int UserID { get; set; }
        public string UserName { get; set;}
        public string Phone { set; get; }
        public string contextAddress { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class EditName
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
    public class EditEmail
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
    }
    public class EditGender
    {
        public int UserID { get; set; }
        public string UserGender { get; set; }
    }
    public class EditBirth
    {
        public int UserID { get; set; }
        public DateTime UserBirth { get; set; }
    }
    public class EditImage
    {
        public int UserID { get; set; }
        public IFormFile File { get; set; }
    }
}

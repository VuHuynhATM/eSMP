using eSMP.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class StoreModel
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public DateTime Create_date { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Pick_date { get; set; }
        public int UserID { get; set; }
        public Address Address { get; set; }
        public Status Store_Status { get; set; }
        public Image Image { get; set; }
        public double Asset { get; set; }
        public long? MomoTransactionID { get; set; }
        public DateTime? Actice_Date { get; set; }
        public double? Actice_Amount { get; set; }
        public string? FirebaseID { get; set; }
        public string? FCM_Firebase { get; set; }
        public int TotalActiveItem { get; set; }
        public int TotalBlockItem { get; set; }
        public int TotalWatingItem { get; set; }
        public int TotalOrder { get; set; }
        public int TotalCancelOrder { get; set; }
        public double TotalRating { get; set; }
    }
    public class StoreViewModel
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string Imagepath { get; set; }
        public string? FirebaseID { get; set; }
        public string? FCM_Firebase { get; set; }
    }
    public class StoreRegister
    {
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int UserID { get; set; }
        public IFormFile File { get; set; }
        public int Pick_date { get; set; }
        public string contextAddress { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class StoreUpdateInfo
    {
        public string StoreName { get; set; }
        public int StoreID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Pick_date { get; set; }
    }
}

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
        public Store_Status Store_Status { get; set; }
        public Image Image { get; set; }
        public double Asset { get; set; }
    }
    public class StoreViewModel
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string Imagepath { get; set; }
    }
    public class StoreRegister
    {
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int UserID { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
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

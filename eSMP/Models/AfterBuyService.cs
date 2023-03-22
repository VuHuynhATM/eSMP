using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.Models
{
    [Table("AfterBuyService")]
    public class AfterBuyService
    {
        [Key]
        public int AfterBuyServiceID { get; set; }
        public DateTime Create_Date { get; set; }
        public double? FeeShipFisrt { get; set; }
        public double? FeeShipSercond { get; set; }
        public string User_Province { get; set; }
        public string User_District { get; set; }
        public string User_Ward { get; set; }
        public string User_Address { get; set; }
        public string User_Tel { get; set; }
        public string User_Name { get; set; }
        public string Store_Name { get; set; }
        public string Store_Tel { get; set; }
        public string Store_Province { get; set; }
        public string Store_District { get; set; }
        public string Store_Ward { get; set; }
        public string Store_Address { get; set; }
        public string? Reason { get; set; }
        public double? RefundPrice { get; set; }
        public int ServicestatusID { get; set; }
        public int ServiceType { get; set; }
        public string? estimated_pick_time { get; set; }
        public string? estimated_deliver_time { get; set; }
        public string PackingLinkCus { get; set; }
        public string Text { get; set; }
    }
}

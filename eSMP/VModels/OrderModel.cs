using eSMP.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class OrderModel
    {
        public int OrderID { get; set; }
        public DateTime Create_Date { get; set; }
        public Boolean IsPay { get; set; }
        public int UserID { get; set; }
        public double FeeShip { get; set; }
        public Address Address { get; set; }
        public List<OrderDetailModel> Details { get; set; }
    }
    public class OrderDetailModel
    {
        public int OrderDetailID { get; set; }
        public double PricePurchase { get; set; }
        public double DiscountPurchase { get; set; }
        public int Amount { get; set; }
        public string? Feedback_Title { get; set; }
        public double? Feedback_Rate { get; set; }
        public DateTime? FeedBack_Date { get; set; }
        public int Sub_ItemID { get; set; }
        public string Sub_ItemName { get; set; }
        public string sub_ItemImage { get; set; }
        public int ItemID { get; set; }
        public Feedback_Status? Feedback_Status { get; set; }
    }
    public class OrderDetailAdd
    {
        public int UserID { get; set; }
        public int Amount { get; set; }
        public int Sub_ItemID { get; set; }
    }
}

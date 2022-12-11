using eSMP.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSMP.VModels
{
    public class OrderModel
    {
        public int OrderID { get; set; }
        public StoreViewModel StoreView { get; set; }
        public DateTime Create_Date { get; set; }
        public Status OrderStatus { get; set; }
        public int UserID { get; set; }
        public double PriceItem { get; set; }
        public double FeeShip { get; set; }
        public string Pick_Province { get; set; }
        public string Pick_District { get; set; }
        public string Pick_Ward { get; set; }
        public string Pick_Address { get; set; }
        public string Pick_Tel { get; set; }
        public string Pick_Name { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public List<OrderDetailModel> Details { get; set; }
        public string? Reason { get; set; }
        public string? Pick_Time { get; set; }
        public string? ShipOrderID { get; set; }
        public string? FirebaseID { get; set; }
    }
    public class OrderModelView
    {
        public int OrderID { get; set; }
        public StoreViewModel StoreView { get; set; }
        public DateTime Create_Date { get; set; }
        public Status OrderStatus { get; set; }
        public int UserID { get; set; }
        public double PriceItem { get; set; }
        public double FeeShip { get; set; }
        public string Pick_Province { get; set; }
        public string Pick_District { get; set; }
        public string Pick_Ward { get; set; }
        public string Pick_Address { get; set; }
        public string Pick_Tel { get; set; }
        public string Pick_Name { get; set; }
        public string Name { get; set; }
        public string Tel { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public List<OrderDetailModel> Details { get; set; }
        public ShipViewModel OrderShip{ get; set; }
        public string? Reason { get; set; }
        public string? Pick_Time { get; set; }
        public string? FirebaseID { get; set; }

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
        public Status? Feedback_Status { get; set; }
        public List<Image>? ListImageFb { get; set; } 

    }
    public class OrderDetailAdd
    {
        public int UserID { get; set; }
        public int Amount { get; set; }
        public int Sub_ItemID { get; set; }
    }
    public class FeedBackOrderDetail
    {
        public int OrderDetaiID { get; set; }
        public int Rate { get; set; }
        public string? Text { get; set; }
        public IFormFileCollection? feedbackImages { get; set; }
    }
}

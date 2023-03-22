using eSMP.Models;

namespace eSMP.VModels
{
    public class AfterBuyServiceModel
    {
        public DateTime Create_Date { get; set; }
        public string PackingLinkCus { get; set; }
        public string List_ServiceDetail { get; set; }
        public int OrderID { get; set; }
        public int AddressID { get; set; }
        public int ServiceType { get; set; }
        public string Text { get; set; }

    }
    public class ServiceDetaiAdd
    {
        public int DetailID { get; set; }
        public int Amount { get; set; }
    }
    public class CancelService
    {
        public int serviceID { get; set; }
        public string reason { get; set; }
    }
    public class AfterBuyServiceModelView
    {
        public int AfterBuyServiceID { get; set; }
        public int OrderID { get; set; }
        public StoreViewModel StoreView { get; set; }
        public DateTime Create_Date { get; set; }
        public Status Servicestatus { get; set; }
        public Status ServiceType { get; set; }
        public int UserID { get; set; }
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
        public List<AfterBuyServiceDetailModel> Details { get; set; }
        public ShipViewModel OrderShip { get; set; }
        public string? Reason { get; set; }
        public string? Pick_Time { get; set; }
        public string? Deliver_time { get; set; }
        public string? FirebaseID { get; set; }
        public double? RefundPrice { get; set; }
        public bool HasStoreDataExchange { get; set; }
        public bool HasUserDataExchange { get; set; }
        public string? PackingLink { get; set; }
        public string? PackingLinkCus { get; set; }
        public string Text { get; set; }

    }
    public class AfterBuyServiceDetailModel
    {
        public int AfterBuyServiceDetailID { get; set; }
        public double PricePurchase { get; set; }
        public double DiscountPurchase { get; set; }
        public int Amount { get; set; }
        public int Sub_ItemID { get; set; }
        public string Sub_ItemName { get; set; }
        public string sub_ItemImage { get; set; }
        public int ItemID { get; set; }
    }
}

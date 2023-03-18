namespace eSMP.VModels
{
    public class AfterBuyServiceModel
    {
        public DateTime Create_Date { get; set; }
        public IFormFileCollection List_Image { get; set; }
        public string List_ServiceDetail { get; set; }
        public int OrderID { get; set; }
        public int AddressID { get; set; }
        public int ServiceType { get; set; }
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
}

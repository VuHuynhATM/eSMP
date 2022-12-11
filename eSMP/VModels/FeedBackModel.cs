using eSMP.Models;

namespace eSMP.VModels
{
    public class FeedBackModel
    {
        public string UserName { get; set; }
        public int UserID { get; set; }
        public int orderDetaiID { get; set; }
        public string UserAvatar { get; set; }
        public string Sub_itemName { get; set; }
        public List<Image>? ImagesFB { get; set; }
        public double? Rate { get; set; }
        public string? Comment { get; set; }
        public DateTime? Create_Date { get; set; }
    }
    public class FeedbackViewModel
    {
        public int orderDetaiID { get; set; }
        public string Sub_itemName { get; set; }
        public string subItemImage { get; set; }
        public double? Rate { get; set; }
        public string? Comment { get; set; }
        public Status? Feedback_Status { get; set; }
        public DateTime? Create_Date { get; set; }
        public DateTime? Delivery_Date { get; set; }
        public List<Image>? ImagesFB { get; set; }
    }
    public class FeedbackDetailModel
    {
        public string UserName { get; set; }
        public string Useravatar { get; set; }
        public int UserID { get; set; }
        public int orderDetaiID { get; set; }
        public string Sub_itemName { get; set; }
        public string subItemImage { get; set; }
        public double? Rate { get; set; }
        public string? Comment { get; set; }
        public Status? Feedback_Status { get; set; }
        public DateTime? Create_Date { get; set; }
        public List<Image>? ImagesFB { get; set; }
    }
}

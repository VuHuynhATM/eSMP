using eSMP.Models;

namespace eSMP.VModels
{
    public class ReportStoreRequest
    {
        public int StoreID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
    }
    public class ReportItemRequest
    {
        public int ItemID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
    }
    public class ReportFeedbackRequest
    {
        public int OrderDetailID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
    }
    public class FeedbackReportModel
    {
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
        public int orderDetaiID { get; set; }
        public List<Image>? ImagesFB { get; set; }
        public double? Rate { get; set; }
        public string? Comment { get; set; }
        public DateTime Create_Date { get; set; }
        public Status ReportStatus { get; set; }
    }
    public class ItemReportModel
    {
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
        public Image ItemImage { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public Status ReportStatus { get; set; }
        public DateTime Create_Date { get; set; }
    }
    public class StoreReportModel
    {
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public string Text { get; set; }
        public Image StoreImage { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public Status ReportStatus { get; set; }
        public DateTime Create_Date { get; set; }
    }
}

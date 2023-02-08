using eSMP.VModels;

namespace eSMP.Services.ReportRepo
{
    public interface IReportReposity
    {
        public Result ReportStore(ReportStoreRequest request);
        public Result ReportItem(ReportItemRequest request);
        public Result ReportFeedback(ReportFeedbackRequest request);
        public Result GetListReport(int? page, int reportType, int? reportStatusID, int? storeID, int? userID);
    }
}

using eSMP.VModels;

namespace eSMP.Services.ReportSaleRepo
{
    public interface ISaleReportReposity
    {
        public Result GetListHotItem(int? storeID, int? month, int? year, bool hot);
    }
}

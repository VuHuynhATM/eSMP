using eSMP.Models;
using eSMP.Services.StatusRepo;
using eSMP.VModels;
using System.Collections;

namespace eSMP.Services.ReportRepo
{
    public class ReportRepository : IReportReposity
    {
        private readonly WebContext _context;
        private readonly IStatusReposity _statusReposity;
        public static int PAGE_SIZE { get; set; } = 25;

        public ReportRepository(WebContext context, IStatusReposity statusReposity)
        {
            _context = context;
            _statusReposity = statusReposity;
        }

        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }

        public Result ReportFeedback(ReportFeedbackRequest request)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == request.UserID);
                var orderDetail = _context.OrderDetails.SingleOrDefault(od => od.OrderDetailID == request.OrderDetailID);
                var reportc = _context.Reports.SingleOrDefault(r => r.UserID == request.UserID && r.OrderDetaiID == request.OrderDetailID);
                if (reportc != null)
                {
                    result.Success = false;
                    result.Message = "Bạn đã báo cáo đánh giá này rồi";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                if (orderDetail != null && user != null)
                {
                    Report report = new Report
                    {
                        Create_Date = GetVnTime(),
                        ReportStatusID = 3,
                        OrderDetaiID = orderDetail.OrderDetailID,
                        Text = request.Text,
                        UserID = request.UserID,
                    };
                    _context.Reports.Add(report);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Báo cáo đánh giá thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Bình luận hoặc người báo cáo không tồn tại";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result ReportItem(ReportItemRequest request)
        {
            Result result = new Result();
            int numpage = 1;

            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == request.UserID);
                var item = _context.Items.SingleOrDefault(i => i.ItemID == request.ItemID);
                var reportc = _context.Reports.SingleOrDefault(r => r.UserID == request.UserID && r.ItemID == request.ItemID);
                if (reportc != null)
                {
                    result.Success = false;
                    result.Message = "Bạn đã báo cáo sản phẩm này rồi";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                if (item != null && user != null)
                {
                    Report report = new Report
                    {
                        Create_Date = GetVnTime(),
                        ReportStatusID = 3,
                        ItemID = item.ItemID,
                        Text = request.Text,
                        UserID = request.UserID,
                    };
                    _context.Reports.Add(report);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Báo cáo sản phẩm thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Sản phẩm hoặc người báo cáo không tồn tại";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result ReportStore(ReportStoreRequest request)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == request.UserID);
                var store = _context.Stores.SingleOrDefault(s => s.StoreID == request.StoreID);
                var reportc = _context.Reports.SingleOrDefault(r => r.UserID == request.UserID && r.StoreID == request.StoreID);
                if (reportc != null)
                {
                    result.Success = false;
                    result.Message = "Bạn đã báo cáo gian hàng này rồi";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                if (store != null && user != null)
                {
                    Report report = new Report
                    {
                        Create_Date = GetVnTime(),
                        ReportStatusID = 3,
                        StoreID = store.StoreID,
                        Text = request.Text,
                        UserID = request.UserID,
                    };
                    _context.Reports.Add(report);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Báo cáo gian hàng thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Gian hàng hoặc người báo cáo không tồn tại";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result GetListReport(int? page, int reportType, int? reportStatusID, int? storeID, int? userID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listReport = _context.Reports.AsQueryable();
                if (reportStatusID.HasValue)
                {
                    listReport = listReport.Where(r => r.ReportStatusID == reportStatusID);
                }
                //type:1: feedback
                //type:2: item
                //type:3: store
                if (reportType == 1)
                {
                    listReport = listReport.Where(r => r.OrderDetaiID != null);
                    if (userID.HasValue)
                    {
                        listReport = listReport.Where(r => r.UserID == userID);
                    }
                    if (storeID.HasValue)
                    {
                        listReport = listReport.Where(r => _context.Items.SingleOrDefault(i => i.ItemID == r.OrderDetail.Sub_Item.ItemID && i.StoreID == storeID) != null);
                    }
                    if (page.HasValue)
                    {
                        numpage = (int)Math.Ceiling((double)listReport.Count() / (double)PAGE_SIZE);
                        if (numpage == 0)
                        {
                            numpage = 1;
                        }
                        listReport = listReport.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                    }
                    List<FeedbackReportModel> list = new List<FeedbackReportModel>();
                    foreach (var report in listReport.ToList())
                    {
                        FeedbackReportModel mode = new FeedbackReportModel
                        {
                            Comment = report.OrderDetail.Feedback_Title,
                            Text = report.Text,
                            Create_Date = report.Create_Date,
                            orderDetaiID = report.OrderDetaiID.Value,
                            UserID = report.UserID,
                            ReportID = report.ReportID,
                            Rate = report.OrderDetail.Feedback_Rate,
                            ImagesFB = GetListImageFB(report.OrderDetaiID.Value),
                            ReportStatus=_statusReposity.GetReportStatus(report.ReportStatusID),
                        };
                        list.Add(mode);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;

                }
                else if (reportType == 2)
                {
                    listReport = listReport.Where(r => r.StoreID != null);
                    if (userID.HasValue)
                    {
                        listReport = listReport.Where(r => r.UserID == userID);
                    }
                    if (storeID.HasValue)
                    {
                        listReport = listReport.Where(r => _context.Items.SingleOrDefault(i =>i.StoreID == storeID) != null);
                    }
                    if (page.HasValue)
                    {
                        numpage = (int)Math.Ceiling((double)listReport.Count() / (double)PAGE_SIZE);
                        if (numpage == 0)
                        {
                            numpage = 1;
                        }
                        listReport = listReport.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                    }
                    List<StoreReportModel> list = new List<StoreReportModel>();
                    foreach (var item in listReport.ToList())
                    {
                        StoreReportModel model = new StoreReportModel
                        {
                            ReportID = item.ReportID,
                            StoreID = item.StoreID.Value,
                            Text = item.Text,
                            StoreImage = item.Store.Image,
                            StoreName = item.Store.StoreName,
                            UserID = item.UserID,
                            ReportStatus= _statusReposity.GetReportStatus(item.ReportStatusID),
                            Create_Date=item.Create_Date,
                        };
                        list.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;
                }
                else if(reportType == 3)
                {
                    listReport = listReport.Where(r => r.ItemID != null);
                    if (userID.HasValue)
                    {
                        listReport = listReport.Where(r => r.UserID == userID);
                    }
                    if (storeID.HasValue)
                    {
                        listReport = listReport.Where(r => _context.Items.SingleOrDefault(i => i.ItemID == r.ItemID && i.StoreID == storeID) != null);
                    }
                    if (page.HasValue)
                    {
                        numpage = (int)Math.Ceiling((double)listReport.Count() / (double)PAGE_SIZE);
                        if (numpage == 0)
                        {
                            numpage = 1;
                        }
                        listReport = listReport.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                    }
                    List<ItemReportModel> list = new List<ItemReportModel>();
                    foreach(var item in listReport.ToList())
                    {
                        var product = item.Item;
                        ItemReportModel model = new ItemReportModel
                        {
                            UserID = item.UserID,
                            ItemID = item.ItemID.Value,
                            ItemImage = getItemImage(item.ItemID.Value),
                            ItemName = item.Item.Name,
                            ReportID = item.ReportID,
                            Text = item.Text,
                            ReportStatus = _statusReposity.GetReportStatus(item.ReportStatusID),
                            Create_Date = item.Create_Date,
                        };
                        list.Add(model);
                    }
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = list;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = true;
                result.Message = "Thành công";
                result.Data = new ArrayList();
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public List<Image> GetListImageFB(int orderdetailID)
        {
            var listIF = _context.Feedback_Images.Where(fi => fi.OrderDetailID == orderdetailID).ToList();
            var list = new List<Image>();
            if (listIF.Count > 0)
            {
                foreach (var image in listIF)
                {
                    list.Add(image.Image);
                }
                return list;
            }
            return null;
        }
        public Image getItemImage(int itemID)
        {
            return _context.Item_Images.FirstOrDefault(i => i.ItemID == itemID).Image;
        }
    }
}

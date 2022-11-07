using eSMP.Services.ReportRepo;
using eSMP.Services.StoreAssetRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportReposity _reportReposity;

        public ReportController(IReportReposity reportReposity)
        {
            _reportReposity = reportReposity;
        }
        [HttpPost]
        [Route("store_report")]
        public IActionResult StoreReport(ReportStoreRequest request)
        {
            try
            {
                return Ok(_reportReposity.ReportStore(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPost]
        [Route("item_report")]
        public IActionResult ItemReport(ReportItemRequest request)
        {
            try
            {
                return Ok(_reportReposity.ReportItem(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPost]
        [Route("feedback_report")]
        public IActionResult FeedbackReport(ReportFeedbackRequest request)
        {
            try
            {
                return Ok(_reportReposity.ReportFeedback(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
    }
}

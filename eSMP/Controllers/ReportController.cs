using eSMP.Services.ReportRepo;
using eSMP.Services.StoreAssetRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportReposity _reportReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserReposity _userReposity;

        public ReportController(IReportReposity reportReposity, IHttpContextAccessor httpContextAccessor, IUserReposity userReposity)
        {
            _reportReposity = reportReposity;
            _httpContextAccessor = httpContextAccessor;
            _userReposity = userReposity;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("store_report")]
        public IActionResult StoreReport(ReportStoreRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_reportReposity.ReportStore(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("item_report")]
        public IActionResult ItemReport(ReportItemRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_reportReposity.ReportItem(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2, 3")]
        [Route("feedback_report")]
        public IActionResult FeedbackReport(ReportFeedbackRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_reportReposity.ReportFeedback(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("get_report")]
        public IActionResult GetListReport(int? page, int reportType, int? reportStatusID, int? storeID, int? userID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_reportReposity.GetListReport(page,reportType,reportStatusID, storeID, userID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
    }
}

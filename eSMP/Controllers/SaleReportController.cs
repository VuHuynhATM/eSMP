using eSMP.Services.ReportSaleRepo;
using eSMP.Services.StoreRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleReportController : ControllerBase
    {
        private readonly ISaleReportReposity _saleReportReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;

        public SaleReportController(ISaleReportReposity saleReportReposity, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity)
        {
            _saleReportReposity = saleReportReposity;
            _httpContextAccessor = httpContextAccessor;
            _storeReposity = storeReposity;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1, 3")]
        [Route("hot_item")]
        public IActionResult GetHotITem(int? storeID, int? month, int? year, bool hot)
        {
            var roleid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            if(roleid == "3")
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (store != null)
                {
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                    }
                }
            }
            return Ok(_saleReportReposity.GetListHotItem(storeID, month, year, hot));    
        }
    }
}

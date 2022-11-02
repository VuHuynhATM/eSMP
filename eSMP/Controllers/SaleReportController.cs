using eSMP.Services.ReportSaleRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleReportController : ControllerBase
    {
        private readonly ISaleReportReposity _saleReportReposity;

        public SaleReportController(ISaleReportReposity saleReportReposity)
        {
            _saleReportReposity = saleReportReposity;
        }
        [HttpGet]
        [Route("hot_item")]
        public IActionResult GetHotITem(int? storeID, int? month, int? year, bool hot)
        {
            return Ok(_saleReportReposity.GetListHotItem(storeID, month, year, hot));    
        }
    }
}

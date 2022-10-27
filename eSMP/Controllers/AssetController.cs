using eSMP.Services.StoreAssetRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetReposity _assetReposity;

        public AssetController(IAssetReposity assetReposity)
        {
            _assetReposity = assetReposity;
        }
        [HttpPost]
        public IActionResult SystemWithdrawal(SystemWithdrawalM request)
        {
            try
            {
                return Ok(_assetReposity.SystemWithdrawal(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        public IActionResult SystemInfo()
        {
            try
            {
                return Ok(_assetReposity.GetSystemInfo());
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("get_system_withdrawal")]
        public IActionResult GetSystemWithdrawal(int? page, DateTime? From, DateTime? To)
        {
            try
            {
                return Ok(_assetReposity.GetALlSystemWitdrawl(page, From, To));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("get_system_reveneu")]
        public IActionResult GetALlReveneu(int? page, DateTime? From, DateTime? To)
        {
            try
            {
                return Ok(_assetReposity.GetALlReveneu(page,From, To));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
    }
}

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
        public IActionResult GetSystemReveneu(int? page, DateTime? From, DateTime? To)
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
        [HttpGet]
        [Route("get_store_reveneu")]
        public IActionResult GetStoreReveneu(int storeID, int? page, DateTime? From, DateTime? To)
        {
            try
            {
                return Ok(_assetReposity.GetStoreReveneu(storeID, page,From, To));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPost]
        [Route("store_withdrawal")]
        public IActionResult SendRequestStoreWithdrawal(StoreWithdrawalRequest request)
        {
            try
            {
                return Ok(_assetReposity.CreateStoreWithdrawal(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("accept_store_withdrawal")]
        public IActionResult AcceptRequestStoreWithdrawal(int storeWithdrawalID)
        {
            try
            {
                return Ok(_assetReposity.ProcessStoreWithdrawal(storeWithdrawalID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [HttpPut]
        [Route("cancel_store_withdrawal")]
        public IActionResult CancelRequestStoreWithdrawal(int storeWithdrawalID, string reason)
        {
            try
            {
                return Ok(_assetReposity.CancelStoreWithdrawal(storeWithdrawalID,reason));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("success_store_withdrawal")]
        public IActionResult SuccessRequestStoreWithdrawal(StoreWithdrawalSuccessRequest request)
        {
            try
            {
                return Ok(_assetReposity.SuccessStoreWithdrawal(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("get_store_withdrawal")]
        public IActionResult GetStoreWithdrawal(int? storeID, int? page, int? statusID)
        {
            try
            {
                return Ok(_assetReposity.GetStoreWithdrawal(storeID,page,statusID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("bank")]
        public IActionResult GetBank()
        {
            try
            {
                return Ok(_assetReposity.GetBankSupport());
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
    }
}

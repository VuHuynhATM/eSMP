using eSMP.Models;
using eSMP.Services.StoreAssetRepo;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetReposity _assetReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;
        private readonly IUserReposity _userReposity;

        public AssetController(IAssetReposity assetReposity, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity, IUserReposity userReposity)
        {
            _assetReposity = assetReposity;
            _httpContextAccessor = httpContextAccessor;
            _storeReposity = storeReposity;
            _userReposity = userReposity;
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        public IActionResult SystemWithdrawal([FromForm] SystemWithdrawalM request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                return Ok(_assetReposity.SystemWithdrawal(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1});
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        public IActionResult SystemInfo()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.GetSystemInfo());
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("get_system_withdrawal")]
        public IActionResult GetSystemWithdrawal(int? page, DateTime? From, DateTime? To)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" ,TotalPage = 1 });
                }
                return Ok(_assetReposity.GetALlSystemWitdrawl(page, From, To));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("get_system_reveneu")]
        public IActionResult GetSystemReveneu(int? page, DateTime? From, DateTime? To, int? orderID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" , TotalPage = 1 });
                }
                return Ok(_assetReposity.GetALlReveneu(page, From, To, orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1,3")]
        [Route("get_store_reveneu")]
        public IActionResult GetStoreReveneu(int storeID, int? page, DateTime? From, DateTime? To, int? orderID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" , TotalPage = 1 });
                }
                if (role == "3")
                {
                    var store = _storeReposity.GetStore(int.Parse(userId));
                    if (store == null)
                    {
                        return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                    }
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của cửa hàng khác ", Data = "", TotalPage = 1 });
                    }
                }
                return Ok(_assetReposity.GetStoreReveneu(storeID, page, From, To, orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("store_withdrawal")]
        public IActionResult SendRequestStoreWithdrawal(StoreWithdrawalRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" , TotalPage = 1 });
                }
                else if (store == null)
                {
                    return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                }
                if (store.StoreID != request.StoreID)
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin cửa hàng khác", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.CreateStoreWithdrawal(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("accept_store_withdrawal")]
        public IActionResult AcceptRequestStoreWithdrawal(int storeWithdrawalID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" , TotalPage = 1 });
                }
                return Ok(_assetReposity.ProcessStoreWithdrawal(storeWithdrawalID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("cancel_store_withdrawal")]
        public IActionResult CancelRequestStoreWithdrawal(int storeWithdrawalID, string reason)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" , TotalPage = 1 });
                }
                return Ok(_assetReposity.CancelStoreWithdrawal(storeWithdrawalID, reason));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("success_store_withdrawal")]
        public IActionResult SuccessRequestStoreWithdrawal([FromForm] StoreWithdrawalSuccessRequest request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.SuccessStoreWithdrawal(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1});
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1, 3")]
        [Route("get_store_withdrawal")]
        public IActionResult GetStoreWithdrawal(int? storeID, int? page, int? statusID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (role == "3")
                {
                    var store = _storeReposity.GetStore(int.Parse(userId));
                    if (store == null)
                    {
                        return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                    }
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin cửa hàng khác", Data = "", TotalPage = 1 });
                    }
                }
                return Ok(_assetReposity.GetStoreWithdrawal(storeID, page, statusID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
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
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("store_chart_reveneu")]
        public IActionResult GetStoreChart(int storeID, int? year)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var store = _storeReposity.GetStore(int.Parse(userId));
                if (!_storeReposity.CheckStoreActive(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (store == null)
                {
                    return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                }
                if (store.StoreID != storeID)
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin cửa hàng khác", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.GetStoreReveneuForChart(storeID, year));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("system_chart_reveneu")]
        public IActionResult GetSystemChart(int? year)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.GetSystemReveneuForChart(year));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("system_store_reveneu")]
        public IActionResult GetSystemStore(int? page, DateTime? From, DateTime? To)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.GetStoreSystemReveneu(page, From, To));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("update_commission_precent")]
        public IActionResult UpdateCommission_Precent(double commission_Precent)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.UpdateCommission_Precent(commission_Precent));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("update_amount_active")]
        public IActionResult UpdateAmountActive(double amountActive)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.UpdateAmountActive(amountActive));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("update_refund_precent")]
        public IActionResult UpdateRefund_Precent(double refund_Precent)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_assetReposity.UpdateRefund_Precent(refund_Precent));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
    }
}

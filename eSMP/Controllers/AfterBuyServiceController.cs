using eSMP.Services.AfterBuyServiceRepo;
using eSMP.Services.DataExchangeRepo;
using eSMP.Services.StoreAssetRepo;
using eSMP.Services.StoreRepo;
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
    public class AfterBuyServiceController : ControllerBase
    {
        private readonly IAfterBuyServiceReposity _afterBuyServiceReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;
        private readonly IUserReposity _userReposity;
        public AfterBuyServiceController(IAfterBuyServiceReposity afterBuyServiceReposity, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity, IUserReposity userReposity)
        {
            _afterBuyServiceReposity = afterBuyServiceReposity;
            _httpContextAccessor = httpContextAccessor;
            _storeReposity = storeReposity;
            _userReposity = userReposity;
        }
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        public IActionResult CreateService([FromForm] AfterBuyServiceModel request)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }*/
                return Ok(_afterBuyServiceReposity.CreateChangeService(request));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Route("cancel")]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1, 3")]
        public IActionResult CancelService(CancelService cancelService)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                if (role == "3")
                {
                    var store = _storeReposity.GetStore(int.Parse(userId));
                    if (store == null)
                    {
                        return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                    }
                }*/
                return Ok(_afterBuyServiceReposity.CancelService(cancelService.serviceID, cancelService.reason));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Route("accepct")]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1, 3")]
        public IActionResult AcceptService(int serviceID)
        {
            try
            {
               /* var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                if (role == "3")
                {
                    var store = _storeReposity.GetStore(int.Parse(userId));
                    if (store == null)
                    {
                        return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                    }
                }*/
                return Ok(_afterBuyServiceReposity.AcceptService(serviceID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }

        [HttpPut]
        [Route("warning")]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        public IActionResult WarningService(int serviceID)
        {
            try
            {
               /* var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                if (role == "3")
                {
                    var store = _storeReposity.GetStore(int.Parse(userId));
                    if (store == null)
                    {
                        return Ok(new Result { Success = false, Message = "chưa tạo cửa hàng", Data = "", TotalPage = 1 });
                    }
                }*/
                return Ok(_afterBuyServiceReposity.WarningService(serviceID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }

        [HttpGet]
        //[Authorize(AuthenticationSchemes = "AuthDemo")]
        public IActionResult GetService(int? serviceID, int? storeID, int? orderID, int? userID, DateTime? from, DateTime? to, int? serviceType, int? servicestatusID, int? page)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var roleid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (roleid == "2")
                {
                    if (userId != userID + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                    }
                }
                else if (roleid == "3")
                {
                    var storeIDofUser = _storeReposity.GetStore(int.Parse(userId)).StoreID;
                    if (storeID.Value != storeIDofUser)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của cửa hàng khác", Data = "", TotalPage = 1 });
                    }
                }*/
                return Ok(_afterBuyServiceReposity.GetServices(serviceID,storeID,orderID,userID,from,to, serviceType,servicestatusID,page));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
    }
}

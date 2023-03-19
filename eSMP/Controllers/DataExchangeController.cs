using eSMP.Services.DataExchangeRepo;
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
    public class DataExchangeController : ControllerBase
    {
        private readonly IDataExchangeReposity _dataExchangeReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;
        private readonly IUserReposity _userReposity;

        public DataExchangeController(IDataExchangeReposity dataExchangeReposity, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity, IUserReposity userReposity)
        {
            _dataExchangeReposity = dataExchangeReposity;
            _httpContextAccessor = httpContextAccessor;
            _storeReposity = storeReposity;
            _userReposity = userReposity;
        }
        [HttpGet]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1, 3")]
        public IActionResult GetStoreDataExchanges(int? storeID, int? orderID, int? serviceID, DateTime? from, DateTime? to, int? page)
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
                    if (store.StoreID != storeID)
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của cửa hàng khác ", Data = "", TotalPage = 1 });
                    }
                }*/
                return Ok(_dataExchangeReposity.GetStoreDataExchanges(storeID, orderID, serviceID, from, to, page));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        public IActionResult FinishStoreDataExchanges([FromForm] DataExchangeStoreFinish DataExchange)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }*/
                return Ok(_dataExchangeReposity.FinishStoreDataExchange(DataExchange));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }

        [HttpGet]
        [Route("user")]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1, 2")]
        public IActionResult GetUserDataExchanges(int? userID, int? orderID, int? serviceID, DateTime? from, DateTime? to, int? page)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                if (role == "2")
                {
                    if (userId != userID.Value + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác ", Data = "", TotalPage = 1 });
                    }
                }*/
                return Ok(_dataExchangeReposity.GetStoreDataExchanges(userID, orderID, serviceID, from, to, page));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Route("user")]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        public IActionResult FinishUserDataExchanges([FromForm] DataExchangeUserFinish DataExchange)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }*/
                return Ok(_dataExchangeReposity.FinishUserDataExchange(DataExchange));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Route("user_addcard")]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        public IActionResult AddCardUserDataExchanges(DataExchangeUserAddCard DataExchange)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }*/
                return Ok(_dataExchangeReposity.AddCardUserDataExchange(DataExchange));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
    }
}

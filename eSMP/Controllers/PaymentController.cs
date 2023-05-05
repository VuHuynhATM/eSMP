using eSMP.Models;
using eSMP.Services.MomoRepo;
using eSMP.Services.OrderRepo;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMomoReposity _momoReposity;
        private readonly WebContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;
        private readonly IOrderReposity _orderReposity;
        private readonly IUserReposity _userReposity;

        public PaymentController(IMomoReposity momoReposity, WebContext context, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity, IOrderReposity orderReposity, IUserReposity userReposity)
        {
            _momoReposity = momoReposity;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _storeReposity = storeReposity;
            _orderReposity = orderReposity;
            _userReposity = userReposity;
        }
        [HttpPost]
        public IActionResult ipnUrl(MomoPayINP payINP)
        {
            int resultCode = payINP.resultCode;
            if (resultCode == 0)
                _momoReposity.PayOrderINPAsync(payINP);
            return Ok();
        }
        [HttpGet]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("order_pay_url")]
        public IActionResult PayOrder(int orderID, string paymentMethod)
        {
            try
            {
                /*var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderID(orderID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userIdOfOrder + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }*/
                return Ok(_momoReposity.GetPayUrl(orderID,paymentMethod));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("cancel_order")]
        public IActionResult CancelOrder(int orderID, string reason)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var roleid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderID(orderID);
                var supplierIDOfOrder = _orderReposity.GetSuppilerIDByOrderID(orderID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (roleid == "2")
                {
                    if (userId != userIdOfOrder + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                    }
                }
                else if (roleid == "3")
                {
                    if (userId != supplierIDOfOrder + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của cửa hàng khác", Data = "", TotalPage = 1 });
                    }
                }
                return Ok(_momoReposity.CancelOrder(orderID, reason));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("confim_order")]
        public IActionResult ConfimlOrder(int orderID)
        {
            try
            {
                return Ok(_momoReposity.ConfimOrder(orderID));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "3")]
        [Route("store_pay_url")]
        public IActionResult PayStore(int storeID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var supplierStoreID= _storeReposity.GetStore(int.Parse(userId)).StoreID;
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (supplierStoreID != storeID)
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của cửa hàng khác", Data = "", TotalPage = 1 });
                }
                return Ok(_momoReposity.GetStorePayUrl(storeID));
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("store")]
        public IActionResult ipnStoreUrl(MomoPayINP payINP)
        {
            int resultCode = payINP.resultCode;
            if (resultCode == 0)
                _momoReposity.PayStoreINP(payINP);
            return Ok();
        }
        [HttpGet]
        [Route("info")]
        public IActionResult infoOrder(int orderID)
        {
            try
            {
                return Ok(_momoReposity.InfoPay(orderID));
            }
            catch
            {
                return BadRequest();
            }
        }
        /*[HttpGet]
        [Route("test")]
        public IActionResult test(int id)
        {
            return Ok(_momoReposity.RefundService(id, 1));
        }*/
    }
}

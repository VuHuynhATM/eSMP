using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderReposity _orderReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreReposity _storeReposity;
        private readonly IUserReposity _userReposity;

        public OrderController(IOrderReposity orderReposity, IHttpContextAccessor httpContextAccessor, IStoreReposity storeReposity, IUserReposity userReposity)
        {
            _orderReposity = orderReposity;
            _storeReposity = storeReposity;
            _httpContextAccessor = httpContextAccessor;
            _userReposity = userReposity;
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        public IActionResult GetOrder(int userID, int? orderStatusID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.GetAllOrder(userID, orderStatusID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        public IActionResult AddToCart(OrderDetailAdd orderDetail)
        {
            try
            {
               /* var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != orderDetail.UserID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }*/
                return Ok(_orderReposity.AddOrderDetail(orderDetail));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("update_address")]
        public IActionResult UpdateAddress(int orderID, int AddressID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderID(orderID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userIdOfOrder + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.UpdateOrderAddress(orderID,AddressID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("update_amount")]
        public IActionResult UpdateAmount(int orderDetailID, int amount)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderDetailID(orderDetailID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userIdOfOrder + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.UpdateAmountOrderDetail(orderDetailID,amount));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        public IActionResult RemoveOrder(int orderID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderID(orderID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userIdOfOrder + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.DeleteOrder(orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("remove_orderdetail")]
        public IActionResult RemoveOrderDetail(int orderDetailID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderDetailID(orderDetailID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userIdOfOrder + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.DeleteOrderDetail(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("order_info")]
        public IActionResult GetOrderInfo(int orderID)
        {
            try
            {
                return Ok(_orderReposity.GetOrderInfo(orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("feedback")]
        public IActionResult FeedBack ([FromForm] FeedBackOrderDetail feedBack)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderDetailID(feedBack.OrderDetaiID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userIdOfOrder + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.FeedBaclOrderDetail(feedBack));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("get_order_status")]
        public IActionResult GetOrdersWithShipstatus(int? userID, int? storeID, DateTime? dateFrom, DateTime? dateTo, int? shipOrderStatus, int? page, string? userName,int? orderID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
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
                }
                return Ok(_orderReposity.GetOrdersWithShipstatus(userID,storeID,dateFrom,dateTo,shipOrderStatus,page, userName,orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2")]
        [Route("get_list_feedback")]
        public IActionResult GetlistFeedback(int? page, bool isFeedback, int? userID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                else if (userId != userID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.GetlistFeedback(page,isFeedback,userID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("get_feedback_detail")]
        public IActionResult GetFeedbackDetail(int orderDetailID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderDetailID(orderDetailID);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.GetFeedbackDetail(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2, 3")]
        [Route("hidden_feedback")]
        public IActionResult HiddenFeedback(int orderDetailID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var roleid = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userIdOfOrder = _orderReposity.GetUserIDByOrderDetailID(orderDetailID);
                var SupplierIdOfOrder = _orderReposity.GetSuppilerIDByOrderDetailID(orderDetailID);
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
                    if (userId != SupplierIdOfOrder+"")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của cửa hàng khác", Data = "", TotalPage = 1 });
                    }
                }
                return Ok(_orderReposity.HiddenFeedback(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("block_feedback")]
        public IActionResult BlockFeedback(int orderDetailID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                return Ok(_orderReposity.BlockFeedback(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
        [HttpGet]
        [Route("check_pay")]
        public IActionResult CheckOrderPay(int orderID)
        {
            try
            {
                return Ok(_orderReposity.CheckPay(orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", TotalPage = 1 });
            }
        }
    }
}

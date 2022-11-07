using eSMP.Models;
using eSMP.Services.OrderRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderReposity _orderReposity;

        public OrderController(IOrderReposity orderReposity)
        {
            _orderReposity = orderReposity;
        }
        [HttpGet]
        public IActionResult GetOrder(int userID, int? orderStatusID)
        {
            try
            {
                return Ok(_orderReposity.GetAllOrder(userID, orderStatusID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPost]
        public IActionResult AddToCart(OrderDetailAdd orderDetail)
        {
            try
            {
                return Ok(_orderReposity.AddOrderDetail(orderDetail));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("update_address")]
        public IActionResult UpdateAddress(int orderID, int AddressID)
        {
            try
            {
                return Ok(_orderReposity.UpdateOrderAddress(orderID,AddressID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("update_amount")]
        public IActionResult UpdateAmount(int orderDetailID, int amount)
        {
            try
            {
                return Ok(_orderReposity.UpdateAmountOrderDetail(orderDetailID,amount));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpDelete]
        public IActionResult RemoveOrder(int orderID)
        {
            try
            {
                return Ok(_orderReposity.DeleteOrder(orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpDelete]
        [Route("remove_orderdetail")]
        public IActionResult RemoveOrderDetail(int orderDetailID)
        {
            try
            {
                return Ok(_orderReposity.DeleteOrderDetail(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("order_info")]
        public IActionResult GetOrderInfo(int orderID)
        {
            try
            {
                return Ok(_orderReposity.GetOrderInfo(orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPost]
        [Route("feedback")]
        public IActionResult FeedBack (FeedBackOrderDetail feedBack)
        {
            try
            {
                return Ok(_orderReposity.FeedBaclOrderDetail(feedBack));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("get_order_status")]
        public IActionResult GetOrdersWithShipstatus(int? userID, int? storeID, DateTime? dateFrom, DateTime? dateTo, int? shipOrderStatus, int? page)
        {
            try
            {
                return Ok(_orderReposity.GetOrdersWithShipstatus(userID,storeID,dateFrom,dateTo,shipOrderStatus,page));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("get_list_feedback")]
        public IActionResult GetlistFeedback(int? page, bool isFeedback, int? userID)
        {
            try
            {
                return Ok(_orderReposity.GetlistFeedback(page,isFeedback,userID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpGet]
        [Route("get_feedback_detail")]
        public IActionResult GetFeedbackDetail(int orderDetailID)
        {
            try
            {
                return Ok(_orderReposity.GetFeedbackDetail(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("hidden_feedback")]
        public IActionResult HiddenFeedback(int orderDetailID)
        {
            try
            {
                return Ok(_orderReposity.HiddenFeedback(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("block_feedback")]
        public IActionResult BlockFeedback(int orderDetailID)
        {
            try
            {
                return Ok(_orderReposity.BlockFeedback(orderDetailID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
        [HttpPut]
        [Route("check_pay")]
        public IActionResult CheckOrderPay(int orderID)
        {
            try
            {
                return Ok(_orderReposity.CheckPay(orderID));
            }
            catch
            {
                return Ok(new Result { Success = false, Message = "Lỗi hệ thống", Data = "", });
            }
        }
    }
}

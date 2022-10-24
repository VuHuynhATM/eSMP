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
        [Route("orderinfo")]
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
    }
}

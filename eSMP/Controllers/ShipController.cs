using eSMP.Models;
using eSMP.Services.ShipRepo;
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
    public class ShipController : ControllerBase
    {
        private readonly IShipReposity _shipReposity;
        private readonly WebContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserReposity _userReposity;

        public ShipController(IShipReposity shipReposity, WebContext context, IHttpContextAccessor httpContextAccessor, IUserReposity userReposity)
        {
            _shipReposity = shipReposity;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userReposity = userReposity;
        }
        [HttpPost]
        public IActionResult CallBackGHTK(ShipINP inp)
        {
            var label_id = inp.label_id;
            var partner_id = inp.partner_id;
            var status_id = inp.status_id;
            var action_time = inp.action_time;
            var reason_code = inp.reason_code;
            var reason = inp.reason;
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName = label_id + "-" + partner_id + "-" + status_id;
            _context.SaveChanges();
            if (_shipReposity.CallBackAsync(label_id, partner_id, status_id, action_time, reason_code, reason))
            {
                return Ok();
            }
            return Ok();
        }
        /*[HttpGet]
        [Route("create_order")]
        public IActionResult Createorder(int orderID)
        {
            return Ok(_shipReposity.CreateOrder(orderID));
        }*/
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("ship_status")]
        public IActionResult GetShipstatus(int? orderID, int? serviceID)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!_userReposity.CheckUser(int.Parse(userId)))
            {
                return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
            }
            return Ok(_shipReposity.GetShipstatus(orderID, serviceID));
        }

        /*[HttpPut]
        [Route("ship_Cancel")]
        public IActionResult Cancel(int orderID)
        {
            return Ok(_shipReposity.CancelOrder(orderID));
        }*/
        [HttpGet]
        [Route("get_ticket")]
        public IActionResult GetTicket(int orderID)
        {
            return Ok(_shipReposity.GetTicket(orderID));
        }
    }
}

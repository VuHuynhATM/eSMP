using eSMP.Models;
using eSMP.Services.ShipRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private readonly IShipReposity _shipReposity;
        private readonly IUserReposity _userReposity;
        private readonly WebContext _context;

        public ShipController(IShipReposity shipReposity, IUserReposity userReposity, WebContext context)
        {
            _shipReposity = shipReposity;
            _userReposity = userReposity;
            _context = context;
        }
        [HttpPost]
        public IActionResult CallBackGHTK(ShipINP inp)
        {
            var label_id=inp.label_id;
            var partner_id=inp.partner_id;
            var status_id =inp.status_id;
            var action_time=inp.action_time;
            var reason_code=inp.reason_code;
            var reason =inp.reason;
            if (_shipReposity.CallBackAsync(label_id, partner_id, status_id, action_time, reason_code, reason))
            {
                return Ok();
            }
            return Ok();
        }
        [HttpGet]
        [Route("createoder")]
        public IActionResult Createorder(int orderID)
        {
            return Ok(_shipReposity.CreateOrder(orderID));
        }
        [HttpGet]
        [Route("ship_status")]
        public IActionResult GetShipstatus(int orderID)
        {
            return Ok(_shipReposity.GetShipstatus(orderID));
        }

        [HttpPut]
        [Route("ship_Cancel")]
        public IActionResult Cancel(int orderID)
        {
            return Ok(_shipReposity.CancelOrder(orderID));
        }
        [HttpGet]
        [Route("get_ticket")]
        public IActionResult GetTicket(int orderID)
        {
            return Ok(_shipReposity.GetTicket(orderID));
        }
    }
}

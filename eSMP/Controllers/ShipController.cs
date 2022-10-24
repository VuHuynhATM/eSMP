﻿using eSMP.Models;
using eSMP.Services.ShipRepo;
using eSMP.Services.UserRepo;
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
        //string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason, float weight, int fee, int return_part_package
        [HttpPost]
        public IActionResult CallBackGHTK()
        {
            _userReposity.Updaterole();
            /*var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName= label_id;
            _context.SaveChanges();*/
            /*if (_shipReposity.CallBackAsync(label_id, partner_id, status_id, action_time, reason_code, reason))
            {
                return Ok();
            }*/
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
    }
}

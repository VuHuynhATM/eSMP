﻿using eSMP.Models;
using eSMP.Services.MomoRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMomoReposity _momoReposity;
        private readonly WebContext _context;

        public PaymentController(IMomoReposity momoReposity, WebContext context)
        {
            _momoReposity = momoReposity;
            _context = context;
        }
        [HttpPost]
        public IActionResult ipnUrl(MomoPayINP payINP)
        {
            string orderId = payINP.orderId.Split('-')[0];
            long transId = payINP.transId;
            int resultCode = payINP.resultCode;
            long responseTime = payINP.responseTime;
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName = orderId + "-" + transId + "-" + resultCode + "-" + responseTime;
            _context.SaveChanges();
            if (resultCode == 9000)
                _momoReposity.PayOrderINP(payINP);
            return Ok();
        }
        [HttpGet]
        [Route("momopay")]
        public IActionResult PayOrder(int orderID)
        {
            try
            {
                return Ok(_momoReposity.GetPayUrl(orderID));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

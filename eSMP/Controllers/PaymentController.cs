using eSMP.Models;
using eSMP.Services.MomoRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            int resultCode = payINP.resultCode;
            if (resultCode == 0)
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
        [HttpPut]
        [Route("cancel_order")]
        public IActionResult CancelOrder(int orderID, string reason)
        {
            try
            {
                return Ok(_momoReposity.CancelOrder(orderID,reason));
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
        [Route("momo_store_pay")]
        public IActionResult PayStore(int storeID)
        {
            try
            {
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
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == 4);
            role.RoleName = payINP.message;
            _context.SaveChanges();
            int resultCode = payINP.resultCode;
            if (resultCode == 0)
                _momoReposity.PayStoreINP(payINP);
            return Ok();
        }
    }
}

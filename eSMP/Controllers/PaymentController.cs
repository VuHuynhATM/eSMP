using eSMP.Services;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IUserReposity _userReposity;

        public PaymentController(IUserReposity userReposity)
        {
            _userReposity = userReposity;
        }
        [HttpPost]
        public IActionResult payment()
        {
            try
            {
                _userReposity.Updaterole();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
            
        }
    }
}

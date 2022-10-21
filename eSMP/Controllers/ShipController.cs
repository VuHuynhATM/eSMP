using eSMP.Services.ShipRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private readonly IShipReposity _shipReposity;
        public ShipController(IShipReposity shipReposity)
        {
            _shipReposity = shipReposity;
        }
        [HttpPost]
        public IActionResult CallBackGHTK(string label_id, string partner_id, int status_id, string action_time, string reason_code, string reason, float weight, int fee, int return_part_package)
        {
            if (_shipReposity.CallBackAsync(label_id, partner_id, status_id, action_time, reason_code, reason))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        [Route("createoder")]
        public IActionResult Createorder(int orderID)
        {
            _shipReposity.CreateOrder(orderID);
            return Ok();
        }
    }
}

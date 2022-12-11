using eSMP.Services.AddressRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressReposity _addressReposity;

        public AddressController(IAddressReposity addressReposity)
        {
            _addressReposity = addressReposity;
        }
        [HttpGet]
        [Route("provine")]
        public IActionResult GetlistProvine()
        {
            return Ok(_addressReposity.GetProvines());
        }
        [HttpGet]
        [Route("district")]
        public IActionResult GetlistDis(string tpid)
        {
            return Ok(_addressReposity.GetDistrict(tpid));
        }
        [HttpGet]
        [Route("ward")]
        public IActionResult GetlistWard(string qhid)
        {
            return Ok(_addressReposity.GetWard(qhid));
        }
    }
}

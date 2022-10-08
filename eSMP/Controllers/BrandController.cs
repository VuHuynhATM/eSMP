using eSMP.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandReposity _brandReposity;

        public BrandController(IBrandReposity brandReposity)
        {
            _brandReposity = brandReposity;
        }
        [HttpGet]
        public IActionResult GetAllBrand()
        {
            try
            {
                var result = _brandReposity.GetAllBrand();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("brand_model")]
        public IActionResult GetBrandModel(int brandID)
        {
            try
            {
                var result = _brandReposity.GetBrandModel(brandID);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("item")]
        public IActionResult GetBrandModelItem(int itemID)
        {
            try
            {
                var result = _brandReposity.GetBrandModelForItem(itemID);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}

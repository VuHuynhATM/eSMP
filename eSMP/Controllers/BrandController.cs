using eSMP.Services.BrandRepo;
using eSMP.Services.SpecificationRepo;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("create_brand")]
        public IActionResult CreateBrand(string brand_Name)
        {
            try
            {
                var result = _brandReposity.CreateBrand(brand_Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("create_motorcycle")]
        public IActionResult CreateMotorcycle(int brandID, string moto_Name)
        {
            try
            {
                var result = _brandReposity.CreateMotorcycle(brandID, moto_Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("remove_motorcycle")]
        public IActionResult removeMotorcycle(int motorcycleID)
        {
            try
            {
                var result = _brandReposity.RemoveMotorcycle(motorcycleID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("active_motorcycle")]
        public IActionResult ActiveMotorcycle(int motorcycleID)
        {
            try
            {
                var result = _brandReposity.ActiveMotorcycle(motorcycleID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("remove_brand")]
        public IActionResult RemoveBrand(int brandID)
        {
            try
            {
                var result = _brandReposity.RemoveBrand(brandID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("active_brand")]
        public IActionResult ActiveBrand(int brandID)
        {
            try
            {
                var result = _brandReposity.ActiveBrand(brandID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

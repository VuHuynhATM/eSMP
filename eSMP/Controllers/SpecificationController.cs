using eSMP.Services.SpecificationRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecificationController : ControllerBase
    {
        private readonly ISpecificationReposity _specificationReposity;

        public SpecificationController(ISpecificationReposity specificationReposity)
        {
            _specificationReposity = specificationReposity;
        }
        [HttpGet]
        public IActionResult GetallSpecification()
        {
            try
            {
                var result = _specificationReposity.GetAllSpecification();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("sub_category")]
        public IActionResult GetSpecificationsBySubCate(int sub_CategoryID)
        {
            try
            {
                var result = _specificationReposity.GetSpecificationsBySubCate(sub_CategoryID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

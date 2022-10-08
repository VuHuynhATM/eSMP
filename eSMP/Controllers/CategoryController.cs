using eSMP.Models;
using eSMP.Services;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryReposity _categoryReposity;

        public CategoryController(ICategoryReposity categoryReposity)
        {
            _categoryReposity = categoryReposity;
        }
        [HttpGet]
        public IActionResult GetAllCategory()
        {
            try
            {
                var result = _categoryReposity.GetAllCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("sub_category")]
        public IActionResult GetSubCategory(int categoryID)
        {
            try
            {
                var result = _categoryReposity.GetSubCategory(categoryID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

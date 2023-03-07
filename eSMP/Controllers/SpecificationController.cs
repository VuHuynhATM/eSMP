using eSMP.Services.SpecificationRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecificationController : ControllerBase
    {
        private readonly ISpecificationReposity _specificationReposity;
        private readonly IUserReposity _userReposity;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SpecificationController(ISpecificationReposity specificationReposity, IUserReposity userReposity, IHttpContextAccessor httpContextAccessor)
        {
            _specificationReposity = specificationReposity;
            _userReposity = userReposity;
            _httpContextAccessor = httpContextAccessor;
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
        public IActionResult GetSpecificationsBySubCate(int sub_CategoryID, string? role)
        {
            try
            {
                //var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var result = _specificationReposity.GetSpecificationsBySubCate(sub_CategoryID,role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        public IActionResult CreateSpecification(string specification_Name)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                var result = _specificationReposity.CreateSpecification(specification_Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("add_specification")]
        public IActionResult AddSpecification(CateSpecification_Request request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _specificationReposity.AddSpecification(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        /*[HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("remove_category_specification")]
        public IActionResult ReomoveCateSpecification(CateSpecification_Request request)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _specificationReposity.ReomoveCateSpecification(request.sub_CategoryID, request.specificationIDsRemove);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }*/
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("remove_specification")]
        public IActionResult ReomoveSpecification(int specificationID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _specificationReposity.ReomoveSpecification(specificationID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("active_specification")]
        public IActionResult ActiveSpecification(int specificationID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _specificationReposity.ActiveSpecification(specificationID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("add_specificationsuggset")]
        public IActionResult AddSpecificationSuggest(SpecSuggestAdd input)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _specificationReposity.AddSuggesTSpecification(input.specificationID, input.suggsetvalues);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

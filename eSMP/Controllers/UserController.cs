using eSMP.Models;
using eSMP.Services.StoreRepo;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly IUserReposity _userReposity;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public userController(IUserReposity userReposity, IHttpContextAccessor httpContextAccessor)
        {
            _userReposity = userReposity;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [Authorize]
        [Route("customersign_in")]
        public IActionResult CustomerLogin(UserLogin user)
        {
            try
            {
                var result = _userReposity.CustomerLogin(user.Phone, user.FCM_Firebase);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("suppliersign_in")]
        public IActionResult SuplierLogin(UserLogin user)
        {
            try
            {
                var result = _userReposity.SupplierLogin(user.Phone, user.FCM_Firebase);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("adminsign_in")]
        public IActionResult AdminLogin(AdminLogin user)
        {
            try
            {
                var result = _userReposity.LoginByEmail(user.Email, user.Password, user.FCM_Firebase);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("logout")]
        public IActionResult UserLogout(int userID)
        {
            try
            {
                var result = _userReposity.Logout(userID);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("user_register")]
        public IActionResult UserRegister(UserRegister user)
        {
            try
            {
                var result = _userReposity.RigisterUser(user);
                if (result == null)
                    return BadRequest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpPost]
        //[Authorize]
        [Route("supplier_register")]
        public IActionResult SupplierRegister(UserRegister user)
        {
            try
            {
                var result = _userReposity.RigisterSupplier(user);
                if (result == null)
                    return BadRequest();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }

        [HttpPost]
        [Route("check_user")]
        public IActionResult CheckUser(string phone)
        {
            try
            {
                var result = _userReposity.CheckRole(phone);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }

        [HttpGet]
        [Authorize]
        [Route("get_users")]
        public IActionResult GetAllUser(int? page, string? search, int? roleID, bool? isActive)
        {
            try
            {
                var result = _userReposity.GetListUser(page, search, roleID, isActive);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "1")]
        [Route("Update_user_status")]
        public IActionResult UpdateUserStatus(int UserID, Boolean isActive, string? statusText)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "", TotalPage = 1 });
                }
                var result = _userReposity.UpdatteUserStatus(UserID, isActive, statusText);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message, TotalPage = 1 });
            }
        }
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("remove_user")]
        public IActionResult RemoveUser(int UserID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _userReposity.RemoveUser(UserID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("add_address")]
        public IActionResult AddAddress(UserAddAddress address)
        {
            try
            {
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (role != "1")
                {
                    if (userId != address.UserID + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                var result = _userReposity.AddAddress(address);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("remove_address")]
        public IActionResult RemoveAddress(int addressID)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _userReposity.RemoveAddress(addressID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("edt_address")]
        public IActionResult EditAddress(Address address)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                var result = _userReposity.UpdateAddress(address);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("edit_name")]
        public IActionResult EditName(EditName name)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (userId != name.UserID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                }
                var result = _userReposity.UpdateName(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "2, 3")]
        [Route("edit_email")]
        public IActionResult EditEmail(EditEmail email)
        {
            try
            {
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (role != "1")
                {
                    if (userId != email.UserID + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                var result = _userReposity.UpdateEmail(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("edit_gender")]
        public IActionResult EditGender(EditGender gender)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (userId != gender.UserID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                }
                var result = _userReposity.UpdateGender(gender);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("edit_birth")]
        public IActionResult EditBirth([FromForm] EditBirth birth)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (userId != birth.UserID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                }
                var result = _userReposity.UpdateBirth(birth);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("edit_image")]
        public IActionResult EditImage([FromForm] EditImage image)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (userId != image.UserID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                }
                var result = _userReposity.UpdateImage(image);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("refeshtoken")]
        public IActionResult RefeshToken(int userID, string token)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (userId != userID + "")
                {
                    return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                }
                var result = _userReposity.RefeshToken(userID, token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("address")]
        public IActionResult GetAddressbyID(int userID)
        {
            try
            {
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (role != "1")
                {
                    if (userId != userID + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                var result = _userReposity.GetAddressByID(userID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "AuthDemo")]
        [Route("detail")]
        public IActionResult GetUserbyID(int userID)
        {
            try
            {
                var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!_userReposity.CheckUser(int.Parse(userId)))
                {
                    return Ok(new Result { Success = false, Message = "Tài khoản đang bị hạn chế", Data = "" });
                }
                else if (role != "1")
                {
                    if (userId != userID + "")
                    {
                        return Ok(new Result { Success = false, Message = "Bạn không được phép truy cập thông tin của người khác", Data = "", });
                    }
                }
                var result = _userReposity.GetUserByID(userID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("AdminContact")]
        public IActionResult AdminContact()
        {
            try
            {
                var result = _userReposity.GetAdminContact();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
    }
}

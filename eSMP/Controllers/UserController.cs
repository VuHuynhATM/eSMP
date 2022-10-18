using eSMP.Models;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly IUserReposity _userReposity;

        public userController(IUserReposity userReposity)
        {
            _userReposity = userReposity;
        }
        [HttpPost]
        [Authorize]
        [Route("customersign_in")]
        public IActionResult CustomerLogin(UserLogin user)
        {
            try
            {
                var result=_userReposity.CustomerLogin(user.Phone);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Route("suppliersign_in")]
        public IActionResult SuplierLogin(UserLogin user)
        {
            try
            {
                var result=_userReposity.SupplierLogin(user.Phone);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Route("adminsign_in")]
        public IActionResult AdminLogin(AdminLogin user)
        {
            try
            {
                var result=_userReposity.LoginByEmail(user.Email, user.Pasword);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Route("user_register")]
        public IActionResult UserRegister(UserRegister user)
        {
            try
            {
                var result=_userReposity.RigisterUser(user);
                if (result == null)
                    return BadRequest();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
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
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }

        [HttpPost]
        [Route("check_user")]
        public IActionResult CheckUser(string phone,int roleID)
        {
            try
            {
                var result=_userReposity.CheckRole(phone,roleID);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }

        [HttpGet]
        [Route("get_users")]
        public IActionResult GetAllUser(int? page)
        {
            try
            {
                var result = _userReposity.GetListUser(page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [Route("Search_user")]
        public IActionResult SearchUser(string phone, int roleID)
        {
            try
            {
                var result = _userReposity.SearchUser(phone,roleID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("Update_user_status")]
        public IActionResult UpdateUserStatus(int UserID, Boolean isActive)
        {
            try
            {
                var result = _userReposity.UpdatteUserStatus(UserID, isActive);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpDelete]
        [Route("remove_user")]
        public IActionResult RemoveUser(int UserID)
        {
            try
            {
                var result = _userReposity.RemoveUser(UserID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Authorize]
        [Route("add_address")]
        public IActionResult AddAddress(UserAddAddress address)
        {
            try
            {
                var result = _userReposity.AddAddress(address);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpDelete]
        [Authorize]
        [Route("remove_address")]
        public IActionResult RemoveAddress(int addressID)
        {
            try
            {
                var result = _userReposity.RemoveAddress(addressID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("edt_address")]
        public IActionResult EditAddress(Address address)
        {
            try
            {
                var result = _userReposity.UpdateAddress(address);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("edit_name")]
        public IActionResult EditName(EditName name)
        {
            try
            {
                var result = _userReposity.UpdateName(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "AuthDemo", Roles = "Customer, Supplier")]
        [Route("edit_email")]
        public IActionResult EditEmail(EditEmail email)
        {
            try
            {
                var result = _userReposity.UpdateEmail(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("edit_gender")]
        public IActionResult EditGender(EditGender gender)
        {
            try
            {
                var result = _userReposity.UpdateGender(gender);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("edit_birth")]
        public IActionResult EditBirth(EditBirth birth)
        {
            try
            {
                var result = _userReposity.UpdateBirth(birth);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPut]
        [Authorize]
        [Route("edit_image")]
        public IActionResult EditImage(EditImage image)
        {
            try
            {
                var result = _userReposity.UpdateImage(image);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpPost]
        [Route("refeshtoken")]
        public IActionResult RefeshToken(int userID, string token)
        {
            try
            {
                var result = _userReposity.RefeshToken(userID,token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("address")]
        public IActionResult GetAddressbyID(int userID)
        {
            try
            {
                var result = _userReposity.GetAddressByID(userID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
        [HttpGet]
        [Route("detail")]
        public IActionResult GetUserbyID(int userID)
        {
            try
            {
                var result = _userReposity.GetUserByID(userID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }
    }
}

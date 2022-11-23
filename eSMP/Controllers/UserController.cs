﻿using eSMP.Models;
using eSMP.Services.UserRepo;
using eSMP.VModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        [Route("customersign_in")]
        public IActionResult CustomerLogin(UserLogin user)
        {
            try
            {
                var result=_userReposity.CustomerLogin(user.Phone, user.FCM_Firebase);
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
                var result=_userReposity.SupplierLogin(user.Phone, user.FCM_Firebase);
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
        [Authorize]
        [Route("adminsign_in")]
        public IActionResult AdminLogin(AdminLogin user)
        {
            try
            {
                var result=_userReposity.LoginByEmail(user.Email, user.Password, user.FCM_Firebase);
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
        public IActionResult CheckUser(string phone)
        {
            try
            {
                var result=_userReposity.CheckRole(phone);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Ok(new Result { Success = false, Message = "Lỗi Hệ thông", Data = ex.Message });
            }
        }

        [HttpGet]
        [Route("get_users")]
        public IActionResult GetAllUser(int? page, string? search)
        {
            try
            {
                var result = _userReposity.GetListUser(page,search);
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
        [Route("edit_birth")]
        public IActionResult EditBirth([FromForm] EditBirth birth)
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
        [Route("edit_image")]
        public IActionResult EditImage([FromForm] EditImage image)
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

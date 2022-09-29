using eSMP.Models;
using eSMP.Services;
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
        [Route("customersign-in")]
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
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("suppliersign-in")]
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
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("adminsign-in")]
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
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Route("user-register")]
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
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Route("supplier-register")]
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
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("check-user")]
        public IActionResult CheckUser(string phone,int roleID)
        {
            try
            {
                var result=_userReposity.CheckRole(phone,roleID);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("get-users")]
        public IActionResult GetAllUser()
        {
            try
            {
                var result = _userReposity.GetListUser();
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("Search-user")]
        public IActionResult SearchUser(string phone, int roleID)
        {
            try
            {
                var result = _userReposity.SearchUser(phone,roleID);
                if(result==null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("Update-user-status")]
        public IActionResult UpdateUserStatus(int UserID, int statusID)
        {
            try
            {
                var result = _userReposity.UpdatteUserStatus(UserID, statusID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("remove_user")]
        public IActionResult RemoveUser(int UserID)
        {
            try
            {
                var result = _userReposity.RemoveUser(UserID);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost]
        [Authorize]
        [Route("edt_address")]
        public IActionResult EditAddress(Address address)
        {
            try
            {
                var result = _userReposity.UpdateAddress(address);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

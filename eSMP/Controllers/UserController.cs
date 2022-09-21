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
    public class userController : ControllerBase
    {
        private readonly IUserReposity _userReposity;

        public userController(IUserReposity userReposity)
        {
            _userReposity = userReposity;
        }
        [HttpPost]
        [Authorize]
        [Route("usersign-in")]
        public IActionResult UserLogin(UserLogin user)
        {
            try
            {
                var result=_userReposity.LoginByPhone(user.Phone);
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
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}

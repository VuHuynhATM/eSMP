using eSMP.Models;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly WebContext _context;

        public userController(WebContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _context.Users.ToList();
            return Ok(userList);
        }
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserID == id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        [HttpPost]
        public IActionResult CreateUser(UserModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Phone = model.Phone,
                Password = model.Password,
                IsActive = model.IsActive,
                RoleID = model.RoleID,
                ImageID = model.ImageID,
                Token = model.Token,
            };
            _context.Add(user);
            _context.SaveChanges();
            return Ok(model);
        }
        [HttpPut]
        public IActionResult UpdateUser(User userUpdate)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserID == userUpdate.UserID);
            if (user == null)
                return BadRequest();
            user.UserName = userUpdate.UserName;
            user.Password = userUpdate.Password;
            user.IsActive = userUpdate.IsActive;
            user.Phone = userUpdate.Phone;
            user.Email = userUpdate.Email;
            user.RoleID = userUpdate.RoleID;
            user.ImageID = userUpdate.ImageID;
            user.Token = userUpdate.Token;
            _context.SaveChanges();
            return Ok(user);
        }
        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            var user=_context.Users.SingleOrDefault(u=>u.UserID==id);
            if (user == null)
                return NotFound();
            _context.Remove(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}

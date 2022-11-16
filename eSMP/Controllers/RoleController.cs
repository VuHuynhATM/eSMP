using eSMP.Models;
using eSMP.VModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eSMP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class roleController : ControllerBase
    {
        private readonly WebContext _context;

        public roleController(WebContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var roleList=_context.Roles.ToList();
            Result result = new Result();
            result.Success = true;
            result.Message = "Thanh cong";
            result.Data = roleList;
            return Ok(result);
        }
        [HttpGet("{id}")]
        public IActionResult GetRoleById(int id)
        {
            var role=_context.Roles.SingleOrDefault(r=>r.RoleID==id);
            if(role==null)
                return NotFound();
            return Ok(role);
        }
        [HttpPost]
        public IActionResult CreateRole(RoleModel role)
        {
            var r = new Role
            {
                RoleName = role.RoleName,
                IsActive = role.IsActive,
            };
            _context.Add(r);
            _context.SaveChanges();
            return Ok();
        }
        [HttpPut]
        public IActionResult UpdateRole(Role role)
        {
            var r = _context.Roles.SingleOrDefault(r => r.RoleID == role.RoleID);
            if (r == null)
                return NotFound();
            r.RoleName = role.RoleName;
            r.IsActive = role.IsActive;
            _context.SaveChanges();
            return Ok(role);
        }
        [HttpDelete]
        public IActionResult DeleteRoleById(int id)
        {
            var role = _context.Roles.SingleOrDefault(r => r.RoleID == id);
            if (role == null)
                return NotFound();
            _context.Remove(role);
            _context.SaveChanges();
            return Ok(role);
        }
    }
}

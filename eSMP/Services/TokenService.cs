using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eSMP.Services
{
    public class TokenService:ITokenService
    {
        private const double EXP_DURATION_MINUTES = 30;
        private readonly WebContext _context;

        public TokenService(WebContext context)
        {
            _context = context;
        }
        public string BuildToken(string key, string issuer, User user)
        {
             var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, getRoleName(user.RoleID)),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: issuer, audience: issuer, claims, expires: DateTime.Now.AddMinutes(EXP_DURATION_MINUTES), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        public string getRoleName(int RoleID)
        {
            try
            {
                var role = _context.Roles.SingleOrDefault(r => r.RoleID == RoleID);
                if (role == null)
                {
                    return null;
                }
                return role.RoleName;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

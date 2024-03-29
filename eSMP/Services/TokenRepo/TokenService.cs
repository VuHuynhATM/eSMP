﻿using eSMP.Models;
using eSMP.VModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eSMP.Services.TokenRepo
{
    public class TokenService : ITokenService
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
                new Claim(ClaimTypes.Role, user.RoleID+""),
                new Claim(ClaimTypes.NameIdentifier, user.UserID+"")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: issuer, audience: issuer, claims, expires: DateTime.Now.AddMinutes(1440), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
       
    }
}

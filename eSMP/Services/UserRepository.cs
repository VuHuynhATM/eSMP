using eSMP.Models;
using eSMP.VModels;
using Firebase.Auth;

namespace eSMP.Services
{
    public class UserRepository : IUserReposity
    {
        private readonly WebContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public UserRepository(WebContext context, IConfiguration configuration, ITokenService tokenService)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
        }
        public UserModel LoginByPhone(string phone)
        {
            CreateTokenByPhone(phone);
            var user = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(phone))
            {
                user=user.Where(x => x.Phone == phone);
            }
            var result = user.Select(u => new UserModel
            {
                UserID = u.UserID,
                Email = u.Email,
                Phone = u.Phone,
                Status = u.status.StatusName,
                Password =u.Password,
                UserName = u.UserName,
                Token = u.Token,
                Role =u.Role.RoleName,
                Image =u.Image.Path
            });
            return result.ToList()[0];
        }
        public void CreateTokenByPhone(string phone)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.Phone == phone);
                string token = _tokenService.BuildToken(_configuration["Jwt:AuthDemo:Key"], _configuration["Jwt:AuthDemo:ValidIssuer"], user);
                user.Token = token;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return;
            }
        }public void CreateTokenByEmail(string email)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == email);
                string token = _tokenService.BuildToken(_configuration["Jwt:AuthDemo:Key"], _configuration["Jwt:AuthDemo:ValidIssuer"], user);
                user.Token = token;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public UserModel LoginByEmail(string email, string password)
        {
            CreateTokenByEmail(email);
            var user = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(email)&& !string.IsNullOrEmpty(password))
            {
                user = user.Where(x => x.Phone == email && x.Password==password);
            }
            var result = user.Select(u => new UserModel
            {
                UserID = u.UserID,
                Email = u.Email,
                Phone = u.Phone,
                Status = u.status.StatusName,
                Password = u.Password,
                UserName = u.UserName,
                Token = u.Token,
                Role = u.Role.RoleName,
                Image = u.Image.Path
            });
            return result.ToList()[0];
        }
    }
}

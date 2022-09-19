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
        public void CreateToken(string phone)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.Phone == phone);
                string token = _tokenService.BuildToken(_configuration["Jwt:AuthDemo:Key"], _configuration["Jwt:AuthDemo:ValidIssuer"], user);
                user.Token = token;
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                return ;
            }
        }
        public UserModel LoginByPhone(string phone)
        {
            CreateToken(phone);
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
                IsActive = u.IsActive,
                Password =u.Password,
                UserName = u.UserName,
                Token = u.Token,
                Role =u.Role.RoleName,
                Image =u.Image.Path,
                AddressesID= u.Address.AddressID
            });
            return result.ToList()[0];
        }
    }
}

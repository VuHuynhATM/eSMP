using eSMP.Models;
using eSMP.VModels;
using User = eSMP.Models.User;

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
            var user = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(phone))
            {
                user = user.Where(x => x.Phone == phone);
            }
            if (user != null)
            {
                CreateTokenByPhone(phone);
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
            return null;
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
        }
        public void CreateTokenByEmail(string email)
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
            var user = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                user = user.Where(x => x.Phone == email && x.Password == password);
            }
            if (user != null) {
                CreateTokenByEmail(email);
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
            return null;
        }

        public UserModel RigisterUser(UserRegister user)
        {
            if (user != null)
            {
                try
                {
                    Image image = new Image();
                    image.FileName = user.ImageName;
                    image.Path = user.Imagepath;
                    image.Crete_date = DateTime.UtcNow;
                    image.IsActive = true;
                    
                    User new_user = new User();
                    new_user.UserName=user.UserName;
                    new_user.Email= user.Email;
                    new_user.Phone = user.Phone;
                    new_user.StatusID = 1;
                    new_user.RoleID = 2;
                    new_user.Token = " ";
                    new_user.Image = image;

                    Address address = new Address();
                    address.context=user.contextAddress;
                    address.latitude=user.latitude;
                    address.longitude=user.longitude;
                    address.IsActive = true;

                    User_Address user_Address = new User_Address();
                    user_Address.Address = address;
                    user_Address.User = new_user;
                    _context.User_Addresses.Add(user_Address);
                    _context.SaveChanges();

                    CreateTokenByPhone(user.Phone);
                    var usersuccess = _context.Users.AsQueryable();
                    if (!string.IsNullOrEmpty(user.Phone))
                    {
                        usersuccess = usersuccess.Where(x => x.Phone == user.Phone);
                    }
                    var result = usersuccess.Select(u => new UserModel
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
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public Boolean CheckPhone(string phone)
        {
            var user = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(phone))
            {
                user = user.Where(x => x.Phone == phone);
            }
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public UserModel RigisterSupplier(UserRegister user)
        {
            if (user != null)
            {
                try
                {
                    Image image = new Image();
                    image.FileName = user.ImageName;
                    image.Path = user.Imagepath;
                    image.Crete_date = DateTime.UtcNow;
                    image.IsActive = true;

                    User new_user = new User();
                    new_user.UserName = user.UserName;
                    new_user.Email = user.Email;
                    new_user.Phone = user.Phone;
                    new_user.StatusID = 1;
                    new_user.RoleID = 3;
                    new_user.Token = " ";
                    new_user.Image = image;

                    Address address = new Address();
                    address.context = user.contextAddress;
                    address.latitude = user.latitude;
                    address.longitude = user.longitude;
                    address.IsActive = true;

                    User_Address user_Address = new User_Address();
                    user_Address.Address = address;
                    user_Address.User = new_user;
                    _context.User_Addresses.Add(user_Address);
                    _context.SaveChanges();

                    CreateTokenByPhone(user.Phone);
                    var usersuccess = _context.Users.AsQueryable();
                    if (!string.IsNullOrEmpty(user.Phone))
                    {
                        usersuccess = usersuccess.Where(x => x.Phone == user.Phone);
                    }
                    var result = usersuccess.Select(u => new UserModel
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
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}

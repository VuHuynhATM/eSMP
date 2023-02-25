using eSMP.Models;
using eSMP.Services.FileRepo;
using eSMP.Services.TokenRepo;
using eSMP.VModels;
using Firebase.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using User = eSMP.Models.User;

namespace eSMP.Services.UserRepo
{
    public class UserRepository : IUserReposity
    {
        private readonly WebContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IFileReposity _fileReposity;
        private int PAGE_SIZE = 10;

        public UserRepository(WebContext context, IConfiguration configuration, ITokenService tokenService, IFileReposity fileReposity)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
            _fileReposity = fileReposity;
        }

        public DateTime GetVnTime()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            string vnTimeZoneKey = "SE Asia Standard Time";
            TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
            DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vnTimeZone);
            return VnTime;
        }
        public Result CustomerLogin(string phone, string? FCM_Firebase)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                if (CheckRole(phone).Success)
                {
                    var u = _context.Users.SingleOrDefault(user => user.Phone == phone && user.RoleID == 2);
                    if (u != null)
                    {
                        CreateTokenByUserID(u.UserID);
                        u.FCM_Firebase=FCM_Firebase;
                        _context.SaveChanges();
                        UserModel model = new UserModel
                        {
                            UserID = u.UserID,
                            Email = u.Email,
                            Phone = u.Phone,
                            IsActive = u.isActive,
                            Password = u.Password,
                            UserName = u.UserName,
                            Crete_date = u.Crete_date,
                            DateOfBirth = u.DateOfBirth,
                            Gender = u.Gender,
                            Token = u.Token,
                            Role = GetUserRole(u.RoleID),
                            Image = GetUserImage(u.ImageID),
                            addresses = GetAddresses(u.UserID),
                            FCM_Firebase=u.FCM_Firebase,
                            FirebaseID=u.FirebaseID,
                        };
                        if (!model.IsActive)
                        {
                            result.Success = false;
                            result.Message = "Tài khoản hiện bị khoá";
                            result.Data = "";
                            result.TotalPage = numpage;
                            return result;
                        }
                        result.Success = true;
                        result.Message = "Đăng nhập thành Công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Nhập sai số điện thoại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result SupplierLogin(string phone, string? FCM_Firebase)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                if (CheckRole(phone).Success)
                {
                    var u = _context.Users.SingleOrDefault(user => user.Phone == phone && user.RoleID == 3);
                    if (u != null)
                    {
                        CreateTokenByUserID(u.UserID);
                        u.FCM_Firebase = FCM_Firebase;
                        _context.SaveChanges();
                        var store = _context.Stores.SingleOrDefault(s => s.UserID == u.UserID);
                        int storeID = -1;
                        if (store != null)
                        {
                            storeID = store.StoreID;
                        }
                        UserModel model = new UserModel
                        {
                            UserID = u.UserID,
                            Email = u.Email,
                            Phone = u.Phone,
                            IsActive = u.isActive,
                            Password = u.Password,
                            UserName = u.UserName,
                            Crete_date = u.Crete_date,
                            DateOfBirth = u.DateOfBirth,
                            Gender = u.Gender,
                            Token = u.Token,
                            Role = GetUserRole(u.RoleID),
                            Image = GetUserImage(u.ImageID),
                            addresses = GetAddresses(u.UserID),
                            StoreID = storeID,
                            FCM_Firebase = u.FCM_Firebase,
                            FirebaseID=u.FirebaseID,
                        };
                        if (!model.IsActive)
                        {
                            result.Success = false;
                            result.Message = "Tài khoản hiện bị khoá";
                            result.Data = "";
                            result.TotalPage = numpage;
                            return result;
                        }
                        result.Success = true;
                        result.Message = "Đăng nhập thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Nhập sai số điện thoại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public void CreateTokenByUserID(int UserID)
        {
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == UserID);
                string token = _tokenService.BuildToken(_configuration["Jwt:AuthDemo:Key"], _configuration["Jwt:AuthDemo:ValidIssuer"], user);
                user.Token = token;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return;
            }
        }
        public Result LoginByEmail(string email, string password, string? FCM_Firebase)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    var u = _context.Users.SingleOrDefault(user => user.Email == email && user.Password == password && user.RoleID == 1);
                    if (u != null)
                    {
                        CreateTokenByUserID(u.UserID);
                        u.FCM_Firebase=FCM_Firebase;
                        _context.SaveChanges();
                        UserModel model = new UserModel
                        {
                            UserID = u.UserID,
                            Email = u.Email,
                            Phone = u.Phone,
                            IsActive = u.isActive,
                            Password = u.Password,
                            UserName = u.UserName,
                            DateOfBirth = u.DateOfBirth,
                            Gender = u.Gender,
                            Crete_date = u.Crete_date,
                            Token = u.Token,
                            Role = GetUserRole(u.RoleID),
                            Image = GetUserImage(u.ImageID),
                            addresses = GetAddresses(u.UserID),
                            FCM_Firebase = FCM_Firebase,
                            FirebaseID= u.FirebaseID,
                        };
                        if (!model.IsActive)
                        {
                            result.Success = false;
                            result.Message = "Tài khoản hiện bị khoá";
                            result.Data = "";
                            result.TotalPage = numpage;
                            return result;
                        }
                        result.Success = true;
                        result.Message = "Đăng nhập thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                }
                result.Success = false;
                result.Message = "Nhập sai email hoặc password";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result RigisterUser(UserRegister user)
        {
            Result result = new Result();
            int numpage = 1; 
            if (!CheckRole(user.Phone).Success)
            {
                if (user != null)
                {
                    try
                    {
                        var date = GetVnTime();
                        Image image = new Image();
                        image.FileName = user.ImageName;
                        image.Path = user.Imagepath;
                        image.Crete_date = date;
                        image.IsActive = true;

                        User new_user = new User();
                        new_user.UserName = user.UserName;
                        new_user.Email = user.Email;
                        new_user.Phone = user.Phone;
                        new_user.Crete_date = date;
                        new_user.DateOfBirth = user.DateOfBirth;
                        new_user.Gender = user.Gender;
                        new_user.isActive = true;
                        new_user.RoleID = 2;
                        new_user.Token = " ";
                        new_user.Image = image;
                        new_user.FirebaseID=user.FirebaseID;
                        new_user.FCM_Firebase= user.FCM_Firebase;

                        Address address = new Address();
                        address.UserName = user.UserName;
                        address.Phone = user.Phone;
                        address.Context = user.contextAddress;
                        address.Latitude = user.Latitude;
                        address.Longitude = user.Longitude;
                        address.Province = user.Province;
                        address.District = user.District;
                        address.Ward = user.Ward;
                        address.IsActive = true;

                        User_Address user_Address = new User_Address();
                        user_Address.Address = address;
                        user_Address.User = new_user;
                        user_Address.IsActive = true;
                        _context.User_Addresses.AddAsync(user_Address);
                        _context.SaveChangesAsync();
                        UserModel model = new UserModel();
                        CreateTokenByUserID(user_Address.UserID);
                        model = GetUserIFByID(user_Address.UserID);
                        result.Success = true;
                        result.Message = "Đăng ký thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                    catch
                    {
                        result.Success = false;
                        result.Message = "Lỗi hệ thông";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }
                }
            }
            result.Success = false;
            result.Message = "Số điện thoại đã được sử dụng";
            result.Data = "";
            result.TotalPage = numpage;
            return result;
        }
        public Result CheckRole(string phone)
        {
            Result result = new Result();
            int numpage = 1;
            if (!string.IsNullOrEmpty(phone))
            {
                var user = _context.Users.SingleOrDefault(u => u.Phone == phone);
                if (user != null)
                {
                    result.Success = true;
                    result.Message = "Số điện thoại đã được kích hoạt";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
            }
            result.Success = false;
            result.Message = "Số điện thoại chưa được kích hoạt";
            result.Data = "";
            result.TotalPage = numpage;
            return result;

        }
        public Result RigisterSupplier(UserRegister user)
        {
            Result result = new Result();
            int numpage = 1;
            if (!CheckRole(user.Phone).Success)
            {
                if (user != null)
                {
                    try
                    {
                        var date = GetVnTime();
                        Image image = new Image();
                        image.FileName = user.ImageName;
                        image.Path = user.Imagepath;
                        image.Crete_date = date;
                        image.IsActive = true;

                        User new_user = new User();
                        new_user.UserName = user.UserName;
                        new_user.Email = user.Email;
                        new_user.Phone = user.Phone;
                        new_user.Crete_date = date;
                        new_user.DateOfBirth = user.DateOfBirth;
                        new_user.Gender = user.Gender;
                        new_user.isActive = true;
                        new_user.RoleID = 3;
                        new_user.Token = " ";
                        new_user.Image = image;
                        new_user.FirebaseID = user.FirebaseID;
                        new_user.FCM_Firebase = user.FCM_Firebase;

                        Address address = new Address();
                        address.UserName = user.UserName;
                        address.Phone = user.Phone;
                        address.Context = user.contextAddress;
                        address.Latitude = user.Latitude;
                        address.Longitude = user.Longitude;
                        address.Province = user.Province;
                        address.District = user.District;
                        address.Ward = user.Ward;
                        address.IsActive = true;

                        User_Address user_Address = new User_Address();
                        user_Address.Address = address;
                        user_Address.User = new_user;
                        user_Address.IsActive = true;
                        _context.User_Addresses.AddAsync(user_Address);
                        _context.SaveChangesAsync();

                        UserModel model = new UserModel();
                        CreateTokenByUserID(user_Address.UserID);
                        model = GetUserIFByID(user_Address.UserID);
                        result.Success = true;
                        result.Message = "Đăng ký thành công";
                        result.Data = model;
                        result.TotalPage = numpage;
                        return result;
                    }
                    catch
                    {
                        result.Success = false;
                        result.Message = "Lỗi hệ thông";
                        result.Data = "";
                        result.TotalPage = numpage;
                        return result;
                    }
                }
            }
            result.Success = false;
            result.Message = "Số điện thoại đã được sử dụng";
            result.Data = "";
            result.TotalPage = numpage;
            return result;
        }
        public List<Address> GetAddresses(int userID)
        {
            try
            {
                if (userID != null)
                {
                    var user_addresses = _context.User_Addresses.Where(a => a.UserID == userID && a.Address.IsActive == true && a.IsActive == true);
                    var listaddress = user_addresses.Select(ud => new Address
                    {
                        AddressID = ud.AddressID,
                        UserName = ud.Address.UserName,
                        Phone = ud.Address.Phone,
                        Context = ud.Address.Context,
                        IsActive = ud.Address.IsActive,
                        Latitude = ud.Address.Latitude,
                        Longitude = ud.Address.Longitude,
                        Province = ud.Address.Province,
                        District = ud.Address.District,
                        Ward = ud.Address.Ward
                    });
                    return listaddress.ToList();
                }
                return new List<Address>();
            }
            catch
            {
                return null;
            }
        }
        public Result UpdatteUserStatus(int userID, bool iaActive)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == userID);
                if (user != null)
                {
                    user.isActive = iaActive;
                    _context.Update(user);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "update thành công";
                    result.Data = user;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "user không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result GetListUser(int? page, string? search, int? roleID, bool? isActive)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listuser = _context.Users.AsQueryable();
                if (search != null)
                {
                    listuser = listuser.Where(u => EF.Functions.Collate(u.UserName, "SQL_Latin1_General_CP1_CI_AI").Contains(search));
                }
                if (isActive.HasValue)
                {
                    listuser = listuser.Where(u => u.isActive == isActive);
                }
                if (roleID.HasValue)
                {
                    listuser = listuser.Where(u => u.RoleID == roleID);
                }
                if (page.HasValue)
                {
                    numpage = (int)Math.Ceiling((double)listuser.Count() / (double)PAGE_SIZE);
                    if (numpage == 0)
                    {
                        numpage = 1;
                    }
                    listuser = listuser.Skip((page.Value - 1) * PAGE_SIZE).Take(PAGE_SIZE);
                }
                var r = new List<UserModel>();
                if (listuser.Count() > 0)
                    foreach (var user in listuser.ToList())
                    {
                        var store = _context.Stores.SingleOrDefault(s => s.UserID == user.UserID);
                        var storeid = -1;
                        if (store != null)
                        {
                            storeid = store.StoreID;
                        }
                        UserModel model = new UserModel
                        {
                            UserID = user.UserID,
                            UserName = user.UserName,
                            Crete_date = user.Crete_date,
                            Email = user.Email,
                            Phone = user.Phone,
                            Token = user.Token,
                            Password = user.Password,
                            DateOfBirth = user.DateOfBirth,
                            Gender = user.Gender,
                            addresses = GetAddresses(user.UserID),
                            Image = GetUserImage(user.ImageID),
                            Role = GetUserRole(user.RoleID),
                            IsActive = user.isActive,
                            StoreID= storeid,
                            FCM_Firebase=user.FCM_Firebase,
                            FirebaseID=user.FirebaseID,
                        };
                        r.Add(model);
                    }
                result.Success = true;
                result.Message = "thành công";
                result.Data = r;
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Image GetUserImage(int imageID)
        {
            Image image = _context.Images.SingleOrDefault(i => i.ImageID == imageID);
            if (image != null)
            {
                return image;
            }
            return null;
        }
        public Role GetUserRole(int roleID)
        {
            Role role = _context.Roles.SingleOrDefault(u => u.RoleID == roleID);
            if (role != null)
            {
                return role;
            }
            return null;
        }
        public Result RemoveUser(int userID)
        {
            Result result = new Result();
            int numpage = 1;
            if (userID != null)
            {
                try
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserID == userID);
                    if (user != null)
                    {
                        var image = _context.Images.SingleOrDefault(i => i.ImageID == user.ImageID);
                        _context.Images.Remove(image);
                        List<Address> listAddress = GetAddresses(user.UserID);
                        foreach (Address address in listAddress)
                        {
                            _context.Addresss.Remove(address);
                        }
                        _context.Users.Remove(user);
                        _context.SaveChanges();
                        result.Success = true;
                        result.Message = "Remove user thành công";
                        result.Data = user;
                        result.TotalPage = numpage;
                        return result;
                    }
                }
                catch
                {
                    result.Success = false;
                    result.Message = "Lỗi hệ thống";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
            }
            result.Success = false;
            result.Message = "User không tồn tại";
            result.Data = "";
            result.TotalPage = numpage;
            return result;
        }
        public Result AddAddress(UserAddAddress address)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == address.UserID);
                if (user != null)
                {
                    Address newAddress = new Address();
                    newAddress.UserName = address.UserName;
                    newAddress.Phone = address.Phone;
                    newAddress.Context = address.contextAddress;
                    newAddress.Province = address.Province;
                    newAddress.District = address.District;
                    newAddress.Ward = address.Ward;
                    newAddress.Latitude = address.Latitude;
                    newAddress.Longitude = address.Longitude;
                    newAddress.IsActive = true;

                    User_Address user_Address = new User_Address();
                    user_Address.Address = newAddress;
                    user_Address.User = user;
                    user_Address.IsActive = true;
                    _context.User_Addresses.AddAsync(user_Address);
                    _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = "Thêm địa chỉ thành công";
                    result.Data = GetAddresses(address.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "User không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result UpdateAddress(Address address)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var addressUpdate = _context.Addresss.SingleOrDefault(a => a.AddressID == address.AddressID);
                if (addressUpdate != null)
                {
                    addressUpdate.UserName = address.UserName;
                    addressUpdate.Phone = address.Phone;
                    addressUpdate.Context = address.Context;
                    addressUpdate.Province = address.Province;
                    addressUpdate.District = address.District;
                    addressUpdate.Ward = address.Ward;
                    addressUpdate.Latitude = address.Latitude;
                    addressUpdate.Longitude = address.Longitude;
                    addressUpdate.IsActive = true;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Sửa địa chỉ thành công";
                    result.Data = addressUpdate;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "địa chỉ không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result UpdateName(EditName name)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == name.UserID);
                if (user != null)
                {
                    user.UserName = name.UserName;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thay đổi thành công";
                    result.Data = GetUserIFByID(user.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Thay đổi không thành công";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result UpdateEmail(EditEmail email)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == email.UserID);
                if (user != null)
                {
                    user.Email = email.UserEmail;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thay đổi thành công";
                    result.Data = GetUserIFByID(user.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Thay đổi không thành công";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result UpdateBirth(EditBirth birth)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == birth.UserID);
                if (user != null)
                {
                    string vnTimeZoneKey = "SE Asia Standard Time";
                    TimeZoneInfo vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById(vnTimeZoneKey);
                    DateTime VnTime = TimeZoneInfo.ConvertTimeFromUtc(birth.UserBirth, vnTimeZone);

                    user.DateOfBirth = VnTime;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thay đổi thành công";
                    result.Data = GetUserIFByID(user.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Thay đổi không thành công";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result UpdateGender(EditGender gender)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == gender.UserID);
                if (user != null)
                {
                    user.Gender = gender.UserGender;
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thay đổi thành công";
                    result.Data = GetUserIFByID(user.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Thay đổi không thành công";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public Result UpdateImage(EditImage image)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == image.UserID);
                if (user != null)
                {
                    var img = _context.Images.SingleOrDefault(i => i.ImageID == user.ImageID);
                    if (img != null)
                    {
                        var file = image.File;
                        var date = GetVnTime();
                        Guid myuuid = Guid.NewGuid();
                        string myuuidAsString = myuuid.ToString();
                        string filename = user.UserID + "-" + myuuidAsString;
                        string path = _fileReposity.UploadFile(file, filename).Result;
                        string pathDelete = img.Path;
                        string imageDelete = img.FileName;
                        if (pathDelete != "https://firebasestorage.googleapis.com/v0/b/esmp-4b85e.appspot.com/o/images%2F16-1c8843e5-4dd0-4fb7-b061-3a9fcbd68c0d?alt=media&token=0c8838a5-d3c4-4c31-82ed-d9b91d8c11d9")
                        {
                            if (_fileReposity.DeleteFileASYNC(imageDelete).Result)
                            {
                                img.Crete_date = GetVnTime();
                                img.FileName = filename;
                                img.Path = path;
                                _context.SaveChanges();
                                result.Success = true;
                                result.Message = "Thay đổi thành công";
                                result.Data = GetUserIFByID(user.UserID);
                                result.TotalPage = numpage;
                                return result;
                            }
                        }
                        else
                        {
                            img.Crete_date = GetVnTime();
                            img.FileName = filename;
                            img.Path = path;
                            _context.SaveChanges();
                            result.Success = true;
                            result.Message = "Thay đổi thành công";
                            result.Data = GetUserIFByID(user.UserID);
                            result.TotalPage = numpage;
                            return result;
                        }
                    }
                }
                result.Success = false;
                result.Message = "Tài khoản không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result RefeshToken(int userID, string token)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var user = _context.Users.SingleOrDefault(u => u.UserID == userID && u.Token == token);
                if (user != null)
                {
                    CreateTokenByUserID(userID);
                    result.Success = true;
                    result.Message = "Gia hạn thành công";
                    result.Data = GetUserIFByID(user.UserID);
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "Gia hạn không thành công";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result RemoveAddress(int addressID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var address = _context.Addresss.SingleOrDefault(a => a.AddressID == addressID);
                if (address != null)
                {
                    _context.Addresss.Remove(address);
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Xoá địa chỉ thành công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "địa chỉ không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public void Updaterole()
        {
            try
            {
                var role = _context.Roles.SingleOrDefault(s => s.RoleID == 4);
                if (role != null)
                {
                    var check = role.IsActive;
                    role.IsActive = !check;
                    _context.SaveChanges();
                }

            }
            catch
            {
                return;
            }
        }

        public Result GetAddressByID(int userID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var listaddress = GetAddresses(userID);
                if (listaddress != null)
                {
                    result.Success = true;
                    result.Message = "Thành công";
                    result.Data = listaddress;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "UserID không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result GetUserByID(int userID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var u = _context.Users.SingleOrDefault(user => user.UserID == userID);
                var store = _context.Stores.SingleOrDefault(s => s.UserID == userID);
                var storeid = -1;
                if (store != null)
                {
                    storeid = store.StoreID;
                }
                if (u != null)
                {
                    UserModel model = new UserModel
                    {
                        UserID = u.UserID,
                        Email = u.Email,
                        Phone = u.Phone,
                        IsActive = u.isActive,
                        Password = u.Password,
                        UserName = u.UserName,
                        Crete_date = u.Crete_date,
                        DateOfBirth = u.DateOfBirth,
                        Gender = u.Gender,
                        Token = u.Token,
                        Role = GetUserRole(u.RoleID),
                        Image = GetUserImage(u.ImageID),
                        addresses = GetAddresses(u.UserID),
                        StoreID = storeid,
                        FCM_Firebase = u.FCM_Firebase,
                        FirebaseID = u.FirebaseID,
                    };
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = model;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "UserID không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }
        public UserModel GetUserIFByID(int userID)
        {
            try
            {
                var u = _context.Users.SingleOrDefault(user => user.UserID == userID);
                var store = _context.Stores.SingleOrDefault(s => s.UserID == userID);
                var storeid = -1;
                if (store != null)
                {
                    storeid = store.StoreID;
                }
                if (u != null)
                {
                    UserModel model = new UserModel
                    {
                        UserID = u.UserID,
                        Email = u.Email,
                        Phone = u.Phone,
                        IsActive = u.isActive,
                        Password = u.Password,
                        UserName = u.UserName,
                        Crete_date = u.Crete_date,
                        DateOfBirth = u.DateOfBirth,
                        Gender = u.Gender,
                        Token = u.Token,
                        Role = GetUserRole(u.RoleID),
                        Image = GetUserImage(u.ImageID),
                        addresses = GetAddresses(u.UserID),
                        StoreID=storeid,
                        FCM_Firebase = u.FCM_Firebase,
                        FirebaseID = u.FirebaseID,
                    };
                    return model;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public Result Logout(int userID)
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var u = _context.Users.SingleOrDefault(user => user.UserID == userID);
                if (u != null)
                {
                    u.FCM_Firebase = null;
                    u.Token = "";
                    _context.SaveChanges();
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = "";
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "UserID không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public Result GetAdminContact()
        {
            Result result = new Result();
            int numpage = 1;
            try
            {
                var u = _context.Users.FirstOrDefault(u=>u.RoleID==1);
                if (u != null)
                {
                    result.Success = true;
                    result.Message = "Thành Công";
                    result.Data = u.FirebaseID;
                    result.TotalPage = numpage;
                    return result;
                }
                result.Success = false;
                result.Message = "UserID không tồn tại";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
            catch
            {
                result.Success = false;
                result.Message = "Lỗi hệ thống";
                result.Data = "";
                result.TotalPage = numpage;
                return result;
            }
        }

        public bool CheckUser(int userID)
        {
            try
            {
                var u = _context.Users.FirstOrDefault(u => u.UserID == userID && u.isActive);
                if (u != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

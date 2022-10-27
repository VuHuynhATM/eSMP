using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.UserRepo
{
    public interface IUserReposity
    {
        public Result CustomerLogin(string phone);
        public Result SupplierLogin(string phone);
        public Result GetUserByID(int userID);
        public Result CheckRole(string phone);
        public Result LoginByEmail(string email, string password);
        public Result RigisterUser(UserRegister user);
        public Result RigisterSupplier(UserRegister user);
        public Result UpdatteUserStatus(int userID, bool isActive);
        public Result GetListUser(int? page);
        public Result SearchUser(string phone, int roleID);
        public Result RemoveUser(int userID);
        public Result GetAddressByID(int userID);
        public Result AddAddress(UserAddAddress address);
        public Result UpdateAddress(Address address);
        public Result RemoveAddress(int addressID);
        public Result UpdateName(EditName name);
        public Result UpdateEmail(EditEmail email);
        public Result UpdateBirth(EditBirth birth);
        public Result UpdateGender(EditGender gender);
        public Result UpdateImage(EditImage image);
        public Result RefeshToken(int userID, string token);
        public void Updaterole();
        public UserModel GetUserIFByID(int userID);
    }
}

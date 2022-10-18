using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.UserRepo
{
    public interface IUserReposity
    {
        Result CustomerLogin(string phone);
        Result SupplierLogin(string phone);
        Result GetUserByID(int userID);
        Result CheckRole(string phone, int roleID);
        Result LoginByEmail(string email, string password);
        Result RigisterUser(UserRegister user);
        Result RigisterSupplier(UserRegister user);
        Result UpdatteUserStatus(int userID, bool isActive);
        Result GetListUser(int? page);
        Result SearchUser(string phone, int roleID);
        Result RemoveUser(int userID);
        Result GetAddressByID(int userID);
        Result AddAddress(UserAddAddress address);
        Result UpdateAddress(Address address);
        Result RemoveAddress(int addressID);
        Result UpdateName(EditName name);
        Result UpdateEmail(EditEmail email);
        Result UpdateBirth(EditBirth birth);
        Result UpdateGender(EditGender gender);
        Result UpdateImage(EditImage image);
        Result RefeshToken(int userID, string token);
        void Updaterole();
    }
}

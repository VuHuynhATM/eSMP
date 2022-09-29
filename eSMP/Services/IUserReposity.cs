using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services
{
    public interface IUserReposity
    {
        Result CustomerLogin(string phone);
        Result SupplierLogin(string phone);
        Result CheckRole(string phone, int roleID);
        Result LoginByEmail(string email, string password);
        Result RigisterUser(UserRegister user);
        Result RigisterSupplier(UserRegister user);
        Result UpdatteUserStatus(int userID, int statusID);
        Result GetListUser();
        Result SearchUser(string phone,int roleID);
        Result RemoveUser(int userID);
        Result AddAddress(UserAddAddress address);
        Result UpdateAddress(Address address);

    }
}

using eSMP.VModels;

namespace eSMP.Services
{
    public interface IUserReposity
    {
        UserModel LoginByPhone(string phone);
        Boolean CheckPhone(string phone);
        UserModel LoginByEmail(string email, string password);
        UserModel RigisterUser(UserRegister user);
        UserModel RigisterSupplier(UserRegister user);

    }
}

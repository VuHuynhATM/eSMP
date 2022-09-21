using eSMP.VModels;

namespace eSMP.Services
{
    public interface IUserReposity
    {
        UserModel LoginByPhone(string phone);
        UserModel LoginByEmail(string email, string password);

    }
}

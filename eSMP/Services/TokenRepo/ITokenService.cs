using eSMP.Models;
using eSMP.VModels;

namespace eSMP.Services.TokenRepo
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, User user);
    }
}

using System.Security.Principal;

namespace JWTAuthentication.IServices
{
    public interface IAuthService
    {
        string GenerateToken(string username, string password);

        IPrincipal ValidateToken(string token);


    }
}
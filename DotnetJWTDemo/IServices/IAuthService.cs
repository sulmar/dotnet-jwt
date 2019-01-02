using DotnetJWTDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace DotnetJWTDemo.IServices
{
    public interface IAuthService
    {
        string GenerateToken(string username, string password);

        IPrincipal ValidateToken(string token);


    }
}
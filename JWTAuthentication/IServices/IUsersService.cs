using JWTAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWTAuthentication.IServices
{
    public interface IUsersService
    {
        User Authenticate(string username, string password);
    }
}
using DotnetJWTDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotnetJWTDemo.IServices
{
    public interface IUsersService
    {
        User Authenticate(string username, string password);
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWTAuthentication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<Role> Roles { get; set; }

        public string Token { get; set; }

        
    }

    public class Role
    {
        public string Name { get; set; }
    }
}
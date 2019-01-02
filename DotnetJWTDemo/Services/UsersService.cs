using DotnetJWTDemo.IServices;
using DotnetJWTDemo.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace DotnetJWTDemo.Services
{
    public class UsersService : IUsersService
    {
        private readonly List<User> _users;
        private readonly IList<Role> roles;

        private string SecretKey;

        public UsersService()
        {
            SecretKey = WebConfigurationManager.AppSettings["SecretKey"];

            roles = new List<Role>
            {
                new Role { Name = "administrator" },
                new Role { Name = "manager" },
            };

            _users = new List<User>
            {
                new User { Id = 1, FirstName = "Marcin", LastName = "Sulecki", Username = "marcin", Password = "P@ssw0rd", Roles = new List<Role>(roles)
                }
            };
        }


        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            var claimsIdentity = new ClaimsIdentity(new Claim[]
             {
                    new Claim(ClaimTypes.Name, user.Username),                    
             });

            claimsIdentity.AddClaims(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name) ));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

         
         

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {                
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = credentials,
                Audience = "http://www.example.com",
                Issuer = "self"
            };

            var securityToken = tokenHandler.CreateToken(securityTokenDescriptor);
            user.Token = tokenHandler.WriteToken(securityToken);

            // remove password before returning
            user.Password = null;

            return user;
        }
    }
}
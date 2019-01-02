using DotnetJWTDemo.IServices;
using DotnetJWTDemo.Models;
using DotnetJWTDemo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DotnetJWTDemo.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult GenerateToken([FromBody] User userParam)
        {
            var user = _usersService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest("Username or password is incorrect");

            return Ok(user);
        }
    }
}
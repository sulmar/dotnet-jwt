﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DotnetJWTDemo.Filters
{
    public class JWTAuthenticationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authToken = FetchFromHeader(actionContext); // fetch authorization token from header

            var secretKey = WebConfigurationManager.AppSettings["SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            if (authToken != null)
            {

                var tokenHandler = new JwtSecurityTokenHandler();

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudiences = new string[] { "http://www.example.com" },
                    ValidIssuers = new string[] { "self" },
                    IssuerSigningKey = securityKey
                };

                try
                {

                    ClaimsPrincipal principal = tokenHandler.ValidateToken(authToken, tokenValidationParameters, out SecurityToken validatedToken);

                    HttpContext.Current.User = principal;
                }
                catch(Exception e)
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
               

            base.OnAuthorization(actionContext);
        }

        private string FetchFromHeader(HttpActionContext actionContext)
        {
            string requestToken = null;

            var authRequest = actionContext.Request.Headers.Authorization;
            if (authRequest != null)
            {
                requestToken = authRequest.Parameter;
            }

            return requestToken;
        }

      


      
    }
}
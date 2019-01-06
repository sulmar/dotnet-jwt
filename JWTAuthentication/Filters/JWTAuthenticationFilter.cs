using DotnetJWTDemo.ActionResults;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace JWTAuthentication.Filters
{
    public class JWTAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = context.Request.Headers.Authorization;

            if (authorization == null)
            {
                return;
            }

            if (authorization.Scheme != "Bearer")
            {
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            var requestToken = authorization.Parameter;

            var secretKey = WebConfigurationManager.AppSettings["SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new string[] { "http://www.example.com" },
                ValidIssuers = new string[] { "self" },
                IssuerSigningKey = securityKey
            };

            try
            {

                IPrincipal principal = tokenHandler.ValidateToken(requestToken, tokenValidationParameters, out SecurityToken validatedToken);

                SetPrincipal(principal);

            }
            catch (Exception e)
            {
                //context.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                //actionContext.Response.Headers.WwwAuthenticate.Add(new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "error=\"invalid_token\""));

            }

            
        }

        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal; //  Thread.CurrentPrincipal property is the standard way to set the thread's principal in .NET.

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal; // HttpContext.Current.User property is specific to ASP.NET.
            }
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = await context.Result.ExecuteAsync(cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer", "error=\"invalid_token\""));
            }

            context.Result = new ResponseMessageResult(result);
        }

     
    }
}
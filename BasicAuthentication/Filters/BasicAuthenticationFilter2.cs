using BasicAuthentication.ActionResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BasicAuthentication.Filters
{
    public class BasicAuthenticationFilter2 : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var authorization = actionContext.Request.Headers.Authorization;

            if (authorization == null)
            {
                return;
            }

            if (authorization.Scheme != "Basic")
            {
                return;
            }

            if (String.IsNullOrEmpty(authorization.Parameter))
            {
                Challenge(actionContext);
                return;
            }

            Tuple<string, string> tokens = ExtractUserNameAndPassword(authorization.Parameter);


            if (!OnAuthorizeUser(tokens.Item1, tokens.Item2))
            {
                Challenge(actionContext);
            }
            else
            {

                IIdentity identity = new BasicAuthenticationIdentity(tokens.Item1, tokens.Item2);

                var principal = new GenericPrincipal(identity, null);

                SetPrincipal(principal);
            }
            

            base.OnAuthorization(actionContext);
        }

        protected virtual Tuple<string, string> ExtractUserNameAndPassword(string authHeader)
        {
            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new Tuple<string, string>(tokens[0], tokens[1]);
        }

        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal; //  Thread.CurrentPrincipal property is the standard way to set the thread's principal in .NET.

            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal; // HttpContext.Current.User property is specific to ASP.NET.
            }
        }

        protected virtual bool OnAuthorizeUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return true;
        }

        protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;
            AuthenticationHeaderValue authorization = actionContext.Request.Headers.Authorization;

            if (authorization != null && authorization.Scheme == "Basic")
                authHeader = authorization.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
        }

        private void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);            
            actionContext.Response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=" + host));            
        }
    }

    public class MyBasicAuthenticationFilter : BasicAuthenticationFilter2
    {
        protected override bool OnAuthorizeUser(string username, string password)
        {
            return username == "testuser" && password == "Pass1word";
        }
    }
}
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace BasicAuthentication.Filters
{
    public class BasicAuthenticationFilter : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }

        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            HttpRequestMessage request = context.Request;            
            var authorization = context.Request.Headers.Authorization;

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
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            Tuple<string, string> tokens = ExtractUserNameAndPassword(authorization.Parameter);

            if (tokens == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                return;
            }

            if (!OnAuthorizeUser(tokens.Item1, tokens.Item2))
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }

            else
            {

                IIdentity identity = new BasicAuthenticationIdentity(tokens.Item1, tokens.Item2);

                var principal = new GenericPrincipal(identity, null);

                SetPrincipal(principal);
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

        protected virtual bool OnAuthorizeUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            return true;
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var result = await context.Result.ExecuteAsync(cancellationToken);
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=" + this.Realm));
            }
            context.Result = new ResponseMessageResult(result);
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
    }
}
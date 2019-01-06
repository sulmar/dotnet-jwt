using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Security.Infrastructure;
using TokenAuthentication.Providers;

// PM> Install-Package Microsoft.Owin.Host.SystemWeb
[assembly: OwinStartup(typeof(TokenAuthentication.Startup))]
namespace TokenAuthentication
{

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // PM> Install-Package Microsoft.Owin.Cors
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }


        private void ConfigureOAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider =  new RefreshTokenProvider()
            };

            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
    }
}
using JWTAuthentication.Filters;
using JWTAuthentication.IServices;
using JWTAuthentication.Services;
using System.Web.Http;
using Unity;
using Unity.Lifetime;

namespace DotnetJWTDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();
            container.RegisterType<IUsersService, UsersService>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new JWTAuthenticationFilter());
        }
    }
}

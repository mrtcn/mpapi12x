using System.Web.Http;
using AutoMapper;
using MovieConnections.Api.MovieConnections.Api;

namespace MovieConnections.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            Mapper.Initialize(x => {
                x.CreateMissingTypeMaps = true;
            });

            // Web API configuration and services
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            // Register Unity with Web API.
            var container = UnityConfig.GetConfiguredContainer();
            config.DependencyResolver = new UnityResolver(container);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

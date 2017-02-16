using System.Web.Mvc;
using System.Web.Routing;
using MovieConnections.Core.Mvc;
using MovieConnections.Framework.Mvc.Route;

namespace MovieConnections.Web {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRouteLowercase("customRoute", "{*customRoute}", null, new
            {
                customRoute = @"(^(?!\/dashboard|.*\.(jpe?g|png|gif|bmp|doc|docx|pdf|ppt|pptx|xls|xlsx)$)[\/\w\.-]+$)"
                    ,
                customRouteMatch = new DefaultConstraint(),
            }, new[] { "MovieConnections.Web.Controllers" }).RouteHandler = new CustomRouteHandler();


            routes.MapRoute("Default", "{controller}/{action}/{id}"
                , new {
                    controller = "Home"
                    , action = "Index"
                    , id = UrlParameter.Optional}
                , new[] { "MovieConnections.Web.Controllers" });
        }
    }
}
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MovieConnections.Core.Services;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;

namespace MovieConnections.Core.Mvc
{
    public class CustomRouteHandler : MvcRouteHandler {
        private RequestContext _requestContext;

        private string CurrentUrl => _requestContext.HttpContext.Request.RawUrl.WithoutQueryString();                    

        protected override IHttpHandler GetHttpHandler(RequestContext requestContext) {
            _requestContext = requestContext;

            var customRouteService = DependencyResolver.Current.GetService<ICustomRouteService>();
            var customRoute = customRouteService.GetActiveRoute(CurrentUrl);

            if(customRoute != null) {
                var parameters = new RouteValueDictionary();
                parameters.Add("id", customRoute.ContentId);

                SetRoute(customRoute, parameters);
            }
            else {
                RedirectPermanent(CurrentUrl);
            }
            
            return base.GetHttpHandler(_requestContext);
        }

        private void RedirectPermanent(string url) {
            _requestContext.HttpContext.Response.RedirectPermanent(url);
        }

        private void SetRoute(CustomRoute customRoute, RouteValueDictionary routeValueDictionary = null) {
            var predefinedPageInfo = customRoute.PredefinedPage.GetAttribute<PredefinedPageInfoAttribute>();

            _requestContext.RouteData.Values["controller"] = predefinedPageInfo.Controller;
            _requestContext.RouteData.Values["action"] = predefinedPageInfo.Action;

            if(routeValueDictionary == null)
                return;

            foreach (var routeValue in routeValueDictionary)
                _requestContext.RouteData.Values[routeValue.Key] = routeValue.Value;
        }
    }

    public class DefaultConstraint : IRouteConstraint
    {
        private RequestContext _requestContext;
        private string CurrentUrl => _requestContext.HttpContext.Request.RawUrl
            .WithoutQueryString().ToLowerInvariant();            

        public bool Match(HttpContextBase httpContextBase, Route route, string parameterName
            , RouteValueDictionary routeValueDictionary, RouteDirection routeDirection)
        {
            _requestContext = httpContextBase.Request.RequestContext;

            var customRouteService = DependencyResolver.Current.GetService<ICustomRouteService>();

            var matchedRoute = customRouteService.GetActiveRoute(CurrentUrl);

            return matchedRoute != null;
        }
    }
}

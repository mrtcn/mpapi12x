using System.Web.Mvc;

namespace MovieConnections.Web {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filterCollection) {
            filterCollection.Add(new HandleErrorAttribute());
            filterCollection.Add(new AuthorizeAttribute());
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MovieConnections.Core.Services;
using MovieConnections.Data.Entities;

namespace MovieConnections.Core.Localization
{
    public static class CultureHelper {

        public static IEnumerable<Culture> Cultures {
            get {
                var cultureService = DependencyResolver.Current.GetService<ICultureService>();
                return cultureService.Entities;
            }
        }

        public static Culture CurrentCulture {            
            get {
                var currentCultureSession = HttpContext.Session?["CurrentCulture"];
                var cultureService = DependencyResolver.Current.GetService<ICultureService>();

                if(currentCultureSession != null)
                    return cultureService.Entities
                        .FirstOrDefault(x => x.CultureCode == ((Culture)currentCultureSession).CultureCode);

                var defaultCulture = cultureService.Entities.FirstOrDefault(x => x.IsDefault);
                HttpContext.Session["CurrentCulture"] = defaultCulture;

                return defaultCulture;
            }
            set {
                HttpContext.Session["CurrentCulture"] = value;
            }
        }

        public static HttpContext HttpContext => HttpContext.Current;
    }
}
using System.Web.Mvc;
using System.Web.Optimization;

namespace MovieConnections.Web.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration 
    {
        public static string GetAreaName {
            get {
                return "Dashboard";                
            }
        }

        public override string AreaName => "Dashboard";

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute("ChangeCulture",
                "Dashboard/{controller}/{action}/{cultureId}/{actionName}/{controllerName}",
                //"Dashboard/{controller}/{action}/{cultureId}",
                new[] { "MovieConnections.Web.Areas.Dashboard.Controllers" });


            context.MapRoute(
                "Dashboard_default",
                "Dashboard/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] {"MovieConnections.Web.Areas.Dashboard.Controllers"
                , "MovieConnections.Web.Areas.Dashboard.Controllers.AccountControllers"
                , "MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers" }
            );

            BundleTable.EnableOptimizations = false;
            //AdminLTE CSS

            BundleTable.Bundles.Add(new StyleBundle("~/AdminLTE/all/css")
                .Include(
                    "~/Areas/Dashboard/Assets/dist/css/fixes.min.css",
                    "~/Areas/Dashboard/Assets/dist/css/main.css",
                    "~/Areas/Dashboard/Assets/bootstrap/css/bootstrap.min.css",
                    "~/Areas/Dashboard/Assets/dist/css/AdminLTE.min.css",
                    "~/Areas/Dashboard/Assets/dist/css/skins/skin-blue.min.css"
                ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Jquery/js")
                .Include(
                    "~/Areas/Dashboard/Assets/plugins/jQuery/jQuery-2.1.4.min.js"));

            BundleTable.Bundles.Add(new ScriptBundle("~/GeneralScripts/js")
                .Include(
                "~/Areas/Dashboard/Assets/dist/js/common-scripts.js"
                ));

            BundleTable.Bundles.Add(new ScriptBundle("~/AdminLTE/all/js")
                .Include(
                    "~/Areas/Dashboard/Assets/bootstrap/js/bootstrap.min.js",
                    "~/Areas/Dashboard/Assets/dist/js/app.min.js"));


            BundleTable.Bundles.Add(new StyleBundle("~/Kendo/all/css")
                .Include(
                "~/Areas/Dashboard/Assets/vendor/Kendo.UI/css/web/kendo.common.min.css",
                "~/Areas/Dashboard/Assets/vendor/Kendo.UI/css/web/kendo.flat.css"
                ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Kendo/all/js")
                .Include(
                "~/Areas/Dashboard/Assets/vendor/Kendo.UI/js/kendo.all.min.js",
                "~/Areas/Dashboard/Assets/vendor/Kendo.UI/js/cultures/kendo.culture.tr-TR.js",
                "~/Areas/Dashboard/Assets/vendor/Kendo.UI/js/messages/kendo.messages.tr-TR.js",
                "~/Areas/Dashboard/Assets/vendor/Kendo.UI/js/kendo.extendedgrid.js"
                ));

            BundleTable.Bundles.Add(new ScriptBundle("~/CkEditor/js")
                .Include(
                "~/Areas/Dashboard/Assets/Plugins/CkEditor/ckeditor.js",
                "~/Areas/Dashboard/Assets/Plugins/CkEditor/config.js"
                ));

            BundleTable.Bundles.Add(new StyleBundle("~/Plugins/css")
                .Include(
                "~/Areas/Dashboard/Assets/Plugins/icheck/square/blue.css"
                ));

            BundleTable.Bundles.Add(new ScriptBundle("~/Plugins/js")
                .Include(
                "~/Areas/Dashboard/Assets/Plugins/bootbox/bootbox.min.js",
                "~/Areas/Dashboard/Assets/Plugins/notify/notify.min.js",
                "~/Areas/Dashboard/Assets/Plugins/icheck/icheck.js",
                "~/Areas/Dashboard/Assets/Plugins/jszip/jszip.js"
                ));
        }
    }
}
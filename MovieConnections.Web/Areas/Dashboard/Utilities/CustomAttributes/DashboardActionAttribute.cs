using System.Web.Mvc;

namespace MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes {
    public class DashboardActionAttribute : ActionFilterAttribute {
        public DashboardActionAttribute(DashboardActionAttribute dashboardActionAttribute, string url = "") {
            Name = dashboardActionAttribute.Name;
            IconClass = dashboardActionAttribute.IconClass;
            SortOrder = dashboardActionAttribute.SortOrder;
            Url = url;
        }
        public DashboardActionAttribute(string name, string iconClass = "-", int sortOrder = 1)
        {
            Name = name;
            IconClass = iconClass;
            SortOrder = sortOrder;
        }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public int SortOrder { get; set; }
        public string Url { get; set; }
    }
}
using System;
using MovieConnections.Web.Areas.Dashboard.Models;

namespace MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes {
    public class DashboardControllerAttribute : Attribute {
        public DashboardControllerAttribute() {
            
        }
        public DashboardControllerAttribute(DashboardControllerAttribute controllerAttribute) {
            Name = controllerAttribute.Name;
            SortOrder = controllerAttribute.SortOrder;
            ParentControllerType = controllerAttribute.ParentControllerType;
            IconClassName = controllerAttribute.IconClassName;

        }
        public DashboardControllerAttribute(
             DashboardControllerType name
            , DashboardControllerType parentControllerType = DashboardControllerType.Independent
            , int sortOrder = 1
            , string iconClassName = "-") {
            Name = name;
            SortOrder = sortOrder;
            ParentControllerType = parentControllerType;
            IconClassName = iconClassName;
        }
        public DashboardControllerType Name { get; set; }
        public int SortOrder { get; set; }
        public DashboardControllerType ParentControllerType { get; set; }
        public string IconClassName { get; set; }
    }
}
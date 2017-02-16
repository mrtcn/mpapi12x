using System.Collections.Generic;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;

namespace MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices.Models {

    public class DashboardControllerAttributeModel {
        public DashboardControllerAttributeModel() {
            
        }
        public DashboardControllerAttributeModel(
            DashboardControllerAttribute parentControllerAttribute
            , List<DashboardControllerAttributeModel> childrenControllerAttributes
            , IEnumerable<DashboardActionAttribute> actionAttributes) {

            ParentControllerAttribute = parentControllerAttribute;
            ChildrenControllerAttributes = childrenControllerAttributes;
            ChildrenActionAttributes = actionAttributes;
        }

        public DashboardControllerAttribute ParentControllerAttribute { get; set; }
        public IEnumerable<DashboardControllerAttributeModel> ChildrenControllerAttributes { get; set; }
        public IEnumerable<DashboardActionAttribute> ChildrenActionAttributes { get; set; }        
    }
}
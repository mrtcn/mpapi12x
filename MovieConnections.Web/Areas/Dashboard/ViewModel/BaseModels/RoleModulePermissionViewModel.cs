using System.Collections.Generic;
using System.Web.Mvc;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel.BaseModels {
    public class RoleModulePermissionViewModel {
        public IEnumerable<SelectListItem> RoleSelectListItems { get; set; }
    }
}
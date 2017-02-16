using System.Collections.Generic;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel.BaseModels {
    public class DashboardPermissionModel
    {
        public DashboardPermissionModel() {
            
        }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public IEnumerable<DashboardPermissionItemModel> PermissionItems { get; set; }
    }
}
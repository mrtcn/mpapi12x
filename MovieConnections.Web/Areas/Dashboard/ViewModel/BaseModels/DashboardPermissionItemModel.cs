
namespace MovieConnections.Web.Areas.Dashboard.ViewModel.BaseModels {
    public class DashboardPermissionItemModel {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool View { get; set; }
        public bool Create { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool Export { get; set; }
    }
}
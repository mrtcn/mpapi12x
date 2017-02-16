using MovieConnections.Data.Entities.ModulePermissions;

namespace MovieConnections.Core.Model.DashboardModule {
    public class ModuleParams : IModule, IEntityParams {
        public int Id { get; set; }
        public string ModuleName { get; set; }
    }
}
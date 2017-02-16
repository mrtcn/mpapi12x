using MovieConnections.Data.Entities.ModulePermissions;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.Model.DashboardModule {
    public class ModulePermissionParams : IModulePermission, IEntityParams {
        public ModulePermissionParams() {
            
        }

        public ModulePermissionParams(dynamic model, int? userId = null, int? roleId = null) {
            Id = model.Id;
            ModuleId = model.ModuleId;
            UserId = userId;
            RoleId = roleId;
            var create = ( model.Create == true) ? Permissions.Create : Permissions.None;
            create |= (model.View == true) ? Permissions.View : Permissions.None;
            create |= (model.Edit == true) ? Permissions.Edit : Permissions.None;
            create |= (model.Delete == true) ? Permissions.Delete : Permissions.None;
            create |= (model.Export == true) ? Permissions.Export : Permissions.None;
            Permission = create;
        }
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public Permissions Permission { get; set; }
    }
}
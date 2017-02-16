using System.Data.Entity.ModelConfiguration;

namespace MovieConnections.Data.Mappings.ModulePermission {
    public class ModulePermissionMap : EntityTypeConfiguration<Entities.ModulePermissions.ModulePermission> {
        public ModulePermissionMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.Module).WithMany(x => x.ModulePermissions)
                .HasForeignKey(x => x.ModuleId);
        }
    }
}
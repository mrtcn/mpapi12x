using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities.ModulePermissions;

namespace MovieConnections.Data.Mappings.ModulePermission {
    public class ModuleMap : EntityTypeConfiguration<Module> {
        public ModuleMap() {
            HasKey(x => x.Id);
            Property(x => x.ModuleName).HasMaxLength(255);            
        }
    }
}
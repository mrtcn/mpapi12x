using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class ConnectionPathMap : EntityTypeConfiguration<ConnectionPath> {
        public ConnectionPathMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.BaseActor).WithMany(x => x.ConnectionPaths).HasForeignKey(x => x.BaseActorId);
        }
    }
}
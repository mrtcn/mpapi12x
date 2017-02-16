using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class ActorMap : EntityTypeConfiguration<Actor> {
        public ActorMap() {
            HasKey(x => x.Id);
        }
    }
}
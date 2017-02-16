using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class MovieBranchMap : EntityTypeConfiguration<MovieBranch> {
        public MovieBranchMap() {
            HasKey(x => x.Id);
        }
    }
}
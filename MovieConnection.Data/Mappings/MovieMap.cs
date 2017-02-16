using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class MovieMap : EntityTypeConfiguration<Movie> {
        public MovieMap() {
            HasKey(x => x.Id);
        }
    }

    public class MovieCultureMap : EntityTypeConfiguration<MovieCulture> {
        public MovieCultureMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.Culture).WithMany().HasForeignKey(x => x.CultureId);
            HasRequired(x => x.BaseEntity).WithMany(x => x.CulturedEntities).HasForeignKey(x => x.BaseEntityId);            
        }
    }
}
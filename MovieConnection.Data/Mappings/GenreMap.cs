using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class GenreMap : EntityTypeConfiguration<Genre> {
        public GenreMap() {
            HasKey(x => x.Id);
        }
    }

    public class GenreCultureMap : EntityTypeConfiguration<GenreCulture> {
        public GenreCultureMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.Culture).WithMany().HasForeignKey(x => x.CultureId);
            HasRequired(x => x.BaseEntity).WithMany(x => x.CulturedEntities).HasForeignKey(x => x.BaseEntityId);            
        }
    }
}
using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class CityMap : EntityTypeConfiguration<City> {
        public CityMap() {
            HasKey(x => x.Id);
        }
    }

    public class CityCultureMap : EntityTypeConfiguration<CityCulture> {
        public CityCultureMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.Culture).WithMany().HasForeignKey(x => x.CultureId);
            HasRequired(x => x.City).WithMany(x => x.CulturedEntities).HasForeignKey(x => x.BaseEntityId);
        }
    }
}
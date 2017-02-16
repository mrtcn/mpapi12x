using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class DistrictMap : EntityTypeConfiguration<District> {
        public DistrictMap() {
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255);
            HasRequired(x => x.City).WithMany(x => x.Districts).HasForeignKey(x => x.CityId);
        }
    }
}
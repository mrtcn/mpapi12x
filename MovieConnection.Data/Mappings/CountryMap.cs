using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class CountryMap : EntityTypeConfiguration<Country> {
        public CountryMap() {
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255);
        }
    }
}
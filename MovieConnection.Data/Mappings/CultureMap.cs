using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class CultureMap : EntityTypeConfiguration<Culture> {
        public CultureMap() {
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255);
            Property(x => x.CultureCode).HasMaxLength(10);
            Property(x => x.ShortName).HasMaxLength(5);
            Property(x => x.Url).HasMaxLength(50);
        }
    }
}
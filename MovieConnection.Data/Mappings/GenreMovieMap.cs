using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class GenreMovieMap : EntityTypeConfiguration<GenreMovie> {
        public GenreMovieMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.Genre).WithMany(x => x.GenreMovies).HasForeignKey(x => x.GenreId);
            HasRequired(x => x.Movie).WithMany(x => x.GenreMovies).HasForeignKey(x => x.MovieId);
        }
    }
}
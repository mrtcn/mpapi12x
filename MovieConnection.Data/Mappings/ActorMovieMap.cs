using System.Data.Entity.ModelConfiguration;
using MovieConnections.Data.Entities;

namespace MovieConnections.Data.Mappings {
    public class ActorMovieMap : EntityTypeConfiguration<ActorMovie> {
        public ActorMovieMap() {
            HasKey(x => x.Id);
            HasRequired(x => x.Actor).WithMany(x => x.ActorMovies).HasForeignKey(x => x.ActorId);
            HasRequired(x => x.Movie).WithMany(x => x.ActorMovies).HasForeignKey(x => x.MovieId);            
        }
    }
}
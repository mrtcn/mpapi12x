using MovieConnections.Core.BaseServices;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IMovieService : ICulturedBaseService<Movie, MovieCulture, MovieParams> {
        
    }

    public class MovieService : CulturedBaseService<Movie, MovieCulture, MovieParams>, IMovieService {
        private readonly IGenreMovieService _genreMovieService;
        private readonly IActorMovieService _actorMovieService;

        public MovieService(
            IGenreMovieService genreMovieService, 
            IActorMovieService actorMovieService,
            IRepository<Movie> repository,
            IRepository<MovieCulture> culturedRepository ) : base(culturedRepository, repository) {
            _genreMovieService = genreMovieService;
            _actorMovieService = actorMovieService;
        }

        protected override void OnRemoved(MovieParams entityParams) {
            base.OnRemoved(entityParams);

            _actorMovieService.DeleteActorMovies(entityParams.Id);
            _genreMovieService.DeleteGenreMovies(entityParams.Id);
        }
    }
}

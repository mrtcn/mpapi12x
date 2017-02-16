using System;
using System.Collections.Generic;
using System.Linq;
using MovieConnections.Core.BaseServices;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IGenreService : ICulturedBaseService<Genre, GenreCulture, GenreParams>
    {
        void CreateGenres(List<string> categories, int movieId);
        void CreateGenresByIds(IEnumerable<int> genreIds, int movieId);
    }

    public class GenreService : CulturedBaseService<Genre, GenreCulture, GenreParams>, IGenreService {
        private readonly IGenreMovieService _genreMovieService;
        private readonly IRepository<GenreMovie> _genreMovieRepository;

        public GenreService(
            IGenreMovieService genreMovieService, 
            IRepository<GenreMovie> genreMovieRepository,
            IRepository<Genre> repository,
            IRepository<GenreCulture> culturedRepository ) : base(culturedRepository, repository) {
            _genreMovieService = genreMovieService;
            _genreMovieRepository = genreMovieRepository;
        }

        protected override void OnRemoved(GenreParams entityParams)
        {
            base.OnRemoved(entityParams);

            _genreMovieService.DeleteGenreMovies(entityParams.Id);
        }

        public void CreateGenres(List<string> categories,  int movieId) {
            foreach (var genre in categories) {
                int genreId;
                CreateIfNotExists(genre, out genreId);

                var genreMovieParams = new GenreMovieParams();
                genreMovieParams.GenreId = genreId;
                genreMovieParams.MovieId = movieId;
                genreMovieParams.Status = Status.Active;
                genreMovieParams.UserType = UserTypes.Dashboard;

                _genreMovieService.CreateOrUpdate(genreMovieParams);
            }
        }

        public void CreateGenresByIds(IEnumerable<int> genreIds, int movieId)
        {
            _genreMovieService.DeleteGenreMovies(movieId);

            foreach (var genreId in genreIds)
            {
                var genre = GetCulturedEntity(genreId);
                int genreCreatedId;
                CreateIfNotExists(genre.Name, out genreCreatedId);

                var genreMovieParams = new GenreMovieParams();
                genreMovieParams.GenreId = genreId;
                genreMovieParams.MovieId = movieId;
                genreMovieParams.Status = Status.Active;
                genreMovieParams.UserType = UserTypes.Dashboard;

                _genreMovieService.CreateOrUpdate(genreMovieParams);
            }
        }

        public bool CreateIfNotExists(string genre, out int id) {
            var genreEntity = CulturedEntities.FirstOrDefault(x => x.Name == genre);
            id = genreEntity?.BaseEntityId ?? 0;
            if (id != 0)
                return false;

            var genreParams = new GenreParams();
            genreParams.Name = genre;
            genreParams.UserType = UserTypes.Dashboard;
            genreParams.Status = Status.Active;

            genreParams = CreateOrUpdate(genreParams);
            id = genreParams.BaseEntityId;

            return true;
        }
    }
}

using System;
using System.Linq;
using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IGenreMovieService : IBaseService<GenreMovie> {
        void DeleteGenreMovies(int id);
    }

    public class GenreMovieService : BaseService<GenreMovie>, IGenreMovieService {
        private readonly IRepository<GenreMovie> _repository;

        public GenreMovieService(IRepository<GenreMovie> repository) : base(repository) {
            _repository = repository;
        }

        protected override void OnRemoved(int id)
        {
            base.OnRemoved(id);

            DeleteGenreMovies(id);
        }

        public void DeleteGenreMovies(int id)
        {
            var genreMovies = Entities.Where(x => x.MovieId == id);

            foreach (var genreMovie in genreMovies)
            {
                genreMovie.Status = Status.Deleted;
                genreMovie.ModificationDate = DateTime.Now;
                
                _repository.Update(genreMovie);
            }
            _repository.SaveChanges();
        }
    }
}

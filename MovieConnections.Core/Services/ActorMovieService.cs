using System;
using System.Linq;
using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IActorMovieService : IBaseService<ActorMovie> {
        void DeleteActorMovies(int id);
    }

    public class ActorMovieService : BaseService<ActorMovie>, IActorMovieService {
        private readonly IRepository<ActorMovie> _repository;

        public ActorMovieService(IRepository<ActorMovie> repository) : base(repository) {
            _repository = repository;
        }

        protected override void OnRemoved(int id)
        {
            base.OnRemoved(id);

            DeleteActorMovies(id);
        }

        public void DeleteActorMovies(int id)
        {
            var actorMovies = Entities.Where(x => x.MovieId == id);

            foreach (var actorMovie in actorMovies)
            {
                actorMovie.Status = Status.Deleted;
                actorMovie.ModificationDate = DateTime.Now;

                _repository.Update(actorMovie);
            }
            _repository.SaveChanges();
        }
    }
}

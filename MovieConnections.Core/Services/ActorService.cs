using System;
using System.Collections.Generic;
using System.Linq;
using MovieConnections.Core.BaseServices;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IActorService : IBaseService<Actor>
    {
        void CreateActors(List<ActorModel> actors, int movieId);
        void CreateActorsById(IEnumerable<int> actorIds, int movieId);
    }

    public class ActorService : BaseService<Actor>, IActorService {
        private readonly IActorMovieService _actorMovieService;
        private readonly IRepository<ActorMovie> _actorMovieRepository;

        public ActorService(
            IActorMovieService actorMovieService, 
            IRepository<ActorMovie> actorMovieRepository, 
            IRepository<Actor> repository) : base(repository) {
            _actorMovieService = actorMovieService;
            _actorMovieRepository = actorMovieRepository;
        }

        protected override void OnRemoved(int id)
        {
            base.OnRemoved(id);

            _actorMovieService.DeleteActorMovies(id);
        }

        public void CreateActors(List<ActorModel> actors, int movieId)
        {
            foreach (var actor in actors)
            {
                int actorId;
                CreateIfNotExists(actor.Name, out actorId);

                var actorMovieParams = new ActorMovieParams();
                actorMovieParams.ActorId = actorId;
                actorMovieParams.MovieId = movieId;
                actorMovieParams.CharacterName = actor.CharacterName;
                actorMovieParams.IsStar = actor.IsStar;
                actorMovieParams.Status = Status.Active;
                actorMovieParams.UserType = UserTypes.Dashboard;

                _actorMovieService.CreateOrUpdate(actorMovieParams);
            }
        }

        public void CreateActorsById(IEnumerable<int> actorIds, int movieId)
        {
            _actorMovieService.DeleteActorMovies(movieId);

            foreach (var actorId in actorIds)
            {
                var actor = Get(actorId);
                int actorCreatedId;
                CreateIfNotExists(actor.Name, out actorCreatedId);

                var actorMovieParams = new ActorMovieParams();
                actorMovieParams.ActorId = actorId;
                actorMovieParams.MovieId = movieId;
                //actorMovieParams.CharacterName = actor.CharacterName;
                //actorMovieParams.IsStar = actor.IsStar;
                actorMovieParams.Status = Status.Active;
                actorMovieParams.UserType = UserTypes.Dashboard;

                _actorMovieService.CreateOrUpdate(actorMovieParams);
            }
        }

        public bool CreateIfNotExists(string actor, out int actorId)
        {
            var actorEntity = Entities.FirstOrDefault(x => x.Name == actor);
            actorId = actorEntity?.Id ?? 0;
            if (actorId != 0)
                return false;

            var actorParams = new ActorParams();
            actorParams.Name = actor;
            actorParams.UserType = UserTypes.Dashboard;
            actorParams.Status = Status.Active;

            actorEntity = CreateOrUpdate(actorParams);
            actorId = actorEntity.Id;

            return true;
        }

    }
}

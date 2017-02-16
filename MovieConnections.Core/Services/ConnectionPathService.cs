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
    public interface IConnectionPathService : IBaseService<ConnectionPath> {
        void DeleteConnectionPaths(int id);
        bool ConnectionItemCheck(GetConnectionPathsModel model);
        List<ActorMovieConnection> GetConnectionPaths(List<string[]> actorList, List<string[]> movieList, string cultureCode);
    }

    public class ConnectionPathService : BaseService<ConnectionPath>, IConnectionPathService {
        private readonly IRepository<ConnectionPath> _repository;
        private readonly IActorMovieService _actorMovieService;

        public ConnectionPathService(
            IRepository<ConnectionPath> repository, 
            IActorMovieService actorMovieService) : base(repository)
        {
            _repository = repository;
            _actorMovieService = actorMovieService;
        }

        protected override void OnRemoved(int id)
        {
            base.OnRemoved(id);

            DeleteConnectionPaths(id);
        }

        public void DeleteConnectionPaths(int id)
        {
            var connectionPaths = Entities.Where(x => x.Id == id);

            foreach (var connectionPath in connectionPaths)
            {
                connectionPath.Status = Status.Deleted;
                connectionPath.ModificationDate = DateTime.Now;

                _repository.Update(connectionPath);
            }
            _repository.SaveChanges();
        }

        public bool ConnectionItemCheck(GetConnectionPathsModel model)
        {
            var connectionPaths = Entities
                .Where(x => x.BaseActorId == model.BaseActorId && x.DestinationActorId == model.DestinationActorId
                && x.Status == Status.Active).ToList();

            var actorList = connectionPaths.SelectMany(x => x.ActorPath.Replace(" ", "").Split(',')).ToList();
            var movieList = connectionPaths.SelectMany(x => x.MoviePath.Replace(" ", "").Split(',')).ToList();

            var result = false;
            for (int index = 0; index < model.ConnectionPathItems.Count; index++)
            {
                result = model.ConnectionPathItems[index].MovieId == Int32.Parse(movieList[index]);
                if (!result)
                    break;

                result = model.ConnectionPathItems[index].ActorId == Int32.Parse(actorList[index]);
                if (!result)
                    break;
            }

            return result;
        }

        public List<ActorMovieConnection> GetConnectionPaths(List<string[]> actorList, List<string[]> movieList, string cultureCode)
        {
            var actorMovieConnections = new List<ActorMovieConnection>();
            for (int outerIndex = 0; outerIndex < actorList.Count; outerIndex++)
            {
                var actorMovieConnection = new ActorMovieConnection { ActorMovieInfo = new List<ActorMoviePath>() };
                var actorMoviePathList = new List<ActorMoviePath>();
                for (int innerIndex = 0; innerIndex < actorList[outerIndex].ToList().Count; innerIndex++)
                {
                    var movieIdString = movieList[outerIndex][innerIndex];
                    int movieId;
                    string movieName = string.Empty;

                    if (Int32.TryParse(movieIdString, out movieId))
                    {
                        movieName =
                            _actorMovieService.Entities.FirstOrDefault(x => x.MovieId == movieId)?
                                .Movie?.CulturedEntities?.FirstOrDefault(
                                    x => x.Culture.CultureCode == cultureCode && x.Status == Status.Active)?.Title;
                    }

                    var actorIdString = actorList[outerIndex][innerIndex];
                    int actorId;
                    string actorName = string.Empty;

                    if (Int32.TryParse(actorIdString, out actorId))
                    {
                        actorName =
                            _actorMovieService.Entities.FirstOrDefault(x => x.ActorId == actorId)?
                                .Actor?.Name;
                    }

                    var movieInfo = new MovieInfo(movieId, movieName);
                    var actorInfo = new ActorInfo(actorId, actorName);

                    var actorMoviePath = new ActorMoviePath(actorInfo, movieInfo);
                    actorMoviePathList.Add(actorMoviePath);

                }
                actorMovieConnection.ActorMovieInfo.AddRange(actorMoviePathList);
                actorMovieConnections.Add(actorMovieConnection);
            }
            return actorMovieConnections;
        }
    }
}

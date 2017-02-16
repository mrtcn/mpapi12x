using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNet.Identity;
using MovieConnections.Api.Models;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Services;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using Newtonsoft.Json;

namespace MovieConnections.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Movie")]
    public class MovieController : ApiController {
        private readonly IMovieService _movieService;
        private readonly IActorService _actorService;
        private readonly IActorMovieService _actorMovieService;
        private readonly IConnectionPathService _connectionPathService;
        private readonly ICultureService _cultureService;

        public MovieController(
            IMovieService movieService, 
            IConnectionPathService connectionPathService, 
            IActorMovieService actorMovieService, 
            ICultureService cultureService, 
            IActorService actorService) {
            _movieService = movieService;
            _connectionPathService = connectionPathService;
            _actorMovieService = actorMovieService;
            _cultureService = cultureService;
            _actorService = actorService;
        }
        [Route("GetMovies"), HttpPost]
        public IHttpActionResult GetMovies(GetMoviesModel model)
        {
            var movies = _movieService.CulturedEntities
                .Where(x => x.Title.StartsWith(model.SearchText) && x.Status == Status.Active)
                .Select(x => new { id = x.Id, title = x.Title, gameState = 2}).ToList();

            var moviesJson = JsonConvert.SerializeObject(movies);
            return Ok(moviesJson);
        }

        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("GetActors"), HttpPost]
        public IHttpActionResult GetActors(GetActorsModel model)
        {
            var movieIdValue = model.MovieId.GetValueOrDefault();
            var actors = _actorMovieService.Entities
                .Where(x => x.MovieId == (movieIdValue == 0 ? x.MovieId : movieIdValue) 
                && x.Actor.Name.StartsWith(model.SearchText) && x.Status == Status.Active)
                .Select(x => new { id = x.Actor.Id, title = x.Actor.Name, gameState = 1 }).Distinct().ToList();
            var actorsJson = JsonConvert.SerializeObject(actors);
            return Ok(actorsJson);
        }

        [Route("CheckConnectionPath"), HttpPost]
        public IHttpActionResult CheckConnectionPath(GetConnectionPathsModel model)
        {
            var result = _connectionPathService.ConnectionItemCheck(model);
            return Ok(result);
        }

        [Route("GetConnectionPaths"), HttpPost]
        public IHttpActionResult GetConnectionPaths()
        {
            var randomConnectionPath = _connectionPathService.Entities.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            var connectionPaths = _connectionPathService.Entities
                .Where(x => x.BaseActorId == randomConnectionPath.BaseActorId 
                && x.DestinationActorId == randomConnectionPath.DestinationActorId && x.Status == Status.Active).ToList();

            var cultureCode = _cultureService.Entities.FirstOrDefault(x => x.IsDefault)?.CultureCode;

            var connectionPathParams = Mapper.Map<List<ConnectionPath>, List<ConnectionPathParams>>(connectionPaths);

            var actorList = connectionPathParams
                .Where(x => !string.IsNullOrEmpty(x.ActorPath))
                .Select(x => x.ActorPath.Split(new [] {',', ' '}, StringSplitOptions.RemoveEmptyEntries)).ToList();

            var movieList = connectionPathParams
                .Where(x => !string.IsNullOrEmpty(x.MoviePath))
                .Select(x => x.MoviePath.Trim(' ').Split(new[] {',',' '}, StringSplitOptions.RemoveEmptyEntries)).ToList();

            var actorMovieDictionaryList = _connectionPathService.GetConnectionPaths(actorList, movieList, cultureCode);
            
            var jsonConnectionPaths = JsonConvert.SerializeObject(connectionPathParams
                .Select(x => new GameInit {
                    BaseActorId = x.BaseActorId,
                    BaseActorName = _actorService.Get(x.BaseActorId)?.Name,
                    DestinationActorId = x.DestinationActorId,
                    DestinationActorName = _actorService.Get(x.DestinationActorId)?.Name,
                    ActorMovieConnections = actorMovieDictionaryList,
                    MaxBranchLevel = x.MaxBranchLevel
                }).FirstOrDefault());
            return Ok(jsonConnectionPaths);
        }

    }
}

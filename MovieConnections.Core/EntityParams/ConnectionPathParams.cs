using System.Collections.Generic;
using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class ConnectionPathParams : IConnectionPath, IEntityParams {
        public int Id { get; set; }
        public int BaseActorId { get; set; }
        public int DestinationActorId { get; set; }

        public string ActorPath { get; set; }
        public string MoviePath { get; set; }
        public int MaxBranchLevel { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

    }

    public class ActorMovieConnection
    {
        public List<ActorMoviePath> ActorMovieInfo { get; set; }
    }

    public class ActorMoviePath
    {
        public ActorInfo ActorInfo { get; set; }
        public MovieInfo MovieInfo { get; set; }

        public ActorMoviePath(ActorInfo actorInfo, MovieInfo movieInfo)
        {
            ActorInfo = actorInfo;
            MovieInfo = movieInfo;
        }
    }

    public class ActorInfo
    {
        public int ActorId { get; set; }
        public string ActorName { get; set; }

        public ActorInfo(int actorId, string actorName)
        {
            ActorId = actorId;
            ActorName = actorName;
        }
    }

    public class MovieInfo
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; }

        public MovieInfo(int movieId, string movieName)
        {
            MovieId = movieId;
            MovieName = movieName;
        }
    }

    public class GetConnectionPathsModel
    {
        public int BaseActorId { get; set; }
        public string BaseActorName { get; set; }
        public int DestinationActorId { get; set; }
        public string DestinationActorName { get; set; }
        public List<ConnectionPathItem> ConnectionPathItems { get; set; }
    }

    public class GameInit
    {
        public int BaseActorId { get; set; }
        public string BaseActorName { get; set; }
        public int DestinationActorId { get; set; }
        public string DestinationActorName { get; set; }
        public int MaxBranchLevel { get; set; }
        public List<ActorMovieConnection> ActorMovieConnections { get; set; }
    }

    public class ConnectionPathItem
    {
        public int ActorId { get; set; }
        public int MovieId { get; set; }
    }
}
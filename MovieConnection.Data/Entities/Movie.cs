using System;
using System.Collections.Generic;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IMovie : IHasStatus, IEntity, ITracingFieldsModel {
        string OriginalTitle { get; set; }
        string Director { get; set; }
        double Rating { get; set; }
        int NumberOfVotes { get; set; }
        DateTime? Year { get; set; }
        MovieType MovieType { get; set; }
    }
    public class Movie : TracingDateModel, IMovie, ICulturedCollection<MovieCulture> {
        public int Id { get; set; }
        public string OriginalTitle { get; set; }
        public string Director { get; set; }        
        public double Rating { get; set; }
        public int NumberOfVotes { get; set; }
        public DateTime? Year { get; set; }
        public MovieType MovieType { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public virtual ICollection<MovieCulture> CulturedEntities { get; set; }
        public virtual ICollection<ActorMovie> ActorMovies { get; set; }
        public virtual ICollection<GenreMovie> GenreMovies { get; set; }
    }

    public interface IMovieCulture : IEntity, ITracingFieldsModel, IBaseEntityId, ICulturedEntity, IHasStatus {
        string Title { get; set; }
        string Country { get; set; }
    }

    public class MovieCulture : TracingDateModel, IMovieCulture {
        public int Id { get; set; }
        public int BaseEntityId { get; set; }
        public int CultureId { get; set; }
        public string Title { get; set; }
        public string Country { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public Culture Culture { get; set; }
        public virtual Movie BaseEntity { get; set; }
    }
}
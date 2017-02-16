using System;
using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class MovieParams : IMovie, ICulturedEntityParams
    {
        public int Id { get; set; }
        public int BaseEntityId { get; set; }
        public int RelatedEntityId { get; set; }
        public string OriginalTitle { get; set; }
        public string Director { get; set; }
        public double Rating { get; set; }
        public int NumberOfVotes { get; set; }
        public DateTime? Year { get; set; }
        public MovieType MovieType { get; set; }

        public string Title { get; set; }
        public string Country { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }       

        public Status Status { get; set; }
    }
}
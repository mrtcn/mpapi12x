using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class GenreMovieParams : IGenreMovie, IEntityParams {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

    }
}
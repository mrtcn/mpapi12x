using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class ActorMovieParams : IActorMovie, IEntityParams {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ActorId { get; set; }

        public string CharacterName { get; set; }
        public bool IsStar { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

    }
}
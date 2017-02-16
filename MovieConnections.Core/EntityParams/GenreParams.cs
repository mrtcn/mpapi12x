using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class GenreParams : IGenre, ICulturedEntityParams
    {
        public int Id { get; set; }
        public int BaseEntityId { get; set; }
        public int RelatedEntityId { get; set; }

        public string Name { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }       

        public Status Status { get; set; }
    }
}
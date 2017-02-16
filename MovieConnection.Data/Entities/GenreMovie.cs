
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IGenreMovie : IEntity, ITracingFieldsModel, IHasStatus {

    }

    public class GenreMovie : TracingDateModel, IGenreMovie {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual Movie Movie { get; set; }
    }
}

using System.Collections.Generic;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IGenre : IEntity, IHasStatus, ITracingFieldsModel
    {
        
    }

    public class Genre : TracingDateModel, IGenre, ICulturedCollection<GenreCulture>
    {
        public int Id { get; set; }
        
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public virtual ICollection<GenreCulture> CulturedEntities { get; set; }
        public virtual ICollection<GenreMovie> GenreMovies { get; set; }
    }

    public interface IGenreCulture : IEntity, ITracingFieldsModel, ICulturedEntity, IBaseEntityId, IHasStatus {
        string Name { get; set; }
    }

    public class GenreCulture : TracingDateModel, IGenreCulture {
        public int Id { get; set; }
        public int BaseEntityId { get; set; }
        public int CultureId { get; set; }

        public string Name { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public Culture Culture { get; set; }
        public virtual Genre BaseEntity { get; set; }
    }
}

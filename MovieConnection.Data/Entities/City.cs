using System.Collections.Generic;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities {
    public interface ICity : IEntity, IHasStatus, ITracingFieldsModel {
        bool IsOverpopulated { get; set; }
    }

    public class City : TracingDateModel, ICity, ICulturedCollection<CityCulture>
    {
        public int Id { get; set; }
        public bool IsOverpopulated { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }
        public virtual ICollection<CityCulture> CulturedEntities { get; set; } 
        public virtual ICollection<District> Districts { get; set; } 
    }

    public interface ICityCulture : IEntity, IHasStatus, IBaseEntityId, ICulturedEntity, ITracingFieldsModel
    {
        string Name { get; set; }
    }

    public class CityCulture : TracingDateModel, ICityCulture {
        public int Id { get; set; }
        public int BaseEntityId { get; set; }
        public string Name { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Culture Culture { get; set; }
        public int CultureId { get; set; }

        public Status Status { get; set; }
        public virtual City City { get; set; }
    }
}
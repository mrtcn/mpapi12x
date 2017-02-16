using System.Collections.Generic;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IActor : IEntity, ITracingFieldsModel, IHasStatus
    {
        string Name { get; set; }
    }

    public class Actor : TracingDateModel, IActor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public virtual ICollection<ActorMovie> ActorMovies { get; set; }
        public virtual ICollection<ConnectionPath> ConnectionPaths { get; set; }
    }
}

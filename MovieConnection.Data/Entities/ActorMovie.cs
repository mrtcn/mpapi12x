using System.Collections.Generic;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IActorMovie : IEntity, ITracingFieldsModel, IHasStatus {
        string CharacterName { get; set; }
    }

    public class ActorMovie : TracingDateModel, IActorMovie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ActorId { get; set; }

        public string CharacterName { get; set; }
        public bool IsStar { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public virtual Actor Actor { get; set; }
        public virtual Movie Movie { get; set; }        
    }
}

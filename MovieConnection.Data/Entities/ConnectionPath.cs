using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IConnectionPath : IEntity, ITracingFieldsModel, IHasStatus
    {
        int BaseActorId { get; set; }
        int DestinationActorId { get; set; }

        string ActorPath { get; set; }
        string MoviePath { get; set; }
        int MaxBranchLevel { get; set; }
    }

    public class ConnectionPath : TracingDateModel, IConnectionPath
    {
        public int Id { get; set; }
        public int BaseActorId { get; set; }
        public int DestinationActorId { get; set; }

        public string ActorPath { get; set; }
        public string MoviePath { get; set; }
        public int MaxBranchLevel { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }

        public virtual Actor BaseActor { get; set; }
    }
}

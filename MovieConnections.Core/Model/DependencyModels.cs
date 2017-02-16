using MovieConnections.Data.Models;

namespace MovieConnections.Core.Model
{
    public class DependencyEntity : IEntity, IHasStatus {
        public int Id { get; set; }
        public Status Status { get; set; }
    }

    public class DependencyCulturedEntity : IEntity, ICulturedEntity, IBaseEntityId, IHasStatus {
        public int Id { get; set; }
        public int BaseEntityId { get; set; }
        public int CultureId { get; set; }
        public Status Status { get; set; }
    }
}
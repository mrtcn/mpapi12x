using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface IMovieBranch : IEntity, ITracingFieldsModel, IHasStatus
    {
        string Name { get; set; }
    }

    public class MovieBranch : TracingDateModel, IMovieBranch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public UserTypes UserType { get; set; }

        public Status Status { get; set; }
    }
}

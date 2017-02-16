using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IMovieBranchService : IBaseService<MovieBranch>
    {
    }

    public class MovieBranchService : BaseService<MovieBranch>, IMovieBranchService {

        public MovieBranchService(
            IRepository<MovieBranch> repository) : base(repository) {
        }
        
    }
}

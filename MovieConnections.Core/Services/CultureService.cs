using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface ICultureService : IBaseService<Culture> {
        
    }

    public class CultureService : BaseService<Culture>, ICultureService {

        public CultureService(IRepository<Culture> repository) : base(repository) {
        }

        protected override void OnRemoving(int id) {
            base.OnRemoved(id);
        }
    }
}

using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface IDistrictService : IBaseService<District> {
        
    }

    public class DistrictService : BaseService<District>, IDistrictService {
        private readonly IRepository<District> _repository;

        public DistrictService(IRepository<District> repository) : base(repository) {
            _repository = repository;
        }
    }
}

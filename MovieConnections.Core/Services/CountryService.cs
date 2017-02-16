using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services
{
    public interface ICountryService : IBaseService<Country> {
        
    }

    public class CountryService : BaseService<Country>, ICountryService {
        private readonly IRepository<Country> _repository;

        public CountryService(IRepository<Country> repository) : base(repository) {
            _repository = repository;
        }
    }
}

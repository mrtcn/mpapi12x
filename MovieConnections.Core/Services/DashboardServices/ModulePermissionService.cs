using MovieConnections.Core.BaseServices;
using MovieConnections.Data.Entities.ModulePermissions;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Core.Services.DashboardServices
{
    public interface IModulePermissionService : IBaseService<ModulePermission> {
        
    }

    public class ModulePermissionService : BaseService<ModulePermission>, IModulePermissionService
    {
        private readonly IRepository<ModulePermission> _repository;

        public ModulePermissionService(IRepository<ModulePermission> repository) : base(repository) {
            _repository = repository;
        }
    }
}

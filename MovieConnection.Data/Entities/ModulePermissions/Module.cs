using System.Collections.Generic;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities.ModulePermissions
{
    public interface IModule : IEntity
    {
        string ModuleName { get; set; }
    }
    public class Module : IModule {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public virtual ICollection<ModulePermission> ModulePermissions { get; set; }
    }
}

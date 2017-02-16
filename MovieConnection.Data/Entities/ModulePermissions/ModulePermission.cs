using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities.ModulePermissions
{
    public interface IModulePermission : IEntity
    {
        int ModuleId { get; set; }
        int? UserId { get; set; }
        int? RoleId { get; set; }
        Permissions Permission { get; set; }

    }
    public class ModulePermission : IModulePermission
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
        public Permissions Permission { get; set; }
        public virtual Module Module { get; set; }
    }
}

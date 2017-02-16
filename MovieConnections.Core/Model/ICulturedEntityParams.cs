using MovieConnections.Data.Models;

namespace MovieConnections.Core.Model
{
    public interface ICulturedEntityParams : IEntityParams {
        int BaseEntityId { get; set; }
        Status Status { get; set; }
    }
}

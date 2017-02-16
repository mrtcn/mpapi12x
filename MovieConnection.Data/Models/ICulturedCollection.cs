
using System.Collections.Generic;

namespace MovieConnections.Data.Models
{
    public interface ICulturedCollection<TCulturedEntity> {
        ICollection<TCulturedEntity> CulturedEntities { get; set; }
    }
}
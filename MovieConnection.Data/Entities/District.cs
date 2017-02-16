using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities {
    public interface IDistrict : IEntity, IHasStatus
    {
        string Name { get; set; }
        bool IsOverpopulated { get; set; }
    }

    public class District : IDistrict{
        public int Id { get; set; }
        public int CityId { get; set; }
        public string Name { get; set; }
        public bool IsOverpopulated { get; set; }
        public Status Status { get; set; }
        public City City { get; set; }
    }
}
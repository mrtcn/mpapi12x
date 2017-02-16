using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities {
    public interface ICountry : IEntity, IHasStatus
    {
        string Name { get; set; }
        bool IsOverpopulated { get; set; }
    }

    public class Country : ICountry{
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOverpopulated { get; set; }
        public Status Status { get; set; }
    }
}
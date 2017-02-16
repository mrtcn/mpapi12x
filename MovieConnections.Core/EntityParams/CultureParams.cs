using MovieConnections.Core.Model;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.EntityParams {
    public class CultureParams : ICulture, IEntityParams {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CultureCode { get; set; }
        public string ShortName { get; set; }
        public string FlagImagePath { get; set; }
        public string Url { get; set; }
        public bool IsDefault { get; set; }
        public Status Status { get; set; }
    }
}
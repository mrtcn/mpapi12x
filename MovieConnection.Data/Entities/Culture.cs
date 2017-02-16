using System;
using MovieConnections.Data.Models;

namespace MovieConnections.Data.Entities
{
    public interface ICulture : IEntity {
        string Name { get; set; }
        string CultureCode { get; set; }
        string ShortName { get; set; }
        string FlagImagePath { get; set; }
        string Url { get; set; }
        bool IsDefault { get; set; }
    }

    [Serializable]
    public class Culture : ICulture {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CultureCode { get; set; }
        public string ShortName { get; set; }
        public string FlagImagePath { get; set; }
        public string Url { get; set; }
        public bool IsDefault { get; set; }
    }
}
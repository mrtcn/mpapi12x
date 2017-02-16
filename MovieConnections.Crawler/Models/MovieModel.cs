using System;

namespace MovieConnections.Crawler.Models
{
    public class MovieModel {
        public string Title { get; set; }
        public string OriginalTitle { get; set; }
        public string Director { get; set; }
        public string Country { get; set; }
        public double Rating { get; set; }
        public int NumberOfVotes { get; set; }
        public DateTime Year { get; set; }
    }

    public class Category {
        public string Title { get; set; }
    }
}
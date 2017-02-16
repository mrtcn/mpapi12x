using System.ComponentModel.DataAnnotations;

namespace MovieConnections.Data.Models {
    public enum MovieType {
        [Display(Name = "Movie")] Movie = 0,
        [Display(Name = "TV Movie")] TvMovie = 1,
        [Display(Name = "TV Series")] Series = 2,
        [Display(Name = "TV Mini-Series")] MiniSeries = 3
    }
}
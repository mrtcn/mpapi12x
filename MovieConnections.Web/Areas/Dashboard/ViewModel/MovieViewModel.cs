using System.Collections.Generic;
using System.Web.Mvc;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Models;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel {
    public class MovieViewModel : MovieParams {
        public MultiSelectList GenreMultiSelectList { get; set; }
        public MultiSelectList ActorMultiSelectList { get; set; }
        public IEnumerable<int> GenreIds { get; set; }
        public IEnumerable<int> ActorIds { get; set; }
        public IEnumerable<SelectListItem> MovieTypeSelectList { get; set; }
        public IEnumerable<SelectListItem> CulturedMovieSelectList { get; set; }
        public MovieViewModel() {
            
        }

        public MovieViewModel(Status status) {
            Status = status;
        }
    }
}
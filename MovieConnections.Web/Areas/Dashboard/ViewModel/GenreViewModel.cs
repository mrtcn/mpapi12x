using System.Collections.Generic;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Models;
using System.Web.Mvc;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel {
    public class GenreViewModel : GenreParams {
        public IEnumerable<SelectListItem> CulturedGenreSelectList { get; set; }

        public GenreViewModel() {
            
        }

        public GenreViewModel(Status status) {
            Status = status;
        }
    }
}
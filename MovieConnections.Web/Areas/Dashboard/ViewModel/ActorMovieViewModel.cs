using System.Collections.Generic;
using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Models;
using System.Web.Mvc;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel {
    public class ActorMovieViewModel : ActorMovieParams {

        public IEnumerable<SelectListItem> MovieSelectList { get; set; }

        public ActorMovieViewModel() {
            
        }

        public ActorMovieViewModel(Status status) {
            Status = status;
        }
    }
}
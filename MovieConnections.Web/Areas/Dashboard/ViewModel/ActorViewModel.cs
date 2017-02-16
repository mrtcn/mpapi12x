using MovieConnections.Core.EntityParams;
using MovieConnections.Data.Models;

namespace MovieConnections.Web.Areas.Dashboard.ViewModel {
    public class ActorViewModel : ActorParams {
        public string CharacterName { get; set; }

        public ActorViewModel() {
            
        }

        public ActorViewModel(Status status) {
            Status = status;
        }
    }
}
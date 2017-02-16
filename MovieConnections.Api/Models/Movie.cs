
using System.Collections.Generic;

namespace MovieConnections.Api.Models
{
    public class GetMoviesModel
    {
        public string SearchText { get; set; }
    }

    public class GetActorsModel
    {
        public int? MovieId { get; set; }
        public string SearchText { get; set; }
    }
}
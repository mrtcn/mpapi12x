using System.Web.Mvc;
using MovieConnections.Core.Services;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Services;

namespace MovieConnections.Web.Controllers
{
    public class MovieCrawlerController : BaseController
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        private readonly IActorService _actorService;

        public MovieCrawlerController(
            IMovieService movieService,
            IGenreService genreService,
            IActorService actorService) {
            _movieService = movieService;
            _actorService = actorService;
            _genreService = genreService; 
        }

        // GET: MovieCrawler
        public ContentResult Index()
        {
            var crawlingService = new CrawlingService(_actorService, _movieService, _genreService);
            crawlingService.StoreAllMovieInGenres(19, "http://www.imdb.com/title/tt1242441/?ref_=adv_li_tt");

            //index++;
            //}

            return Content("asd");
        }        
    }
}
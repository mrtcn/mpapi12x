using System.Web.Mvc;
using MovieConnections.Core.Services;
using MovieConnections.Crawler.Services;
using MovieConnections.Crawler.Controllers.BaseControllers;
using MovieConnections.Data.Entities;
using MovieConnections.Framework.Repository;

namespace MovieConnections.Crawler.Controllers
{
    public class MovieCrawlerController : BaseController
    {
        private readonly IRepository<Actor> _repository;
        private readonly IMovieService _movieService;
        private readonly ICityService _cityService;        
        private readonly ICategoryService _categoryService;
        private readonly IActorService _actorService;

        public MovieCrawlerController(
            IRepository<Actor> repository,
            IMovieService movieService,
            ICityService cityService,            
            ICategoryService categoryService,
            IActorService actorService) {
            _repository = repository;
            _movieService = movieService;
            _cityService = cityService;
            _actorService = actorService;
            _categoryService = categoryService;
        }

        // GET: MovieCrawler
        public ContentResult Index()
        {
            var crawlingService = new CrawlingService(_actorService, _movieService, _categoryService);
            crawlingService.StoreAllMovieInGenres();

            //index++;
            //}

            return Content("asd");
        }        
    }
}
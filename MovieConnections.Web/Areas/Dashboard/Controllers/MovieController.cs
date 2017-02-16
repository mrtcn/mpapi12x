using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Localization;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;
using MovieConnections.Framework.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers {
    [DashboardController(DashboardControllerType.Movie, DashboardControllerType.Independent, 1, "fa fa-link")]
    public class MovieController : BaseController {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        private readonly IActorService _actorService;
        private readonly IGenreMovieService _genreMovieService;
        private readonly IActorMovieService _actorMovieService;

        public MovieController(
            IMovieService movieService, 
            IGenreService genreService, 
            IGenreMovieService genreMovieService, 
            IActorMovieService actorMovieService, 
            IActorService actorService) {
            _movieService = movieService;
            _genreService = genreService;
            _genreMovieService = genreMovieService;
            _actorMovieService = actorMovieService;
            _actorService = actorService;
        }

        private IEnumerable<SelectListItem> GetMovieTypeSelectListItem()
        {
            return Enum<MovieType>.ToSelectListItem();
        }

        private IEnumerable<SelectListItem> GetRelatedEntitiesList()
        {
            var culturedCities = new List<SelectListItem>();
            culturedCities.Add(new SelectListItem { Value = "0", Text = "Seçiniz..." });

            culturedCities.AddRange(_movieService.CulturedEntities
                .Where(x => x.CultureId != CultureHelper.CurrentCulture.Id 
                && x.Status != Status.Deleted && x.BaseEntity.Status != Status.Deleted)
                .Select(x => new SelectListItem {Text = x.Title, Value = x.Id.ToString()}).ToList());
            
            return culturedCities;

        }

        private MultiSelectList GenreMultiSelectList(IEnumerable<string> selectedValues = null)
        {
            var roleMultiSelectList = _genreService.CulturedEntities
                .Where(x => x.Status == Status.Active && x.BaseEntity.Status == Status.Active)
                .Select(x => new { Value = x.Id.ToString(), Text = x.Name })
                .ToMultiSelectList("Value", "Text", selectedValues);
            return roleMultiSelectList;

        }

        private MultiSelectList ActorMultiSelectList(IEnumerable<string> selectedValues = null)
        {
            var roleMultiSelectList = _actorService.Entities
                .Where(x => x.Status == Status.Active)
                .Select(x => new { Value = x.Id.ToString(), Text = x.Name })
                .ToMultiSelectList("Value", "Text", selectedValues);
            return roleMultiSelectList;

        }

        private IEnumerable<string> GetGenreIds(int id) {
            return _genreMovieService.Entities
                .Where(x => x.Status == Status.Active && x.MovieId == id)
                .Select(x => x.Genre.Id.ToString());
        }

        private IEnumerable<string> GetActorIds(int id)
        {
            return _actorMovieService.Entities
                .Where(x => x.Status == Status.Active && x.MovieId == id)
                .Select(x => x.Actor.Id.ToString());
        }

        // GET: Dashboard/Movie
        [DashboardAction("Film Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create(int? relatedEntityId) {
            var movieViewModel = new MovieViewModel(Status.Active);
            movieViewModel.CulturedMovieSelectList = GetRelatedEntitiesList();
            movieViewModel.MovieTypeSelectList = GetMovieTypeSelectListItem();
            movieViewModel.GenreMultiSelectList = GenreMultiSelectList();
            movieViewModel.ActorMultiSelectList = ActorMultiSelectList();

            if (relatedEntityId != null)
            {
                var selectedEntity = _movieService.CulturedEntities.Include(x => x.BaseEntity)
                    .FirstOrDefault(x => x.Id == relatedEntityId);
                
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Movie, MovieViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.BaseEntityId, opt => opt.UseValue(selectedEntity?.BaseEntity.Id))
                    .ForMember(dest => dest.RelatedEntityId, opt => opt.UseValue(relatedEntityId))
                    .ForMember(dest => dest.Status, opt => opt.Ignore());
                });
                var mapper = config.CreateMapper();

                mapper.Map(selectedEntity?.BaseEntity, movieViewModel);
                movieViewModel.CulturedMovieSelectList = GetRelatedEntitiesList();
                return View("CreateOrUpdate", movieViewModel);

            }
            
            return View("CreateOrUpdate", movieViewModel);
        }

        public ActionResult CreateOrUpdate(MovieViewModel model) {
            if (!ValidateForm(model)) {
                model.MovieTypeSelectList = GetMovieTypeSelectListItem();
                model.CulturedMovieSelectList = GetRelatedEntitiesList();

                var genreIds = GetGenreIds(model.Id);
                model.GenreMultiSelectList = GenreMultiSelectList(genreIds);

                var actorIds = GetActorIds(model.Id);
                model.ActorMultiSelectList = ActorMultiSelectList(actorIds);

                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }

            var movieParams = _movieService.CreateOrUpdate(model);

            _genreService.CreateGenresByIds(model.GenreIds, movieParams.Id);
            _actorService.CreateActorsById(model.ActorIds, movieParams.Id );

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { movieParams.Id});
        }

        public ActionResult Update(int id, int? relatedEntityId) {

            MovieViewModel movieViewModel = new MovieViewModel();

            var movieCulture = _movieService.CulturedEntities.Include(x => x.BaseEntity)
                .FirstOrDefault(x => x.Id == id);

            if (relatedEntityId != null)
            {
                var selectedEntity = _movieService.CulturedEntities.Include(x => x.BaseEntity)
                    .FirstOrDefault(x => x.Id == relatedEntityId);
                var movie = selectedEntity?.BaseEntity;
                Mapper.Map(movie, movieViewModel);
                Mapper.Map(movieCulture, movieViewModel);


                movieViewModel.RelatedEntityId = relatedEntityId.GetValueOrDefault();
                movieViewModel.BaseEntityId = (movie?.Id).GetValueOrDefault();
                movieViewModel.CulturedMovieSelectList = GetRelatedEntitiesList();

                return View("CreateOrUpdate", movieViewModel);

            }

            Mapper.Map(movieCulture, movieViewModel);
            Mapper.Map(movieCulture.BaseEntity, movieViewModel);

            movieViewModel.CulturedMovieSelectList = GetRelatedEntitiesList();
            movieViewModel.MovieTypeSelectList = GetMovieTypeSelectListItem();

            var genreIds = GetGenreIds(movieViewModel.Id);
            movieViewModel.GenreMultiSelectList = GenreMultiSelectList(genreIds);

            var actorIds = GetActorIds(movieViewModel.Id);
            movieViewModel.ActorMultiSelectList = ActorMultiSelectList(actorIds);

            return View("CreateOrUpdate", movieViewModel);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            var currentId = CultureHelper.CurrentCulture.Id;
            return Json(_movieService.CulturedEntities.Include(x => x.BaseEntity)
                .Where(x => x.Status == Status.Active && x.CultureId == currentId)
                .Select(x => new { Id = x.BaseEntityId, x.Title
                , x.Country, x.BaseEntity.Director, x.BaseEntity.OriginalTitle
                , x.BaseEntity.Rating, x.BaseEntity.Year, MovieType = x.BaseEntity.MovieType.ToString()
                , Status = x.Status.ToString(), x.CreationDate, x.ModificationDate, x.BaseEntity.NumberOfVotes }).ToList()
                .ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var culturedEntity = _movieService.CulturedEntities
                .FirstOrDefault(x => x.Id == id);

            var entityParams = Mapper.Map<MovieCulture, MovieParams>(culturedEntity);

            var result = _movieService.Remove(entityParams, false, true);

            return result;
        }

        public bool ValidateForm(MovieViewModel model) {
            if(string.IsNullOrEmpty(model.Title))
                ModelState.AddModelError("Title", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
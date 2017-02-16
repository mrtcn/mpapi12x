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
using MovieConnections.Framework.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers {
    [DashboardController(DashboardControllerType.Genre, DashboardControllerType.Independent, 1, "fa fa-link")]
    public class GenreController : BaseController
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        private IEnumerable<SelectListItem> GetRelatedEntitiesList(int? id = null)
        {
            var culturedGenres = new List<SelectListItem>();
            culturedGenres.Add(new SelectListItem { Value = "0", Text = "Seçiniz..." });

            culturedGenres.AddRange(_genreService.CulturedEntities
                .Where(x => x.CultureId != CultureHelper.CurrentCulture.Id
                && x.Status != Status.Deleted && x.BaseEntity.Status != Status.Deleted)
                .Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList());

            return culturedGenres;

        }

        // GET: Dashboard/Genre
        [DashboardAction("Film Kategorisi Düzenle", "fa fa-link")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int? relatedEntityId)
        {
            var genreViewModel = new GenreViewModel(Status.Active);
            genreViewModel.CulturedGenreSelectList = GetRelatedEntitiesList(relatedEntityId);


            if (relatedEntityId != null)
            {
                var selectedEntity = _genreService.CulturedEntities.Include(x => x.BaseEntity)
                    .FirstOrDefault(x => x.Id == relatedEntityId);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Genre, GenreViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.BaseEntityId, opt => opt.UseValue(selectedEntity?.BaseEntity.Id))
                    .ForMember(dest => dest.RelatedEntityId, opt => opt.UseValue(relatedEntityId))
                    .ForMember(dest => dest.Status, opt => opt.Ignore());
                });
                var mapper = config.CreateMapper();

                mapper.Map(selectedEntity?.BaseEntity, genreViewModel);
                genreViewModel.CulturedGenreSelectList = GetRelatedEntitiesList();
                return View("CreateOrUpdate", genreViewModel);

            }


            return View("CreateOrUpdate", genreViewModel);
        }

        public ActionResult CreateOrUpdate(GenreViewModel model)
        {
            if (!ValidateForm(model))
            {
                model.CulturedGenreSelectList = GetRelatedEntitiesList();

                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }

            var genreParams = _genreService.CreateOrUpdate(model);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { genreParams.Id });
        }

        public ActionResult Update(int? id, int? relatedEntityId)
        {

            var idValue = id.GetValueOrDefault();
            GenreViewModel genreViewModel = new GenreViewModel();

            var genreCulture = _genreService.CulturedEntities.Include(x => x.BaseEntity)
                .FirstOrDefault(x => x.Id == idValue);

            if (relatedEntityId != null)
            {
                var selectedEntity = _genreService.CulturedEntities.Include(x => x.BaseEntity)
                    .FirstOrDefault(x => x.Id == relatedEntityId);
                var genre = selectedEntity?.BaseEntity;
                Mapper.Map(genre, genreViewModel);
                Mapper.Map(genreCulture, genreViewModel);


                genreViewModel.RelatedEntityId = relatedEntityId.GetValueOrDefault();
                genreViewModel.BaseEntityId = (genre?.Id).GetValueOrDefault();
                genreViewModel.CulturedGenreSelectList = GetRelatedEntitiesList();

                return View("CreateOrUpdate", genreViewModel);

            }
            Mapper.Map(genreCulture, genreViewModel);
            Mapper.Map(genreCulture.BaseEntity, genreViewModel);

            genreViewModel.CulturedGenreSelectList = GetRelatedEntitiesList();

            return View("CreateOrUpdate", genreViewModel);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request)
        {
            var currentId = CultureHelper.CurrentCulture.Id;
            return Json(_genreService.CulturedEntities
                .Where(x => x.Status != Status.Deleted && x.CultureId == currentId)
                .Select(x => new { x.Id, x.BaseEntityId, x.Name, Status = x.Status.ToString(), x.CreationDate, x.ModificationDate }).ToList()
                .ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id)
        {
            var culturedEntity = _genreService.CulturedEntities
                .FirstOrDefault(x => x.Id == id);

            var entityParams = Mapper.Map<GenreCulture, GenreParams>(culturedEntity);

            var result = _genreService.Remove(entityParams, false, true);

            return result;
        }

        public bool ValidateForm(GenreViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
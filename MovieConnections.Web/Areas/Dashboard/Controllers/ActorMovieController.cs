using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Localization;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers {
    public class ActorMovieController : BaseController {
        private readonly IActorMovieService _actorMovieService;
        private readonly IMovieService _movieService;

        public ActorMovieController(
            IActorMovieService actorService, 
            IMovieService movieService) {
            _actorMovieService = actorService;
            _movieService = movieService;
        }

        private IEnumerable<SelectListItem> GetRelatedEntitiesList()
        {
            var culturedCities = new List<SelectListItem>();
            //culturedCities.Add(new SelectListItem { Value = "0", Text = "Seçiniz..." });

            culturedCities.AddRange(_movieService.CulturedEntities
                .Where(x => x.CultureId == CultureHelper.CurrentCulture.Id
                && x.Status != Status.Deleted && x.BaseEntity.Status != Status.Deleted)
                .Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }).ToList());

            return culturedCities;

        }

        // GET: Dashboard/ActorMovie
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            var model = new ActorMovieViewModel(Status.Active);
            model.MovieSelectList = GetRelatedEntitiesList();

            TempData["actorId"] = Request.QueryString.Get("actorId");

            return View("CreateOrUpdate", model);
        }

        public ActionResult CreateOrUpdate(ActorMovieViewModel model) {
            if (!ValidateForm(model)) {
                model.MovieSelectList = GetRelatedEntitiesList();
                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }
            
            var actorParams = Mapper.Map<ActorMovieParams>(model);

            if (actorParams.ActorId == 0) {
                var actorIdString = TempData["actorId"].ToString();
                int actorId;
                if(Int32.TryParse(actorIdString, out actorId))
                    actorParams.ActorId = actorId;
            }
                
            var actorMovie = _actorMovieService.CreateOrUpdate(actorParams);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { actorMovie.Id});
        }

        public ActionResult Update(int? id) {
            var actorMovie = _actorMovieService.Get(id.GetValueOrDefault());
            var model = Mapper.Map<ActorMovieViewModel>(actorMovie);

            model.MovieSelectList = GetRelatedEntitiesList();

            return View("CreateOrUpdate", model);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request, int? id) {
            var idValue = id.GetValueOrDefault();

            return Json(_actorMovieService.Entities.Where(x => x.Status != Status.Deleted && x.ActorId == idValue)
                .Select(x => new { x.Id, x.CharacterName, Status = x.Status.ToString(), x.CreationDate, x.ModificationDate })
                .ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var result = _actorMovieService.Remove(id);
            return result;
        }

        public bool ValidateForm(ActorMovieViewModel model) {
            if(string.IsNullOrEmpty(model.CharacterName))
                ModelState.AddModelError("CharacterName", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
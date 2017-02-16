using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MovieConnections.Core.BaseServices;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Localization;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Data.Entities;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers {
    [DashboardController(DashboardControllerType.City, DashboardControllerType.Country, 1, "fa fa-link")]
    public class CityController : BaseController {
        private readonly ICityService _cityService;
        private readonly ICulturedBaseService<City, CityCulture, CityParams> _culturedBaseService;
        private readonly ICustomRouteService _customRouteService;

        public CityController(ICityService cityService, 
            ICulturedBaseService<City, 
            CityCulture, CityParams> culturedBaseService, 
            ICustomRouteService customRouteService) {
            _cityService = cityService;
            _culturedBaseService = culturedBaseService;
            _customRouteService = customRouteService;
        }

        private IEnumerable<SelectListItem> GetCulturedCityList(int? id = null)
        {
            var culturedCites = _culturedBaseService.CulturedEntities
                .Where(x => x.CultureId != CultureHelper.CurrentCulture.Id)
                .Select(x => new SelectListItem {Text = x.Name, Value = x.Id.ToString()});

            return culturedCites;

        }

            // GET: Dashboard/City
        [DashboardAction("Şehir Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create(int? relatedEntityId) {
            var cityViewModel = new CityViewModel(Status.Active);
            cityViewModel.CulturedCitySelectList = GetCulturedCityList(relatedEntityId);


            if (relatedEntityId != null)
            {
                var selectedEntity = _culturedBaseService.CulturedEntities.Include(x => x.City)
                    .FirstOrDefault(x => x.Id == relatedEntityId);
                
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<City, CityViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.BaseEntityId, opt => opt.UseValue(selectedEntity?.City.Id))
                    .ForMember(dest => dest.RelatedEntityId, opt => opt.UseValue(relatedEntityId))
                    .ForMember(dest => dest.Status, opt => opt.Ignore());
                });
                var mapper = config.CreateMapper();

                mapper.Map(selectedEntity?.City, cityViewModel);
                cityViewModel.CulturedCitySelectList = GetCulturedCityList();
                return View("CreateOrUpdate", cityViewModel);

            }


            return View("CreateOrUpdate", cityViewModel);
        }

        public ActionResult CreateOrUpdate(CityViewModel model) {
            if (!ValidateForm(model)) {
                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }

            var cityParams = _culturedBaseService.CreateOrUpdate(model);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { cityParams.Id});
        }

        public ActionResult Update(int? id, int? relatedEntityId) {

            var idValue = id.GetValueOrDefault();
            CityViewModel cityViewModel = new CityViewModel();

            var cityCulture = _culturedBaseService.CulturedEntities.Include(x => x.City)
                .FirstOrDefault(x => x.Id == idValue);

            if (relatedEntityId != null)
            {
                var selectedEntity = _culturedBaseService.CulturedEntities.Include(x => x.City)
                    .FirstOrDefault(x => x.Id == relatedEntityId);
                var city = selectedEntity?.City;
                Mapper.Map(city, cityViewModel);
                Mapper.Map(cityCulture, cityViewModel);

                var customRoute = _customRouteService.GetCustomRoute(PredefinedPage.City, cityViewModel.Id);
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<CustomRoute, CityViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore());
                });

                var mapper = config.CreateMapper();
                mapper.Map(customRoute, cityViewModel);

                cityViewModel.RelatedEntityId = relatedEntityId.GetValueOrDefault();
                cityViewModel.BaseEntityId = (city?.Id).GetValueOrDefault();
                cityViewModel.CulturedCitySelectList = GetCulturedCityList();

                return View("CreateOrUpdate", cityViewModel);

            }            

            var model = _culturedBaseService.Map(cityCulture?.City, PredefinedPage.City);

            Mapper.Map(model, cityViewModel);
            cityViewModel.CulturedCitySelectList = GetCulturedCityList();

            return View("CreateOrUpdate", cityViewModel);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            var currentId = CultureHelper.CurrentCulture.Id;
            return Json(_cityService.CulturedEntities
                .Where(x => x.Status == Status.Active && x.CultureId == currentId)
                .Select(x => new { x.Id, x.BaseEntityId, x.Name}).ToList()
                .ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var culturedEntity = _culturedBaseService.CulturedEntities
                .FirstOrDefault(x => x.Id == id);

            var entityParams = Mapper.Map<CityCulture, CityParams>(culturedEntity);

            var result = _culturedBaseService.Remove(entityParams, false, true);

            return result;
        }

        public bool ValidateForm(CityViewModel model) {
            if(string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
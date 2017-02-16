using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers {
    [DashboardController(DashboardControllerType.District, DashboardControllerType.City, 1, "fa fa-link")]
    public class DistrictController : BaseController {
        private readonly IDistrictService _districtService;
        private readonly ICityService _cityService;

        public DistrictController(IDistrictService districtService, ICityService cityService) {
            _districtService = districtService;
            _cityService = cityService;
        }

        public IEnumerable<SelectListItem> CitySelectListItem() {
            return _cityService.CulturedEntities
                .Select(x => new SelectListItem { Text = x.Name, Value = x.BaseEntityId.ToString() });
        }

        // GET: Dashboard/District
        [DashboardAction("İlçe Düzenle", "fa fa-link", 1)]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            var model = new DistrictViewModel();
            model.CitySelectListItems = CitySelectListItem();
            return View("CreateOrUpdate", model);
        }

        public ActionResult CreateOrUpdate(DistrictViewModel model) {
            if (!ValidateForm(model)) {
                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }
            var districtParams = Mapper.Map<DistrictParams>(model);
            var district = _districtService.CreateOrUpdate(districtParams);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { district.Id});
        }

        public ActionResult Update(int? id) {
            var district = _districtService.Get(id.GetValueOrDefault());
            var model = Mapper.Map<DistrictViewModel>(district);
            model.CitySelectListItems = CitySelectListItem();
            return View("CreateOrUpdate", model);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            return Json(_districtService.Entities.Where(x => x.Status == Status.Active)
                .Select(x => new {x.Id, x.Name}).ToList()
                .ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var result = _districtService.Remove(id, false, true);
            return result;
        }

        public bool ValidateForm(DistrictViewModel model) {
            if(string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
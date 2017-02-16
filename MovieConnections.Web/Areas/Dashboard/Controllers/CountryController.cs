using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MovieConnections.Core.EntityParams;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services;
using MovieConnections.Framework.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers {
    [DashboardController(DashboardControllerType.Country, DashboardControllerType.Independent, 1, "fa fa-link")]
    public class CountryController : BaseController {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService) {
            _countryService = countryService;
        }

        // GET: Dashboard/Country
        [DashboardAction("Ülke Düzenle", "fa fa-link", 1)]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            var model = new CountryViewModel();
            return View("CreateOrUpdate", model);
        }

        public ActionResult CreateOrUpdate(CountryViewModel model) {
            if (!ValidateForm(model)) {
                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }
            var countryParams = Mapper.Map<CountryParams>(model);
            var country = _countryService.CreateOrUpdate(countryParams);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { country.Id});
        }

        public ActionResult Update(int? id) {
            var country = _countryService.Get(id.GetValueOrDefault());
            var model = Mapper.Map<CountryViewModel>(country);
            return View("CreateOrUpdate", model);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            return Json(_countryService.Entities.ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var result = _countryService.Remove(id);
            return result;
        }

        public bool ValidateForm(CountryViewModel model) {
            if(string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
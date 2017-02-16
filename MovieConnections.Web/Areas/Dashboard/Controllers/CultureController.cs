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
    [DashboardController(DashboardControllerType.Culture, DashboardControllerType.DashboardManagement, 1, "fa fa-link")]
    public class CultureController : BaseController {
        private readonly ICultureService _cultureService;

        public CultureController(ICultureService cultureService) {
            _cultureService = cultureService;
        }

        // GET: Dashboard/Culture
        [DashboardAction("Dil Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            var model = new CultureViewModel();
            return View("CreateOrUpdate", model);
        }

        public ActionResult CreateOrUpdate(CultureViewModel model) {
            if (!ValidateForm(model)) {
                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }
            var cultureParams = Mapper.Map<CultureParams>(model);
            var culture = _cultureService.CreateOrUpdate(cultureParams);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { culture.Id});
        }

        public ActionResult Update(int? id) {
            var culture = _cultureService.Get(id.GetValueOrDefault());

            var model = Mapper.Map<CultureViewModel>(culture);

            return View("CreateOrUpdate", model);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            return Json(_cultureService.Entities.ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var result = _cultureService.Remove(id);
            return result;
        }

        public bool ValidateForm(CultureViewModel model) {
            if(string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
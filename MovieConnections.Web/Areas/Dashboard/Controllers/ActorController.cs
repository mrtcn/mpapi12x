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
    [DashboardController(DashboardControllerType.Actor, DashboardControllerType.Independent, 1, "fa fa-link")]
    public class ActorController : BaseController {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService) {
            _actorService = actorService;
        }

        // GET: Dashboard/Actor
        [DashboardAction("Aktör Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            var model = new ActorViewModel(Status.Active);
            return View("CreateOrUpdate", model);
        }

        public ActionResult CreateOrUpdate(ActorViewModel model) {
            if (!ValidateForm(model)) {
                CreateNotification(ActionResultType.Failure);
                return View("CreateOrUpdate", model);
            }
            
            var actorParams = Mapper.Map<ActorParams>(model);
            
            var actor = _actorService.CreateOrUpdate(actorParams);

            CreateNotification(ActionResultType.Success);

            return RedirectToAction("Update", new { actor.Id});
        }

        public ActionResult Update(int? id) {
            var actor = _actorService.Get(id.GetValueOrDefault());
            var model = Mapper.Map<ActorViewModel>(actor);
            return View("CreateOrUpdate", model);
        }
        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            return Json(_actorService.Entities.Where(x => x.Status != Status.Deleted)
                .Select(x => new { x.Id, x.Name, Status = x.Status.ToString(), x.CreationDate, x.ModificationDate })
                .ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public RemoveResultStatus Remove(int id) {
            var result = _actorService.Remove(id);
            return result;
        }

        public bool ValidateForm(ActorViewModel model) {
            if(string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
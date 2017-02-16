using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using MovieConnections.Core.Model;
using MovieConnections.Core.Services.IdentityServices;
using MovieConnections.Data.Models;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers.AccountControllers {
    [DashboardController(DashboardControllerType.Role, DashboardControllerType.DashboardManagement, 2, "fa fa-link")]
    public class RoleController : BaseController {
        private readonly ApplicationRoleManager _applicationRoleManager;

        public RoleController(ApplicationRoleManager applicationRoleManager) {
            _applicationRoleManager = applicationRoleManager;
        }

        // GET: Dashboard/Role
        [DashboardAction("Rol Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }

        public ActionResult Create() {
            var model = new RoleViewModel();
            return View("Role", model);
        }

        public async Task<ActionResult> CreateOrUpdate(RoleViewModel model) {
            if (!ValidateForm(model)) {
                return View("Role", model);
            }

            var result = await CreateOrUpdateAsync(model);
            if (!result.Succeeded)
                return View("Role", model);

            return RedirectToAction("Update", new { model.Id });
        }

        public async Task<IdentityResult> CreateOrUpdateAsync(RoleViewModel model) {
            if (model.Id == 0) {
                var applicationRole = Mapper.Map<ApplicationRole>(model);
                applicationRole.Id = 1;
                var result = await _applicationRoleManager.CreateAsync(applicationRole);
                model.Id = applicationRole.Id;
                return result;
            }
            
            var role = _applicationRoleManager.FindByIdAsync(model.Id).Result;
            if (role == null)
                return IdentityResult.Failed();

            MapApplicationRole(role, model);
            var updateResult = await _applicationRoleManager.UpdateAsync(role);
            return updateResult;
        }
        public void MapApplicationRole(ApplicationRole role, RoleViewModel model ) {
            role.Name = model.Name;
            role.Description = model.Description;
        }
        public ActionResult Update(int id) {
            var role = _applicationRoleManager.FindByIdAsync(id).Result;
            if (role == null)
                return View("Role");
            var model = Mapper.Map<RoleViewModel>(role);
            return View("Role", model);
        }

        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            var roles = _applicationRoleManager.Roles;
            return Json(roles.Select(x => new { x.Name, x.Id }).ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public RemoveResultStatus Remove(int id) {
            var role = _applicationRoleManager.FindByIdAsync(id).Result;
            var result = _applicationRoleManager.DeleteAsync(role);
            if (result.Result.Succeeded)
                return RemoveResultStatus.Success;
            return RemoveResultStatus.Failed;
        }

        public bool ValidateForm(RoleViewModel model) {
            if (string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("Name", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
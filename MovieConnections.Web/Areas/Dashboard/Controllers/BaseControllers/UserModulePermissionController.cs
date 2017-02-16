using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MovieConnections.Core.Model;
using MovieConnections.Core.Model.DashboardModule;
using MovieConnections.Core.Services.DashboardServices;
using MovieConnections.Core.Services.IdentityServices;
using MovieConnections.Data.Entities.ModulePermissions;
using MovieConnections.Data.Models;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel.BaseModels;
using Newtonsoft.Json;

namespace MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers {
    [DashboardController(DashboardControllerType.UserModulePermission, DashboardControllerType.DashboardPermission, 1, "fa fa-link")]
    public class UserModulePermissionController : BaseController {
        private readonly IModulePermissionService _modulePermissionService;
        private readonly IModuleService _moduleService;
        private readonly ApplicationUserManager _applicationUserManager;

        public UserModulePermissionController(IModulePermissionService modulePermissionService
            , IModuleService moduleService, ApplicationUserManager applicationUserManager) {
            _modulePermissionService = modulePermissionService;
            _moduleService = moduleService;
            _applicationUserManager = applicationUserManager;
        }

        // GET: Dashboard/UserModulePermission
        [DashboardAction("Kullanıcı İzni Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }
        [HttpPost]
        public void CreateOrUpdate(string items, int userId) {
            var userModulePermissionParams = JsonConvert.DeserializeObject<IEnumerable<DashboardPermissionItemModel>>(items);
            foreach (var userModulePermission in userModulePermissionParams) {
                var usermodulePermissionModel = new ModulePermissionParams(userModulePermission, userId);
                _modulePermissionService.CreateOrUpdate(usermodulePermissionModel);
            }
        }
        [HttpPost]
        public JsonResult GetUsers() {
            var users = _applicationUserManager.Users
                .Select(x => new { Text = x.UserName, Value = x.Id.ToString() });
            return Json(users);

        }
        [HttpPost]
        public JsonResult List(int? id) {
            if (id == null)
                return null;
            var modules = _moduleService.Entities.ToList();
            var userPermissions = _modulePermissionService.Entities.Where(x => x.UserId == id).ToList();
            var userModulePermissionModel = GetUserModulePermission(userPermissions, modules);
            return Json(userModulePermissionModel, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<DashboardPermissionItemModel> GetUserModulePermission(List<ModulePermission> userPermissions, List<Module> modules) {

            var userPermissionModule = modules.GroupJoin(userPermissions
                , module =>  module.Id
                , permission =>  permission.ModuleId
                , (module, userPermission) => new { module, userPermission })
                .SelectMany(x => x.userPermission.DefaultIfEmpty()
                , (module, userPermission) => new { module, userPermission});


            var userModulePermissionModel = userPermissionModule.Select(x => new DashboardPermissionItemModel
            {
                Id = x.userPermission?.Id ?? 0,
                ModuleId = x.module.module.Id,
                ModuleName = x.module.module.ModuleName,
                Create = x.userPermission != null && (x.userPermission.Permission & Permissions.Create) == Permissions.Create,
                Delete = x.userPermission != null && (x.userPermission.Permission & Permissions.Delete) == Permissions.Delete,
                Edit = x.userPermission != null && (x.userPermission.Permission & Permissions.Edit) == Permissions.Edit,
                Export = x.userPermission != null && (x.userPermission.Permission & Permissions.Export) == Permissions.Export,
                View = x.userPermission != null && (x.userPermission.Permission & Permissions.View) == Permissions.View                
            });
            return userModulePermissionModel;
        }

        public RemoveResultStatus Remove(int id) {
            var result = _modulePermissionService.Remove(id);
            return result;
        }
    }
}
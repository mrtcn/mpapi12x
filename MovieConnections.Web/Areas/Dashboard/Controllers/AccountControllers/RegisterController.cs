using System.Collections.Generic;
using System.Data.Entity;
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
using MovieConnections.Framework.Extensions;
using MovieConnections.Web.Areas.Dashboard.Controllers.BaseControllers;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.ViewModel;

namespace MovieConnections.Web.Areas.Dashboard.Controllers.AccountControllers {
    [DashboardController(DashboardControllerType.Register, DashboardControllerType.DashboardManagement, 1, "fa fa-link")]
    public class RegisterController : BaseController {
        private readonly ApplicationUserManager _applicationUserManager;
        private readonly ApplicationRoleManager _applicationRoleManager;
        private readonly ApplicationUserRoleManager _applicationUserRoleManager;
        
        public RegisterController(ApplicationUserManager applicationUserManager
            , ApplicationRoleManager applicationRoleManager
            , ApplicationUserRoleManager applicationUserRoleManager) {
            _applicationUserManager = applicationUserManager;
            _applicationRoleManager = applicationRoleManager;
            _applicationUserRoleManager = applicationUserRoleManager;
        }

        private MultiSelectList RoleMultiSelectList(IEnumerable<string> selectedValues = null) {
            var roleMultiSelectList = _applicationRoleManager.Roles
                .Select(x => new { Value = x.Id.ToString(), Text = x.Name})
                .ToMultiSelectList("Value", "Text", selectedValues);
            return roleMultiSelectList;
            
        }

        // GET: Dashboard/Register
        [DashboardAction("Kullanıcı Düzenle", "fa fa-link")]
        public ActionResult Index() {
            return View();
        }
        
        public ActionResult Create() {
            var model = new RegisterViewModel();
            model.RoleMultiSelectList = RoleMultiSelectList();
            return View("Register", model);
        }

        public async Task<ActionResult> CreateOrUpdate(RegisterViewModel model) {
            if (!ValidateForm(model)) {
                return View("Register", model);
            }

            var result = await CreateOrUpdateAsync(model);
            if (!result.Succeeded)
                return View("Register", model);

            return RedirectToAction("Update", new { model.Id});
        }

        private async Task<IdentityResult> CreateOrUpdateAsync(RegisterViewModel model) {
            
            if (model.Id == 0) {
                var applicationUser = new ApplicationUser();
                Mapper.Map<RegisterViewModel, ApplicationUser>(model, applicationUser);
                
                applicationUser.Id = 1;
                var result = await _applicationUserManager.CreateAsync(applicationUser, model.Password);
                model.Id = applicationUser.Id;

                await CreateOrRemoveRoles(model.RoleIds, model.Id);

                return result;
            }

            var user = _applicationUserManager.FindByIdAsync(model.Id).Result;
            
            if (user == null)
                return IdentityResult.Failed();

            if (user.PasswordHash != model.Password)
                user.PasswordHash = _applicationUserManager.PasswordHasher.HashPassword(model.Password);

            MapApplicationUser(user, model);
            var updateResult = await _applicationUserManager.UpdateAsync(user);

            await CreateOrRemoveRoles(model.RoleIds, model.Id);

            return updateResult;
        }
        private void MapApplicationUser(ApplicationUser applicationUser, RegisterViewModel model) {
            applicationUser.UserName = model.UserName;
            applicationUser.FirstName = model.FirstName;
            applicationUser.LastName = model.LastName;
            applicationUser.Email = model.Email;
            applicationUser.ImagePath = model.ImagePath;
            applicationUser.PhoneNumber = model.PhoneNumber;
            applicationUser.Id = model.Id;            
        }
        private async Task CreateOrRemoveRoles(IEnumerable<int> roleIds, int id) {
            var user = _applicationUserManager.Users.Include(x => x.Roles)
                .FirstOrDefault(x => x.Id == id);
            var userRoleIds = user.Roles.Any() ? user.Roles.Select(x => x.RoleId).ToList() : null;
            var roleIdList = roleIds?.ToList();

            if (userRoleIds != null && roleIdList == null) {
                await _applicationUserRoleManager.RemoveExceptedRolesAsync(userRoleIds, null, id);
            } else if(userRoleIds != null) {
                await _applicationUserRoleManager.CreateNotContainedRolesAsync(userRoleIds, roleIdList, id);
                await _applicationUserRoleManager.RemoveExceptedRolesAsync(userRoleIds, roleIdList, id);
            } else if(roleIdList != null) {
                await _applicationUserRoleManager.CreateNotContainedRolesAsync(null, roleIdList, id);
            }                
        }
        
        public ActionResult Update(int id) {
            var user = _applicationUserManager.Users.Include(x => x.Roles)
                .FirstOrDefault(x => x.Id == id);
            if (user == null)
                return View("Register");

            var model = Mapper.Map<RegisterViewModel>(user);
            model.Password = user.PasswordHash;

            var roleIds = user.Roles.Select(x => x.RoleId.ToString());
            model.RoleMultiSelectList = RoleMultiSelectList(roleIds);

            return View("Register", model);
        }

        public JsonResult List([DataSourceRequest] DataSourceRequest request) {
            var users = _applicationUserManager.Users;
            return Json(users.Select(x => new {x.FirstName, x.LastName, x.UserName, x.Id}).ToList().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public RemoveResultStatus Remove(int id) {
            var user = _applicationUserManager.FindByIdAsync(id).Result;
            var result = _applicationUserManager.DeleteAsync(user);
            return result.Result.Succeeded ? RemoveResultStatus.Success : RemoveResultStatus.Failed;
        }

        public bool ValidateForm(RegisterViewModel model) {
            if(string.IsNullOrEmpty(model.FirstName))
                ModelState.AddModelError("FirstName", "Bu alanın doldurulması zorunludur");
            if (string.IsNullOrEmpty(model.LastName))
                ModelState.AddModelError("LastName", "Bu alanın doldurulması zorunludur");
            if (string.IsNullOrEmpty(model.UserName))
                ModelState.AddModelError("UserName", "Bu alanın doldurulması zorunludur");
            return ModelState.IsValid;
        }
    }
}
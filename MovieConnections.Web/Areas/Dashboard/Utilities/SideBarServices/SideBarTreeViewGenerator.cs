using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MovieConnections.Core.Services.DashboardServices;
using MovieConnections.Core.Services.IdentityServices;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices.Models;

namespace MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices {

    public class SideBarTreeViewGenerator
    {
        private static IEnumerable<DashboardControllerAttribute> AllControllerAttributes() {
            var dashboardControllers = DashboardControllers();
            var dashboardAttributes = dashboardControllers
                .SelectMany(x => x.GetCustomAttributes<DashboardControllerAttribute>());
            return dashboardAttributes;
        }

        private static List<Type> DashboardControllers() {
            var allControllers = Assembly.GetCallingAssembly().GetTypes()
                .Where(x => typeof(Controller).IsAssignableFrom(x));

            var dashboardControllers = allControllers
                .Where(x => x.CustomAttributes
                    .Any(y => y.AttributeType == typeof(DashboardControllerAttribute)))
                    .ToList();
            return dashboardControllers;
        }

        private static IEnumerable<DashboardControllerAttribute> GetChildrenControllerAttributes(
            IEnumerable<DashboardControllerAttribute> dashboardControllerAttributes
            , DashboardControllerType parentControllerType) {

            var childControllerAttributes = dashboardControllerAttributes
                .Where(x => x.ParentControllerType == parentControllerType);
            return childControllerAttributes;
        }

        private static List<DashboardActionAttribute> GetActionAttributes(
            DashboardControllerAttribute dashboardControllerAttribute) {
            //if(dashboardControllerAttribute.ParentControllerType == DashboardControllerType.Null)
            //    return new List<DashboardActionAttribute>();

            var controllers = DashboardControllers();
            var dashboardControllerName = ( dashboardControllerAttribute.Name + "Controller").ToLower();
            var actionAttributes = controllers
                .FirstOrDefault(x => x.Name.ToLower() == dashboardControllerName).GetMethods()
                .Where(x => x.IsPublic && x.ReturnType == typeof(ActionResult) && x.GetCustomAttributes()
                .Any()).ToList()
                .SelectMany(x => x.GetCustomAttributes<DashboardActionAttribute>()
                .Select(y => new DashboardActionAttribute(y,  DashboardAreaRegistration.GetAreaName
                .AddSlashBetweenWords(x.DeclaringType.Name.RemoveWordFromString("Controller")
                .AddSlashBetweenWords(x.Name), true) ))).ToList();

            //MODULE PERMISSIONS
            var modulePermissionService = DependencyResolver.Current.GetService<IModulePermissionService>();
            var applicationRoleManager = DependencyResolver.Current.GetService<ApplicationRoleManager>();

            if(HttpContext.Current.User != null) {
                IIdentity identity = HttpContext.Current.User.Identity;
                var userId = Int32.Parse(identity.GetUserId());
                var roleIds = applicationRoleManager.Roles
                    .Where(x => x.Users.Select(y => y.UserId)
                    .Contains(userId)).Select(x => x.Id).ToList();

                var permittedModules = modulePermissionService.Entities.Include(x => x.Module)
                    .Where(x => 
                    ( x.UserId == userId || roleIds.Contains(x.RoleId.Value) ) 
                    && x.Permission != Permissions.None)
                    .Select(x => x.Module.ModuleName).ToList();

                if (permittedModules != null && permittedModules.Any())
                    actionAttributes.Where(x => permittedModules.Contains(x.Name)).ToList();
                else {
                    return new List<DashboardActionAttribute>();
                }
            }
            
            return actionAttributes;
        }

        public static List<DashboardControllerAttributeModel> GetControllerAttributesInOrder(
            List<DashboardControllerAttribute> dashboardControllerAttributes = null ) {            

            var allControllerAttributes = AllControllerAttributes().ToList();
            if (!allControllerAttributes.Any())
                return null;

            var independentAttributes = dashboardControllerAttributes ?? allControllerAttributes
                .Where(x => x.ParentControllerType == DashboardControllerType.Independent).ToList();            

            //Get Modules Which Belong To a DashboardControllerType that has no parent
            if (dashboardControllerAttributes == null) {

                var parentAttributes = allControllerAttributes
                    .Where(x => x.ParentControllerType != DashboardControllerType.Independent)
                    .Select(x => x.ParentControllerType);
                var childrenAttributeNames = allControllerAttributes.Select(x => x.Name);
                if(parentAttributes != null) {
                    parentAttributes = parentAttributes.Except(childrenAttributeNames);
                    independentAttributes.AddRange(allControllerAttributes
                        .Where(x => parentAttributes.Contains(x.ParentControllerType)).ToList());
                }                
            }
            
            if (!independentAttributes.Any())
                return null;

            if(dashboardControllerAttributes == null)
                independentAttributes = independentAttributes
                    .Select(x => new DashboardControllerAttribute {
                        IconClassName = x.IconClassName,
                        Name = x.Name,
                        ParentControllerType = DashboardControllerType.Null,
                        SortOrder = 1
                    }).Distinct().ToList();

            var orderedControllerAttributes = new List<DashboardControllerAttributeModel>();
            foreach (var independentAttribute in independentAttributes) {

                var actionAttributes = GetActionAttributes(independentAttribute);

                var childrenControllerAttributes = GetChildrenControllerAttributes(
                    allControllerAttributes, independentAttribute.Name)
                    .OrderBy(x => x.SortOrder).ToList();
                
                var dashboardControllerAttributeModel = new DashboardControllerAttributeModel(
                    independentAttribute, GetControllerAttributesInOrder(childrenControllerAttributes), actionAttributes);

                orderedControllerAttributes.Add(dashboardControllerAttributeModel);
            }

            return orderedControllerAttributes;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using MovieConnections.Core.Model.DashboardModule;
using MovieConnections.Core.Mvc.ModelBinders;
using MovieConnections.Core.Services.DashboardServices;
using MovieConnections.Data.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices;
using MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices.Models;

namespace MovieConnections.Web {
    public class Global : HttpApplication {
        private void Application_Start(object sender, EventArgs e) {
            //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            // Code that runs on application startup

            Mapper.Initialize(x => {
                x.CreateMissingTypeMaps = true;                
            });
            
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);    
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);        

            ModelBinders.Binders.Add(typeof(Status), new StatusModelBinder());

            CreateModuleIfExists();
        }

        //Create Module If Exists
        private void CreateModuleIfExists(IEnumerable<DashboardControllerAttributeModel> dashboardControllerAttributeModel = null) {
            var moduleService = DependencyResolver.Current.GetService<IModuleService>();
            var orderedControllerAttributes = dashboardControllerAttributeModel ?? SideBarTreeViewGenerator.GetControllerAttributesInOrder();
            if(orderedControllerAttributes == null)
                return;
            foreach (var orderedControllerAttribute in orderedControllerAttributes) {
                if (orderedControllerAttribute.ChildrenActionAttributes == null)
                    return;
                foreach (var childrenActionAttributes in orderedControllerAttribute.ChildrenActionAttributes) {
                    var actionAttribute = moduleService.Entities.FirstOrDefault(x => x.ModuleName == childrenActionAttributes.Name);
                    if (actionAttribute == null)
                        moduleService.CreateOrUpdate(new ModuleParams { Id = 0, ModuleName = childrenActionAttributes.Name });
                    if(orderedControllerAttribute.ChildrenControllerAttributes != null 
                        && orderedControllerAttribute.ChildrenControllerAttributes.Any())
                        CreateModuleIfExists(orderedControllerAttribute.ChildrenControllerAttributes);
                }
            }
        }
    }
}
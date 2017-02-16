using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MovieConnections.Core.Localization;
using MovieConnections.Core.Services.IdentityServices;
using MovieConnections.Data.Models;
using MovieConnections.Framework.Extensions;
using MovieConnections.Web.Areas.Dashboard.Models;
using MovieConnections.Web.Areas.Dashboard.Utilities.CustomAttributes;
using MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices;
using MovieConnections.Web.Areas.Dashboard.Utilities.SideBarServices.Models;

namespace MovieConnections.Web.Areas {
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel> {
        public string ControllerName => Request.RequestContext.RouteData.Values["Controller"] as string;
        public string ActionName => Request.RequestContext.RouteData.Values["Action"] as string;

        public string UserFullName()
        {
            var identity = GetIdentity();
            return identity != null ? identity.FirstName + " " + identity.LastName : "Bilinmeyen Kullanıcı";
        }

        private ApplicationUser GetIdentity()
        {
            var user = User.Identity;

            var userIdString = user.GetUserId();
            var userId = Int32.Parse(userIdString);

            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var identity = userManager.FindByIdAsync(userId).Result;
            return identity;
        }

        public string UserImagePath()
        {
            return GetIdentity().ImagePath;
        }

        protected override void InitializePage() {
            base.InitializePage();

            var currentTitle = TempData["DashboardPageTitle"];

            if (currentTitle == null)
                TempData["DashboardPageTitle"] = "DashboardModuleTitle(true)";
        }

        public List<SelectListItem> CultureListItems
        {
            get {
                var cultures = CultureHelper.Cultures;
                var cultureListItems = cultures.ToList()
                    .Select(x => new SelectListItem {
                        Text = x.Name,
                        Value = Url.Action("ChangeCulture", "Home"
                        , new { cultureId = x.Id, actionName = ActionName, controllerName = ControllerName }),
                        Selected = x.CultureCode == CultureHelper.CurrentCulture.CultureCode
                    }).ToList();
                return cultureListItems;
            }
        }

        public MvcHtmlString GenerateSideBarMenu() {
            var result = new StringBuilder();

            var orderedControllerAttributes = SideBarTreeViewGenerator.GetControllerAttributesInOrder();
            foreach (var orderedControllerAttribute in orderedControllerAttributes) {
                TagBuilder treeViewLi = null;
                if (orderedControllerAttribute.ParentControllerAttribute.Name == DashboardControllerType.Independent) {
                    foreach (var childrenControllerAttribute in orderedControllerAttribute.ChildrenControllerAttributes) {
                        treeViewLi = GenerateTreeviewLi(childrenControllerAttribute);
                    }
                } else {
                    treeViewLi = GenerateTreeviewLi(orderedControllerAttribute);
                }
                
                result.Append(treeViewLi);
            }

            return MvcHtmlString.Create(result.ToString());
        }

        private TagBuilder GenerateTreeviewLi(DashboardControllerAttributeModel dashboardControllerAttributeModel) {
            var liMenu = new TagBuilder("li");
            liMenu.AddCssClass("treeview");
            var aMenu = new TagBuilder("a");
            aMenu.MergeAttribute("href", "#");

            var iMenu = new TagBuilder("i");
            iMenu.AddCssClass(dashboardControllerAttributeModel.ParentControllerAttribute.IconClassName);

            var spanMenu = new TagBuilder("span");
            spanMenu.SetInnerText(dashboardControllerAttributeModel.ParentControllerAttribute.Name
                .GetEnumDescription<DisplayAttribute>().Name);
            var iSecondMenu = new TagBuilder("i");
            iSecondMenu.AddCssClass("fa fa-angle-left");
            iSecondMenu.AddCssClass("pull-right");

            var ulTreeViewMenu = new TagBuilder("ul");
            ulTreeViewMenu.AddCssClass("treeview-menu");

            if (dashboardControllerAttributeModel.ChildrenActionAttributes != null
                && !dashboardControllerAttributeModel.ChildrenActionAttributes.Any()) {
                
                if (dashboardControllerAttributeModel.ChildrenControllerAttributes != null
                    && dashboardControllerAttributeModel.ChildrenControllerAttributes.Any())
                {
                    foreach (var childControllerAttribute in dashboardControllerAttributeModel.ChildrenControllerAttributes)
                    {
                        var subTreeviewLi = GenerateTreeviewLi(childControllerAttribute);
                        ulTreeViewMenu.InnerHtml += subTreeviewLi;
                    }
                }

                aMenu.InnerHtml += iMenu;
                aMenu.InnerHtml += spanMenu;
                aMenu.InnerHtml += iSecondMenu;
                liMenu.InnerHtml += aMenu;
                liMenu.InnerHtml += ulTreeViewMenu;
                return liMenu;
            }
                

            foreach (var childActionAttribute in dashboardControllerAttributeModel.ChildrenActionAttributes 
                ?? new List<DashboardActionAttribute>()) {

                var liSubMenu = new TagBuilder("li");
                var aSubMenu = new TagBuilder("a");

                aSubMenu.MergeAttribute("href", childActionAttribute.Url);
                aSubMenu.SetInnerText(childActionAttribute.Name);

                liSubMenu.InnerHtml += aSubMenu;
                ulTreeViewMenu.InnerHtml += liSubMenu;
            }

            if (dashboardControllerAttributeModel.ChildrenControllerAttributes != null
                && dashboardControllerAttributeModel.ChildrenControllerAttributes.Any()) {
                foreach (var childControllerAttribute in dashboardControllerAttributeModel.ChildrenControllerAttributes) {
                    var subTreeviewLi = GenerateTreeviewLi(childControllerAttribute);
                    ulTreeViewMenu.InnerHtml += subTreeviewLi;
                }
            }

            aMenu.InnerHtml += iMenu;
            aMenu.InnerHtml += spanMenu;
            aMenu.InnerHtml += iSecondMenu;
            liMenu.InnerHtml += aMenu;
            liMenu.InnerHtml += ulTreeViewMenu;
            return liMenu;
        }
    }

    public abstract class WebViewPage : WebViewPage<dynamic> {
    }
}
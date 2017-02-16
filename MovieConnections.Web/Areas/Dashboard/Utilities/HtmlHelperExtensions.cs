using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MovieConnections.Framework.Extensions;
using MovieConnections.Framework.Models;
using MovieConnections.Framework.Utilities;

namespace MovieConnections.Web.Areas.Dashboard.Utilities
{
    public static class HtmlHelperExtensions {
        public static MvcHtmlString FormSubmitGroup(this HtmlHelper htmlHelper, string submitName = "Gönder") {
            return htmlHelper.Partial("Partials/_FormSubmitGroup", submitName);
        }

        public static MvcHtmlString CustomValidationSummaryMessage(this HtmlHelper htmlHelper) {

            INotificationManager notificationManager = new NotificationsViaViewData(htmlHelper);
            notificationManager.RenderNotification(ActionResultType.Failure);
            notificationManager.RenderNotification(ActionResultType.Success);

            return MvcHtmlString.Create(string.Empty);
        }

        public static MvcHtmlString RenderNotification(this HtmlHelper htmlHelper)
        {

            INotificationManager notificationManager = new NotificationViaTempData(htmlHelper);
            notificationManager.RenderNotification(ActionResultType.Failure);
            notificationManager.RenderNotification(ActionResultType.Success);

            return MvcHtmlString.Create(string.Empty);
        }

        public static string GetNotificationTypeClassName(this HtmlHelper htmlHelper, ActionResultType notificationType)
        {
            switch (notificationType)
            {
                case ActionResultType.Success:
                    return "success";
                case ActionResultType.Failure:
                    return "danger";
                default:
                    return string.Empty;
            }
        }

        public static MvcHtmlString CustomValidationMessage(this HtmlHelper htmlHelper, string expression) {
            var modelName = ExpressionHelper.GetExpressionText(expression);
            if (htmlHelper.ViewData[modelName] != null)
                return MvcHtmlString.Create(
                    string.Format("<label class=\"text-danger\">{0}</label>", htmlHelper.ViewData[modelName]));
            return MvcHtmlString.Create(string.Empty);
        }

        public static MvcHtmlString CustomValidationMessageFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            var modelName = ExpressionHelper.GetExpressionText(expression);
            if (htmlHelper.ViewData[modelName] != null)
                return MvcHtmlString.Create(
                    string.Format("<label class=\"text-danger\">{0}</label>", htmlHelper.ViewData[modelName]));
            return MvcHtmlString.Create(string.Empty);
        }        
    }    
}
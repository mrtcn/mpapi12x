using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MovieConnections.Framework.Models;

namespace MovieConnections.Framework.Utilities
{
    public interface INotificationManager {
        void RenderNotification(ActionResultType actionResultType);
    }

    public class NotificationViaTempData : INotificationManager {
        private readonly HtmlHelper _htmlHelper;

        public NotificationViaTempData(HtmlHelper htmlHelper) {
            _htmlHelper = htmlHelper;
        }

        public void RenderNotification(ActionResultType actionResultType) {
            Render(actionResultType);
        }

        private void Render(ActionResultType actionResultType) {
            var errorMessage = _htmlHelper.ViewContext.TempData[actionResultType.ToString()];

            var ulError = new TagBuilder("ul");

            if (errorMessage != null) {
                var li = new TagBuilder("li");
                li.SetInnerText(errorMessage.ToString());
                ulError.InnerHtml += li;

                _htmlHelper.RenderPartial("Widget/_Notification"
                    , new NotificationModel(actionResultType, ulError.ToString()));
            }
        }
    }

    public class NotificationsViaViewData : INotificationManager {
        private readonly HtmlHelper _htmlHelper;

        public NotificationsViaViewData(HtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public void RenderNotification(ActionResultType actionResultType) {
            Render(actionResultType);
        }

        private void Render(ActionResultType actionResultType) {
            var modelStates = _htmlHelper.ViewData.ModelState
                .Where(x => x.Key == actionResultType.ToString()).ToList();

            var ul = new TagBuilder("ul");

            if (modelStates.Any()) {
                foreach (var modelState in modelStates) {
                    var li = new TagBuilder("li");
                    li.SetInnerText(modelState.Value.Errors.FirstOrDefault().ErrorMessage);
                    ul.InnerHtml += li;
                }
                _htmlHelper.RenderPartial("Widget/_Notification"
                    , new NotificationModel(actionResultType, ul.ToString()));
            }
        }
    }
}

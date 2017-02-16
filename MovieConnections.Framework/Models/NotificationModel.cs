namespace MovieConnections.Framework.Models
{
    public class NotificationModel {
        public NotificationModel(ActionResultType result, string message) {
            Result = result;
            Message = message;
        }
        public ActionResultType Result { get; set; }
        public string Message { get; set; }
    }
}
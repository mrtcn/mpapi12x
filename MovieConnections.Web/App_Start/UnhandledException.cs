using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MovieConnections.Framework.Extensions;
using MovieConnections.Framework.Models;

namespace MovieConnections.Web
{
    public class UnhandledException : IExceptionFilter, IResultFilter {
        public void OnException(ExceptionContext context) {
            context.Controller.ViewData.ModelState.AddModelError(ActionResultType.Failure.ToString()
                , ActionResultType.Failure.GetEnumDescription<DisplayAttribute>().Name);
        }

        public void OnResultExecuting(ResultExecutingContext context) {

        }

        public void OnResultExecuted(ResultExecutedContext context) {
            
        }
    }
}
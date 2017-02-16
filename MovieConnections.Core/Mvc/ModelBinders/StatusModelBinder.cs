using System;
using System.Web.Mvc;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.Mvc.ModelBinders
{
    public class StatusModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext modelBindingContext) {
            try {
                if (!(modelBindingContext.Model is Status))
                    return null;

                var value = modelBindingContext.ValueProvider.GetValue("Status.Status").AttemptedValue;

                if (value.Contains(","))
                    value = value.Split(',')[0];

                int i;
                if (int.TryParse(value, out i))
                    value = Convert.ToBoolean(i).ToString();

                var boolValue = Convert.ToBoolean(value);
                return boolValue ? Status.Active : Status.Inactive;
                
            } catch (Exception exception) {

                return Status.Inactive;
            }
        }
    }
}
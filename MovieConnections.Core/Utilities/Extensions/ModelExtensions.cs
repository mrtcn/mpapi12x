using System;
using MovieConnections.Data.Entities.SupplementaryModels;
using MovieConnections.Data.Models;

namespace MovieConnections.Core.Utilities.Extensions
{
    public static class ModelExtensions {
        public static T FillTracingFields<T>(this T model, ActionTypes actionType) where T : TracingDateModel {
            switch (actionType) {
                case ActionTypes.Create:
                    model.CreationDate = DateTime.Now;
                    break;
                case ActionTypes.Update:
                case ActionTypes.Remove:
                    //model.CreationDate = DateTime.Now;
                    model.ModificationDate = DateTime.Now;
                    break;
            }
            return model;
        }
    }
}

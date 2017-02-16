using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace MovieConnections.Framework.Extensions {
    public static class Enum<T> {
        public static IEnumerable<SelectListItem> ToSelectListItem()
        {
            return Enum.GetValues(typeof(T)).Cast<Enum>()
                .Select(x => new SelectListItem { Text = x.GetEnumDescription<DisplayAttribute>().Name, Value = ((int)Enum.Parse(typeof(T), x.ToString())).ToString() });
        }
    }

    public static class EnumExtensions {
        public static TAttribute GetEnumDescription<TAttribute>(this Enum enumValue) where TAttribute : Attribute {
            var enumAttributeValue = enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<TAttribute>();
            return enumAttributeValue;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) where TAttribute : Attribute {
            var valueType = enumValue.GetType();
            var enumAttribute = enumValue.GetType().GetMember(Enum.GetName(valueType, enumValue))[0].GetCustomAttribute<TAttribute>(false);
            return enumAttribute;
        }

        
    }
}
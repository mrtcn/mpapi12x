using System;
using System.Linq;
using System.Reflection;

namespace MovieConnections.Web.Areas.Dashboard.Utilities.Extensions {

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
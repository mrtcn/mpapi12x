using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MovieConnections.Framework.Utilities.Extensions
{
    public static class AnonymousObjectExtensions
    {
        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            return new AnonymousObjectToDictionary(obj);
        }
    }

    public class AnonymousObjectToDictionary : Dictionary<string, object> {
        private readonly Dictionary<string, object> _dictionary;

        public AnonymousObjectToDictionary() {
            _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public AnonymousObjectToDictionary(object values) {
            _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if(values != null) {
                var valueProperties = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor valueProperty in valueProperties) {
                    var valueToDictionary = valueProperty.GetValue(values);
                    _dictionary.Add(valueProperty.Name, valueToDictionary);
                }
            }
            
        }
    }
}

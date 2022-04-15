using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Text;

namespace OrbitalHelpers
{
    public static class Extensions
    {
        public static bool ContainsChar(this string myString, char myChar)
        {
            return myString.Contains(myChar);
        }

        public static int ContainsNumberOfChar(this string myString, char myChar)
        {
            return myString.Count(a => a == myChar);
        }

        public static T CreateFromDictionary<T>(IReadOnlyDictionary<string, dynamic> d) where T : new()
        {
            var obj = new T();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                propertyInfo.SetValue(obj,d[propertyInfo.Name]);
            }
            return obj;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}

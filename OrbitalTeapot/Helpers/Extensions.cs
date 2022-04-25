using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Data.SqlClient;

namespace OrbitalTeapot.Helpers
{
    /// <summary>
    /// General Extensions for Utviklerne AS
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// Takes string and returns true of char is present
        /// </summary>
        /// <example>
        /// For example:
        /// <code>
        /// string myString = "This is a test string";
        /// var foundChar = myString.ContainsChar('T');
        /// </code>
        /// </example>
        /// <param name="myString"></param>
        /// <param name="myChar"></param>
        /// <returns></returns>
        public static bool ContainsChar(this string myString, char myChar)
        {
            return myString.Contains(myChar);
        }

        /// <summary>
        /// Counts number of specified char in string
        /// </summary>
        /// <example>
        /// For example:
        /// <code>
        /// string myString = "This is a test string";
        /// var lengthString = myString.ContainsNumberOfChar('t');
        /// </code>
        /// </example>
        /// <param name="myString"></param>
        /// <param name="myChar"></param>
        /// <returns>int</returns>
        public static int ContainsNumberOfChar(this string myString, char myChar)
        {
            return myString.Count(a => a == myChar);
        }

        /// <summary>
        /// Creates the passed in class T based of a (string, dynamic) Dictionary
        /// </summary>
        /// <example>
        /// For example:
        /// <code>
        /// var dict = new Dictionary();
        /// dict.CreateClassFromDictionary()
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="d"></param>
        /// <returns>Class of type T</returns>
        public static T CreateClassFromDictionary<T>(this IReadOnlyDictionary<string, dynamic> d) where T : new()
        {
            var obj = new T();

            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                propertyInfo.SetValue(obj, d[propertyInfo.Name]);
            }
            return obj;
        }

        /// <summary>
        /// Creates Dictionary of passed in class T
        /// </summary>
        /// <example>
        /// For example:
        /// <code>
        /// var myClass = new MyClass();
        /// var dictionary = myClass.CreateDictionaryFromClass();
        /// </code>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns>dictionary</returns>
        public static Dictionary<string, dynamic> CreateDictionaryFromClass<T>(this T data)
        {
            return data?.GetType().GetProperties().ToDictionary(a => a.Name, a => (dynamic)a.GetValue(data)) ?? new Dictionary<string, dynamic>();
        }
    }

    public static class IdentityExtensions
    {
        /// <summary>
        /// Returns NameIdentifier from ClaimsPrincipal for lookup in user table
        /// </summary>
        /// <typeparam name="T">User identifier datatype</typeparam>
        /// <param name="user"></param>
        /// <returns>T</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetUserId<T>(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier);
            if (id != null)
            {
                return (T)Convert.ChangeType(id.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            return default(T) ?? throw new InvalidOperationException("Can not return NULL");
        }
    }

    public static class SqlExtensions
    {
        /// <summary>
        /// Creates secure string based on password and user
        /// </summary>
        /// <param name="password"></param>
        /// <param name="user"></param>
        /// <returns>SqlClient.SqlCredential</returns>
        public static SqlCredential PasswordToSecureString(string password, string user)
        {
            var charArrayPassword = password.ToCharArray();
            SecureString secureString = new SecureString();
            foreach (var ch in charArrayPassword)
            {
                secureString.AppendChar(ch);
            }
            secureString.MakeReadOnly();

            return new SqlCredential(user, secureString);
        }
        /// <summary>
        /// Takes IEnumerable string, and returns string with ", " separator for columns
        /// </summary>
        /// <param name="data"></param>
        /// <returns>string</returns>
        public static string ColumnArrayToString(this IEnumerable<string> data)
        {
            return string.Join(", ", data.Select(s => string.Format(s)));
        }
        /// <summary>
        /// Takes IEnumerable string and returns Parameters for dapper in format: "@value, @value2"
        /// </summary>
        /// <param name="data"></param>
        /// <returns>string</returns>
        public static string ValueParamArrayToString(this IEnumerable<string> data)
        {
            return string.Join(", ", data.Select(s => string.Format("@" + s)));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <example>
        /// For example:
        /// <code>
        /// 
        /// </code>
        /// </example>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string StringValueArrayToString(this string[] data)
        {
            return string.Join(", ", data.Select(s => string.Format("'" + s + "'")));
        }
        /// <summary>
        /// Returns SQL datatype string from dynamic list of values
        /// </summary>
        /// <param name="data"></param>
        /// <returns>string</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string[] DynamicArrayToSqlDataType(this dynamic[] data)
        {
            var result = new string[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                TypeCode datatype = Type.GetTypeCode(data[i].GetType());
                result[i] = datatype switch
                {
                    TypeCode.Empty => "",
                    TypeCode.Object => "",
                    TypeCode.DBNull => "",
                    TypeCode.Boolean => "bit",
                    TypeCode.Char => "nvarchar(1)",
                    TypeCode.SByte => "tinyint",
                    TypeCode.Byte => "tinyint",
                    TypeCode.Int16 => "smallint",
                    TypeCode.UInt16 => "smallint",
                    TypeCode.Int32 => "int",
                    TypeCode.UInt32 => "int",
                    TypeCode.Int64 => "bigint",
                    TypeCode.UInt64 => "bigint",
                    TypeCode.Single => "real",
                    TypeCode.Double => "real",
                    TypeCode.Decimal => "real",
                    TypeCode.DateTime => "datetime",
                    TypeCode.String => "nvarchar(255)",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            return result;
        }
    }
}

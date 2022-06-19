using System;
using System.Collections;
using System.Reflection;

namespace XrmEarth.Logger.Utility
{
    public class ReflectionUtil
    {
        public static object GetPropertyValue(object obj, string propName, object[] index = null)
        {
            if (obj == null || string.IsNullOrEmpty(propName))
                return null;

            return obj.GetType().GetProperty(propName).GetValue(obj, index);
        }

        public static void SetPropertyValue(object obj, string propName, object value, object[] index = null)
        {
            if (obj == null || string.IsNullOrEmpty(propName))
                throw new NullReferenceException("obj or propName is null.");

            obj.GetType().GetProperty(propName).SetValue(obj, value, index);
        }

        public static string GetExecutablePath()
        {
            return new Uri(Assembly.GetEntryAssembly().EscapedCodeBase).LocalPath;
        }

        public static string GetExecutablePath(Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }

        public static bool IsWritable(Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
                default:
                    if (t == typeof(Guid))
                    {
                        return true;
                    }
                    return false;
            }
        }

        public static bool IsList(Type t)
        {
            return typeof(ICollection).IsAssignableFrom(t);
        }
    }
}

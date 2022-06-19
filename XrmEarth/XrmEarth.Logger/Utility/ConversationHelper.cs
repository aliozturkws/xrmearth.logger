using System;
using System.ComponentModel;
using System.Data;

namespace XrmEarth.Logger.Utility
{
    public static class ConversionHelper
    {
        public static SqlDbType ToDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    return SqlDbType.Binary;
                case TypeCode.Boolean:
                    return SqlDbType.Bit;
                case TypeCode.Char:
                    return SqlDbType.NChar;
                case TypeCode.SByte:
                    return SqlDbType.SmallInt;
                case TypeCode.Byte:
                    return SqlDbType.TinyInt;
                case TypeCode.Int16:
                    return SqlDbType.SmallInt;
                case TypeCode.Int32:
                case TypeCode.UInt16:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                case TypeCode.UInt32:
                    return SqlDbType.BigInt;
                case TypeCode.Single:
                    return SqlDbType.Real;
                case TypeCode.Double:
                    return SqlDbType.Float;
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                    return SqlDbType.Decimal;
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;
                case TypeCode.String:
                    return SqlDbType.NVarChar;
                case TypeCode.Object:
                    {
                        if (type == typeof(DateTimeOffset))
                        {
                            return SqlDbType.DateTimeOffset;
                        }
                        if (type == typeof(TimeSpan))
                        {
                            return SqlDbType.Timestamp;
                        }
                        if (type == typeof(Guid))
                        {
                            return SqlDbType.UniqueIdentifier;
                        }
                        if (type == typeof(Byte[]))
                        {
                            return SqlDbType.VarBinary;
                        }
                        return SqlDbType.Binary;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static SqlDbType ToDbType<T>()
        {
            return ToDbType(typeof(T));
        }

        public static object ConvertFrom(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                targetType = Nullable.GetUnderlyingType(targetType);

            if (targetType.IsPrimitive || targetType == typeof(decimal) || targetType == typeof(DateTime))
            {
                return Convert.ChangeType(value, targetType);
            }

            return TypeDescriptor.GetConverter(targetType).ConvertFromInvariantString(value.ToString());
        }

        public static T ConvertFrom<T>(object value)
        {
            return (T)ConvertFrom(value, typeof(T));
        }
    }
}

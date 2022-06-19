using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using XrmEarth.Logger.Utility;

namespace XrmEarth.Logger.Data.Sql
{
    public class Parameter : Column
    {
        public ParameterDirection Direction { get; set; }


        public static IEnumerable<Parameter> GenerateParameter<T>(string[] propNames = null)
        {
            var t = typeof (T);

            var props = propNames == null 
                ? t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead)
                : t.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && propNames.Any(pn => pn == p.Name));

            var parameters = new Parameter[0];
            foreach (var prop in props)
            {
                var propType = prop.PropertyType;
                var colName = prop.Name;

                var isNullable = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>);
                if (isNullable)
                    propType = Nullable.GetUnderlyingType(propType);

                if (propType == typeof(string))
                    isNullable = true;

                var propTypeCode = Type.GetTypeCode(propType);

                if (SqlHelper.ExternTypeCodes.Any(tc => tc == propTypeCode) && propType != typeof(Guid))
                    continue;

                var colType = ConversionHelper.ToDbType(propType);

                Array.Resize(ref parameters, parameters.Length + 1);
                parameters[parameters.Length - 1] = new Parameter{Direction = ParameterDirection.Input, Name = colName, SqlDbType = colType, IsNullable = isNullable};
            }
            return parameters;
        }
    }
}

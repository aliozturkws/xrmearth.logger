using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;

namespace XrmEarth.Logger.Extensions
{
    public static class SqlExtensions
    {
        public static SqlParameter Bind<T, TProp>(this SqlParameter sender, T instance, Expression<Func<T, TProp>> propertyExpression)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyExpression.Body).Member;

            sender.ParameterName = propertyInfo.Name;
            sender.Value = propertyInfo.GetValue(instance) ?? DBNull.Value;
            return sender;
        }
    }
}

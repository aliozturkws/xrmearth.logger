using System;
using System.Linq.Expressions;
using System.Reflection;

namespace XrmEarth.Logger.Data.Sql
{
    public class TableConfigBuilder<T>
    {
        public TableConfigBuilder()
        {
            _tableConfig = new TableConfig {Name = typeof (T).Name};
        }

        private readonly TableConfig _tableConfig;

        public TableConfigBuilder<T> SetSchema(string schema)
        {
            _tableConfig.Schema = schema;
            return this;
        }

        public TableConfigBuilder<T> AddForeignKey<TRef, TRefProp, TProp>(Expression<Func<T, TProp>> propertyEpExpression, Expression<Func<TRef, TRefProp>> primaryKeyExpression)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyEpExpression.Body).Member;
            var refPropertyInfo = (PropertyInfo)((MemberExpression)primaryKeyExpression.Body).Member;

            var refTableName = refPropertyInfo.DeclaringType.Name;
            var refColumnName = refPropertyInfo.Name;

            if(_tableConfig.ForeignKeys == null)
                _tableConfig.ForeignKeys = new ForeignKeyColumn[0];

            var foreignKeys = _tableConfig.ForeignKeys;
            Array.Resize(ref foreignKeys, foreignKeys.Length + 1);

            foreignKeys[foreignKeys.Length - 1] = new ForeignKeyColumn{ Name = propertyInfo.Name, ReferenceColumnName = refColumnName, ReferenceTableName = refTableName };
            _tableConfig.ForeignKeys = foreignKeys;

            return this;
        }

        public TableConfigBuilder<T> SetPrimaryKey<TProp>(Expression<Func<T, TProp>> propertyEpExpression)
        {
            var propertyInfo = (PropertyInfo)((MemberExpression)propertyEpExpression.Body).Member;

            _tableConfig.PrimaryKey = new PrimaryKeyColumn{Name = propertyInfo.Name};

            return this;
        }


        public TableConfig Complete()
        {
            return _tableConfig;
        }
    }
}

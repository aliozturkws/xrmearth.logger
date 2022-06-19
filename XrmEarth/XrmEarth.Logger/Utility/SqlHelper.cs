using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using XrmEarth.Logger.Data.Sql;

namespace XrmEarth.Logger.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlHelper : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlHelper(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        private readonly SqlConnection _connection;

        #region - Connection -

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();
        }

        #endregion - Connection -

        #region - Workers -

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string text, CommandType commandType)
        {
            return GetDataTable(text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return GetDataTable(text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string text, CommandType commandType, List<SqlParameter> parameters)
        {
            SqlCommand sqlCom = null;
            var dataTable = new DataTable();
            try
            {
                sqlCom = _connection.CreateCommand();

                sqlCom.CommandTimeout = 60 * 3600;
                sqlCom.CommandType = commandType;
                sqlCom.CommandText = text;

                foreach (var parameter in parameters)
                {
                    sqlCom.Parameters.Add(parameter);
                }

                using (var sqlDatAdapt = new SqlDataAdapter(sqlCom))
                {
                    sqlDatAdapt.Fill(dataTable);
                }
            }
            finally
            {
                if (sqlCom != null)
                    sqlCom.Dispose();
            }

            return dataTable;
        }
            
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string text, CommandType commandType)
        {
            return GetDataSet(text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return GetDataSet(text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string text, CommandType commandType, List<SqlParameter> parameters)
        {
            SqlCommand sqlCom = null;
            var dataSet = new DataSet();
            try
            {
                sqlCom = _connection.CreateCommand();

                sqlCom.CommandTimeout = 60 * 3600;
                sqlCom.CommandType = commandType;
                sqlCom.CommandText = text;

                foreach (var parameter in parameters)
                {
                    sqlCom.Parameters.Add(parameter);
                }

                using (var sqlDatAdapt = new SqlDataAdapter(sqlCom))
                {
                    sqlDatAdapt.Fill(dataSet);
                }
            }
            finally
            {
                if (sqlCom != null)
                    sqlCom.Dispose();
            }

            return dataSet;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string text, CommandType commandType)
        {
            return ExecuteNonQuery(text, commandType, new List<SqlParameter>());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string text, CommandType commandType, List<SqlParameter> parameters)
        {
            SqlCommand sqlCom = null;
            int result;
            try
            {
                sqlCom = _connection.CreateCommand();

                sqlCom.CommandTimeout = 60 * 3600;
                sqlCom.CommandType = commandType;
                sqlCom.CommandText = text;

                foreach (var parameter in parameters)
                {
                    sqlCom.Parameters.Add(parameter);
                }

                result = sqlCom.ExecuteNonQuery();
            }
            finally
            {
                if (sqlCom != null)
                    sqlCom.Dispose();
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public object ExecuteScalar(string text, CommandType commandType)
        {
            return ExecuteScalar(text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteScalar(text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string text, CommandType commandType, List<SqlParameter> parameters)
        {
            SqlCommand sqlCom = null;
            object result;
            try
            {
                sqlCom = _connection.CreateCommand();

                sqlCom.CommandTimeout = 60 * 3600;
                sqlCom.CommandType = commandType;
                sqlCom.CommandText = text;

                foreach (var parameter in parameters)
                {
                    sqlCom.Parameters.Add(parameter);
                }

                result = sqlCom.ExecuteScalar();
            }
            finally
            {
                if (sqlCom != null)
                    sqlCom.Dispose();
            }

            return result;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string text, CommandType commandType)
        {
            return ExecuteScalar<T>(text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteScalar<T>(text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string text, CommandType commandType, List<SqlParameter> parameters)
        {
            var result = ExecuteScalar(text, commandType, parameters);

            if (result == null || result == DBNull.Value)
                return default(T);

            return ConversionHelper.ConvertFrom<T>(result);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }
        }
        #endregion

        #region - Static -

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string connectionString, string text, CommandType commandType)
        {
            return GetDataTable(connectionString, text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string connectionString, string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return GetDataTable(connectionString, text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string connectionString, string text, CommandType commandType, List<SqlParameter> parameters)
        {
            var helper = new SqlHelper(connectionString);

            helper.Open();

            var dt = helper.GetDataTable(text, commandType, parameters);

            helper.Close();

            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string connectionString, string text, CommandType commandType)
        {
            return GetDataSet(connectionString, text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string connectionString, string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return GetDataSet(connectionString, text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string connectionString, string text, CommandType commandType, List<SqlParameter> parameters)
        {
            var helper = new SqlHelper(connectionString);

            helper.Open();

            var dt = helper.GetDataSet(text, commandType, parameters);

            helper.Close();

            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string text, CommandType commandType)
        {
            return ExecuteNonQuery(connectionString, text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(connectionString, text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, string text, CommandType commandType, List<SqlParameter> parameters)
        {
            var helper = new SqlHelper(connectionString);

            helper.Open();

            var res = helper.ExecuteNonQuery(text, commandType, parameters);

            helper.Close();

            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, string text, CommandType commandType)
        {
            return ExecuteScalar(connectionString, text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteScalar(connectionString, text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, string text, CommandType commandType, List<SqlParameter> parameters)
        {
            var helper = new SqlHelper(connectionString);

            helper.Open();

            var res = helper.ExecuteScalar(text, commandType, parameters);

            helper.Close();

            return res;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string connectionString, string text, CommandType commandType)
        {
            return ExecuteScalar<T>(connectionString, text, commandType, new List<SqlParameter>());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string connectionString, string text, CommandType commandType, params SqlParameter[] parameters)
        {
            return ExecuteScalar<T>(connectionString, text, commandType, parameters.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="text"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string connectionString, string text, CommandType commandType, List<SqlParameter> parameters)
        {
            var helper = new SqlHelper(connectionString);

            helper.Open();

            var res = helper.ExecuteScalar<T>(text, commandType, parameters);

            helper.Close();

            return res;
        }

        #region - External - 

        /// <summary>
        /// 
        /// </summary>
        internal static readonly TypeCode[] ExternTypeCodes = {TypeCode.DBNull, TypeCode.Empty, TypeCode.Object };

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string CreateTableQuery<T>(TableConfig config)
        {
            return CreateTableQuery(typeof (T), config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string CreateTableQuery(Type value, TableConfig config)
        {
            if(config == null)
                throw new NullReferenceException("'config' nesnesi boş olamaz.");
            
            var props = value
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead)
                .ToList();

            var haveFk = config.ForeignKeys != null;
            var havePk = config.PrimaryKey != null;
            var columns = new List<Column>();
            foreach (var prop in props)
            {
                var isPk = false;
                var isFk = false;

                var propType = prop.PropertyType;
                var colName = prop.Name;


                var isNullable = propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof (Nullable<>);
                if (isNullable)
                    propType = Nullable.GetUnderlyingType(propType);

                if (propType == typeof (string))
                    isNullable = true;

                var propTypeCode = Type.GetTypeCode(propType);

                if(ExternTypeCodes.Any(tc => tc == propTypeCode) && propType != typeof(Guid))
                    continue;

                var colType = ConversionHelper.ToDbType(propType);

                if (havePk && config.PrimaryKey.Name == colName)
                    isPk = true;

                if (haveFk && config.ForeignKeys.Any(fk => fk.Name == colName))
                    isFk = true;

                Column column;
                if (isPk)
                {
                    column = new PrimaryKeyColumn
                    {
                        Name = colName,
                        SqlDbType = colType,
                        IsNullable = isNullable,
                        AutoIncrement = propTypeCode == TypeCode.Int32
                    };
                }
                else if (isFk)
                {
                    var fkConfig = config.ForeignKeys.First(fk => fk.Name == colName);
                    column = new ForeignKeyColumn
                    {
                        Name = colName,
                        SqlDbType = colType,
                        IsNullable = isNullable,
                        ReferenceTableName = fkConfig.ReferenceTableName,
                        ReferenceColumnName = fkConfig.ReferenceColumnName
                    };
                }
                else
                {
                    column = new Column
                    {
                        Name = colName,
                        SqlDbType = colType,
                        IsNullable = isNullable,     
                    };
                }

                columns.Add(column);
            }

            var sb = new StringBuilder();

            sb.Append("CREATE TABLE ");

            if (!string.IsNullOrWhiteSpace(config.Schema))
                sb.Append(config.Schema).Append(".");

            sb.Append(config.Name).Append(" ( ");

            for (var i = 0; i < columns.Count; i++)
            {
                var col = columns[i];

                sb.Append(col.CreateQuery());

                if (i + 1 < columns.Count)
                    sb.Append(", ");
            }

            sb.Append(" ) ");

            return sb.ToString();
        }

        #endregion - External -

        #endregion - Static -
    }
}

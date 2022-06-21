using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Data.Sql;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Renderer;
using XrmEarth.Logger.Renderer.Base;
using XrmEarth.Logger.Renderer.Mssql;
using XrmEarth.Logger.Utility;

namespace XrmEarth.Logger.Logger
{
    public class MssqlLogger : BaseLogger<MssqlConnection, MssqlRenderer>, XrmEarth.Logger.Initializer.ILogEnvironmentInitializer
    {
        public MssqlLogger(MssqlConnection connection)
            : this(connection, new MssqlRenderer())
        {
        }

        public MssqlLogger(MssqlConnection connection, MssqlRenderer renderer)
            : base(connection, renderer, true)
        {

        }

        protected override void OnPush(Dictionary<string, object> keyValuesDictionary)
        {
            if(!keyValuesDictionary.ContainsKey(BaseRenderer.TypeKey))
                throw new KeyNotFoundException(string.Format("The render object must send the type of renderer object named '{0}'.", BaseRenderer.TypeKey));

            var rendererType = keyValuesDictionary[BaseRenderer.TypeKey] as Type;
            if(rendererType == null)
                throw new NullReferenceException(string.Format("The '{0}' key must store the renderer object type.", BaseRenderer.TypeKey));

            if (rendererType.IsAssignableFrom(typeof (MssqlSPRendererBase)))
            {
                var spName = keyValuesDictionary[MssqlSPRendererBase.NameKey];
                var parameters = keyValuesDictionary[MssqlSPRendererBase.ParametersKey] as List<SqlParameter>;

                if(spName == null)
                    throw new NullReferenceException(string.Format("SP name field left blank in renderer object of type '{0}'.", rendererType.Name));

                if (parameters == null)
                {
                    SqlHelper.ExecuteNonQuery(Connection.CreateConnectionString(), spName.ToString(), CommandType.StoredProcedure);
                }
                else
                {
                    SqlHelper.ExecuteNonQuery(Connection.CreateConnectionString(), spName.ToString(), CommandType.StoredProcedure, parameters);
                }
            }
            else if (rendererType.IsAssignableFrom(typeof(MssqlQueryRendererBase)))
            {
                var query = keyValuesDictionary[MssqlQueryRendererBase.QueryKey];
                var parameters = keyValuesDictionary[MssqlQueryRendererBase.ParametersKey] as List<SqlParameter>;

                if (query == null)
                    throw new NullReferenceException(string.Format("The Query field is left blank in the renderer object of type '{0}'.", rendererType.Name));

                if (parameters == null)
                {
                    SqlHelper.ExecuteNonQuery(Connection.CreateConnectionString(), query.ToString(), CommandType.Text);
                }
                else
                {
                    SqlHelper.ExecuteNonQuery(Connection.CreateConnectionString(), query.ToString(), CommandType.Text, parameters);
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("Unsupported renderer type : '{0}'", rendererType.Name));
            }
        }

        public static void InitializeTables(MssqlConnection connection)
        {
            var conStr = connection.CreateConnectionString();

            var appQuery = SqlHelper.CreateTableQuery<Application>(
                new TableConfigBuilder<Application>()
                    .SetPrimaryKey(a => a.ID)
                    .Complete());
            //SimpleLog.Instance.Push(appQuery, SimpleLogLevel.Debug);

            var appInstanceQuery = SqlHelper.CreateTableQuery<ApplicationInstance>(
                new TableConfigBuilder<ApplicationInstance>()
                    .SetPrimaryKey(ai => ai.ID)
                    .AddForeignKey<Application, Guid, Guid>(ai => ai.ApplicationID, a => a.ID)
                    .Complete());
            //SimpleLog.Instance.Push(appInstanceQuery, SimpleLogLevel.Debug);

            var appLogQuery = SqlHelper.CreateTableQuery<ApplicationLog>(
                new TableConfigBuilder<ApplicationLog>()
                    .SetPrimaryKey(al => al.ID)
                    .AddForeignKey<ApplicationInstance, Guid, Guid>(al => al.ApplicationInstanceID, ai => ai.ID)
                    .Complete());
            //SimpleLog.Instance.Push(appLogQuery, SimpleLogLevel.Debug);

            var fullQuery = "BEGIN TRANSACTION " + appQuery + " " + appInstanceQuery + " " + appLogQuery + " COMMIT TRANSACTION";
            //SimpleLog.Instance.Push(fullQuery, SimpleLogLevel.Debug);

            SqlHelper.ExecuteNonQuery(conStr, fullQuery, CommandType.Text);
        }

        public string InitializeEnvironment()
        {
            InitializeTables(Connection);
            return "Success";
        }
    }
}

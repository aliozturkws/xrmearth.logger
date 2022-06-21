using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Exceptions;
using XrmEarth.Logger.Extensions;
using XrmEarth.Logger.Renderer.Base;
using XrmEarth.Logger.Renderer.Mssql;

namespace XrmEarth.Logger.Renderer
{
    public class MssqlRenderer : MultiBaseRenderer<IMssqlRenderer>
    {
        public MssqlRenderer()
        {
            Register<Application>(MssqlSPRendererBase.Create("SP_Insert_Application", _appParamsFunc, _appValidateAction));
            Register<ApplicationInstance>(new MultiKeyRenderer<ProcessType, IMssqlRenderer>(
            new Dictionary<ProcessType, IMssqlRenderer>
            {
                {ProcessType.Insert, MssqlSPRendererBase.Create("SP_Insert_ApplicationInstance", _appInstanceInsertParamsFunc)},
                {ProcessType.Update, MssqlSPRendererBase.Create("SP_Update_ApplicationInstance", _appInstanceUpdateParamsFunc)},
            }));
            Register<ApplicationLog>(MssqlSPRendererBase.Create("SP_Insert_ApplicationLog", _appLogParamsFunc));
        }

        public const string AppSqlIDParamKey = "ValidID";


        #region - Application -

        private readonly Func<object, List<SqlParameter>> _appParamsFunc = delegate(object o)
        {
            var app = o as Application;
            if (app == null)
                return null;

            return new List<SqlParameter>
            {
                new SqlParameter().Bind(app, a => a.ID),
                new SqlParameter().Bind(app, a => a.Name),
                new SqlParameter().Bind(app, a => a.Namespace),
                new SqlParameter().Bind(app, a => a.Description),
                new SqlParameter().Bind(app, a => a.AssemblyGuid),
                new SqlParameter().Bind(app, a => a.AssemblyVersion),
                new SqlParameter().Bind(app, a => a.CreatedAt),
                new SqlParameter(AppSqlIDParamKey, SqlDbType.UniqueIdentifier){Direction = ParameterDirection.Output}
            };
        };

        private readonly Action<object, Dictionary<string, object>> _appValidateAction = (o, objects) =>
        {
            var app = o as Application;
            if (app == null)
                return;

            if (!objects.ContainsKey(MssqlSPRendererBase.ParametersKey))
                return;

            var sParams = objects[MssqlSPRendererBase.ParametersKey] as List<SqlParameter>;

            if (sParams == null)
                return;

            var sParam = sParams.FirstOrDefault(p => p.ParameterName == AppSqlIDParamKey);

            if (sParam == null || sParam.Value == DBNull.Value)
                return;

            if (sParam.SqlDbType != SqlDbType.UniqueIdentifier)
                throw new InvalidTypeException(string.Format("Object of type '{0}' cannot be converted to Guid.", sParam.SqlDbType));

            var id = Guid.Parse(sParam.Value.ToString());
            if (id == Guid.Empty)
                return;

            app.ID = id;
        };

        #endregion - Application -


        #region - Application Instance -

        private readonly Func<object, List<SqlParameter>> _appInstanceInsertParamsFunc = delegate(object o)
        {
            var appIns = o as ApplicationInstance;
            if (appIns == null)
                return null;

            return new List<SqlParameter>
            {
                new SqlParameter().Bind(appIns, a => a.ID),
                new SqlParameter().Bind(appIns, a => a.ApplicationID),
                new SqlParameter().Bind(appIns, a => a.Path),
                new SqlParameter().Bind(appIns, a => a.StartAt),
                new SqlParameter().Bind(appIns, a => a.Parameters),
                new SqlParameter().Bind(appIns, a => a.CreatedAt)
            };
        };

        private readonly Func<object, List<SqlParameter>> _appInstanceUpdateParamsFunc = delegate(object o)
        {
            var appIns = o as ApplicationInstance;
            if (appIns == null)
                return null;

            return new List<SqlParameter>
            {
                new SqlParameter().Bind(appIns, a => a.ID),
                new SqlParameter().Bind(appIns, a => a.FinishAt),
                new SqlParameter().Bind(appIns, a => a.Result),
                new SqlParameter().Bind(appIns, a => a.Summary)
            };
        };

        #endregion - Application Instance -


        #region - Application Log -

        private readonly Func<object, List<SqlParameter>> _appLogParamsFunc = delegate(object o)
        {
            var appLog = o as ApplicationLog;
            if (appLog == null)
                return null;

            return new List<SqlParameter>
            {
                new SqlParameter().Bind(appLog, a => a.ID),
                new SqlParameter().Bind(appLog, a => a.ApplicationInstanceID),
                new SqlParameter().Bind(appLog, a => a.ParentCallerMember),
                new SqlParameter().Bind(appLog, a => a.CallerMember),
                new SqlParameter().Bind(appLog, a => a.Message),
                new SqlParameter().Bind(appLog, a => a.Type),
                new SqlParameter().Bind(appLog, a => a.LogLevel),
                new SqlParameter().Bind(appLog, a => a.Tag1),
                new SqlParameter().Bind(appLog, a => a.Tag2),
                new SqlParameter().Bind(appLog, a => a.CreatedAt)
            };
        };

        #endregion - Application Log -
    }
}

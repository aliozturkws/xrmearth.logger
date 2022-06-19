using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace XrmEarth.Logger.Renderer.Mssql
{
    public class MssqlQueryRendererBase : MssqlRendererBase, IMssqlQueryRenderer
    {
        protected MssqlQueryRendererBase(string query)
        {
            Query = query;
        }

        public const string QueryKey = "QueryRendererBase_Query";
        public const string ParametersKey = "QueryRendererBase_Parameters";

        public string Query { get; set; }

        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            return new Dictionary<string, object>
                   {
                       {QueryKey, Query},
                       {ParametersKey, GetParameters(value)},
                   };
        }

        public Func<object, List<SqlParameter>> GetParameters { get; set; }


        public static MssqlQueryRendererBase Create(string query, Func<object, List<SqlParameter>> parametersFunction, Action<object, Dictionary<string, object>> validateAction = null)
        {
            return new MssqlQueryRendererBase(query)
            {
                GetParameters = parametersFunction,
                ValidateAction = validateAction
            };
        }
    }
}

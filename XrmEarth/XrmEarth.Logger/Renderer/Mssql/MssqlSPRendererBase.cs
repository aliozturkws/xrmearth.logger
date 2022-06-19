using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace XrmEarth.Logger.Renderer.Mssql
{
    public class MssqlSPRendererBase : MssqlRendererBase, IMssqlSPRenderer
    {
        protected MssqlSPRendererBase(string name)
        {
            Name = name;
        }

        public const string NameKey = "SPRendererBase_Name";
        public const string ParametersKey = "SPRendererBase_Parameters";

        public string Name { get; set; }

        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            
            return new Dictionary<string, object>
                   {
                       {NameKey, Name},
                       {ParametersKey, GetParameters(value)},
                   };
        }

        public Func<object, List<SqlParameter>> GetParameters { get; set; }

        public static MssqlSPRendererBase Create(string spName, Func<object, List<SqlParameter>> parametersFunction, Action<object, Dictionary<string, object>> validateAction = null)
        {
            return new MssqlSPRendererBase(spName)
            {
                GetParameters = parametersFunction,
                ValidateAction = validateAction
            };
        }
    }
}

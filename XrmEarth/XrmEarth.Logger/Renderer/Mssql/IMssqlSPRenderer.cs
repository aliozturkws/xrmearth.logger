using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace XrmEarth.Logger.Renderer.Mssql
{
    public interface IMssqlSPRenderer : IMssqlRenderer
    {
        string Name { get; set; }

        Func<object, List<SqlParameter>> GetParameters { get; set; }
    }
}

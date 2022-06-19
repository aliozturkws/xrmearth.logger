using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace XrmEarth.Logger.Renderer.Mssql
{
    public interface IMssqlQueryRenderer : IMssqlRenderer
    {
        string Query { get; set; }

        Func<object, List<SqlParameter>> GetParameters { get; set; }
    }
}

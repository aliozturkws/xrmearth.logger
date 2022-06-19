using System;
using System.Collections.Generic;
using XrmEarth.Logger.Renderer.Base;

namespace XrmEarth.Logger.Renderer.Mssql
{
    public abstract class MssqlRendererBase : BaseRenderer
    {
        public override Action<object, Dictionary<string, object>> ValidateAction { get; set; }
    }
}

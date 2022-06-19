using System;
using System.Collections.Generic;

namespace XrmEarth.Logger.Renderer
{
    public interface IRenderer
    {
        Action<object, Dictionary<string, object>> ValidateAction { get; set; }

        Dictionary<string, object> RenderObject(object value);
    }
}

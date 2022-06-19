using System;
using System.Collections.Generic;

namespace XrmEarth.Logger.Renderer.Base
{
    public abstract class BaseRenderer : IRenderer
    {
        public const string TypeKey = "BaseRenderer_Type";

        public abstract Action<object, Dictionary<string, object>> ValidateAction { get; set; }
        public Dictionary<string, object> RenderObject(object value)
        {
            var dic = OnRenderObject(value) ?? new Dictionary<string, object>();
            if(!dic.ContainsKey(TypeKey))
                dic[TypeKey] = GetType();

            return dic;
        }

        protected abstract Dictionary<string, object> OnRenderObject(object value);
    }
}

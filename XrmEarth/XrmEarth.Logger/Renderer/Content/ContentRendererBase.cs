using System;
using System.Collections.Generic;
using XrmEarth.Logger.Renderer.Base;

namespace XrmEarth.Logger.Renderer.Content
{
    public class ContentRendererBase : BaseRenderer, IContentRenderer
    {
        public const string ContentKey = "ContentRendererBase_Content";

        public override Action<object, Dictionary<string, object>> ValidateAction { get; set; }
        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            return new Dictionary<string, object>
            {
                {ContentKey, GetContent(value)}
            };
        }

        public Func<object, string> GetContent { get; set; }
    }
}

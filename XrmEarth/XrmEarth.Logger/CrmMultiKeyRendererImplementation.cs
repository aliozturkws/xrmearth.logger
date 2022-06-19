using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using XrmEarth.Logger.Renderer;
using XrmEarth.Logger.Renderer.Base;

namespace XrmEarth.Logger
{
    public class CrmMultiKeyRendererImplementation<TKey, TRenderer> : MultiKeyRenderer<TKey, TRenderer>, ICrmRenderer
        where TRenderer : IRenderer
    {
        public CrmMultiKeyRendererImplementation(List<KeyValuePair<TKey, TRenderer>> typeRenderers) : base(typeRenderers)
        {

        }

        public Func<object, Microsoft.Xrm.Sdk.Entity> GetLogEntity { get; set; }
        public Func<object, RequestType> GetRequestType { get; set; }
    }
}

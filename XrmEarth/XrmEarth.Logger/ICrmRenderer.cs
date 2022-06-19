using System;
using Microsoft.Xrm.Sdk;
using XrmEarth.Logger.Renderer;

namespace XrmEarth.Logger
{
    public interface ICrmRenderer : IRenderer
    {
        Func<object, Microsoft.Xrm.Sdk.Entity> GetLogEntity { get; set; }
        Func<object, RequestType> GetRequestType { get; set; }
    }

    public enum RequestType
    {
        Create,
        Update,
    }
}

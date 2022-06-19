using System;

namespace XrmEarth.Logger.Renderer.Smtp
{
    public interface ISmtpRenderer : IRenderer
    {
        Func<object, string> GetSubject { get; set; }
        Func<object, string> GetBody { get; set; }
    }
}

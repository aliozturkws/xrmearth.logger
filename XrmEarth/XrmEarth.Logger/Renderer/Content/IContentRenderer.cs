using System;

namespace XrmEarth.Logger.Renderer.Content
{
    public interface IContentRenderer : IRenderer
    {
        Func<object, string> GetContent { get; set; }
    }
}

using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Logger;
using XrmEarth.Logger.Renderer;

namespace XrmEarth.Logger.Target
{
    public class FileTarget : LogTarget
    {
        public BaseLogger<FileConnection, ContentRenderer> CreateLogger(ContentRenderer contentRenderer = default(ContentRenderer))
        {
            var fileCon = CastConnection<FileConnection>(Connection);
            return contentRenderer == null ? new FileLogger(fileCon) : new FileLogger(fileCon, contentRenderer);
        }

        protected override CoreLogger OnCreateLogger()
        {
            return new FileLogger(CastConnection<FileConnection>(Connection));
        }
    }
}

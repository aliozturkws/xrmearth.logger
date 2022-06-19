using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Logger;
using XrmEarth.Logger.Renderer;

namespace XrmEarth.Logger.Target
{
    public class MssqlLogTarget : LogTarget
    {
        public BaseLogger<MssqlConnection, MssqlRenderer> CreateLogger(MssqlRenderer mssqlRenderer = default(MssqlRenderer))
        {
            var msCon = CastConnection<MssqlConnection>(Connection);
            return mssqlRenderer == null ? new MssqlLogger(msCon) : new MssqlLogger(msCon, mssqlRenderer);
        }

        protected override CoreLogger OnCreateLogger()
        {
            return new MssqlLogger(CastConnection<MssqlConnection>(Connection));
        }
    }
}

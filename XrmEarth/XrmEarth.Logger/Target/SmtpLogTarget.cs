using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Logger;

namespace XrmEarth.Logger.Target
{
    public class SmtpLogTarget : LogTarget
    {
        protected override CoreLogger OnCreateLogger()
        {
            return new SmtpLogger((SmtpConnection)Connection);
        }
    }
}

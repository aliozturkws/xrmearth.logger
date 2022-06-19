using XrmEarth.Logger.Logger;
using XrmEarth.Logger.Target;

namespace XrmEarth.Logger
{
    public class CrmTarget : LogTarget
    {
        protected override CoreLogger OnCreateLogger()
        {
            return new CrmLogger((CrmConnection)Connection);
        }
    }
}

using XrmEarth.Logger.Logger;

namespace XrmEarth.Logger.Target
{
    public class ConsoleTarget : LogTarget
    {
        protected override CoreLogger OnCreateLogger()
        {
            return new ConsoleLogger(true);
        }
    }
}

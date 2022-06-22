using System;

namespace XrmEarth.Logger.Plugins
{
    public class Sample1 : BasePlugin
    {
        public override void OnExecute(IServiceProvider serviceProvider)
        {
            CrmLogger.Error("Test Error", 1501, "Tag1 Value","Tag2 Value");
            CrmLogger.Info("Test Info", 1502, "Tag1 Value", "Tag2 Value");
            CrmLogger.Warning("Test Warning", 1502, "Tag1 Value", "Tag2 Value");
        }
    }
}
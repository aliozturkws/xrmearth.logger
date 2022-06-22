using Microsoft.Xrm.Sdk;
using System;
using XrmEarth.Logger.Configuration;
using XrmEarth.Logger.Logger;

namespace XrmEarth.Logger.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        public abstract void OnExecute(IServiceProvider serviceProvider);

        public CoreLogger CrmLogger { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {
            #region | Context and Service |
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            #endregion | Context and Service |

            try
            {
                tracingService.Trace("started..");

                SetLoggerConfiguration(service);

                OnExecute(serviceProvider);

                tracingService.Trace("ended..");
            }
            catch (Exception ex)
            {
                tracingService.Trace(string.Concat("Error : ", ex.ToString()));
                throw;
            }
        }

        private void SetLoggerConfiguration(IOrganizationService service)
        {
            InitConfiguration.InjectApplication = false;
            InitConfiguration.OverrideAssembly = typeof(BasePlugin).Assembly;
            var crmConnection = new CrmConnection(service);
            CrmLogger = LogManager.CreateLogger(crmConnection);
            LogManager.RegisterAll(CrmLogger);
        }
    }
}

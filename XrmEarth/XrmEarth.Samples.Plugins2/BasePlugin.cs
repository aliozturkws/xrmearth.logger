//using Microsoft.Xrm.Sdk;
//using System;
//using XrmEarth.Logger;
//using XrmEarth.Logger.Logger;

//namespace XrmEarth.Samples.Plugins
//{
//    public abstract class BasePlugin : IPlugin
//    {
//        protected abstract void OnExecute(IServiceProvider serviceProvider);

//        public CoreLogger CrmLogger { get; set; }

//        public void Execute(IServiceProvider serviceProvider)
//        {
//            try
//            {
//                #region | Context |
//                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
//                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
//                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
//                #endregion | Context |

//                var crmConnection = new CrmConnection(service);
//                CrmLogger = LogManager.CreateLogger(crmConnection);
//                LogManager.RegisterAll(CrmLogger);

//                OnExecute(serviceProvider);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//    }
//}

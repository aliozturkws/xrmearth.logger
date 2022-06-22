using Microsoft.Xrm.Sdk;
using System;
using System.Text;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Console.Base;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Exceptions;

namespace XrmEarth.Logger.Console
{
    public class SimpleCrm : BaseSample
    {
        public IOrganizationService Service { get; set; }

        public SimpleCrm(IOrganizationService service)
        {
            Service = service;
        }

        protected override void OnRun()
        {
            LogManager.Instance.ApplicationInjectFailedException += InstanceOnApplicationInjectFailedException;
            LogManager.Instance.UnhandledException += InstanceOnUnhandledException;
            LogManager.Instance.ApplicationClosing += InstanceOnApplicationClosing;
            LogManager.Instance.SystemNotify += InstanceOnSystemNotify;

            var crmConnection = new CrmConnection(Service);
            var crmLogger = LogManager.CreateLogger(crmConnection);
            LogManager.RegisterAll(crmLogger);

            crmLogger.Info("Test Log Message", 15);
        }

        private void InstanceOnSystemNotify(SystemNotifyArgs e)
        {
            Output.WriteLine(
                new StringBuilder()
                    .Append("Message: ").Append(e.Message).AppendLine()
                    .Append("Type: ").Append(e.Type).AppendLine()
                    .Append("Level: ").Append(e.Level).AppendLine()
                    .Append("Initialize Completed: ").Append(e.InitializeCompleted).AppendLine()
                    .AppendLine()
                    .ToString());
            if (e.InitializeCompleted)
            {
                LogManager.Push(e.Message, e.Type, e.Level, "SYSTEM_NOTIFY");
            }
            else
            {
                //The 'Push' method may not work because the system has not been fully loaded yet.
            }
        }

        private void InstanceOnApplicationClosing(InstanceSummary instanceSummary)
        {
            instanceSummary.Result = "Success";
            instanceSummary.Summary = string.Format("Total Imported Records: {0} | Number of Successful Transactions: {1} | Number of Failed Transactions: {2}", 65535, 65280, 255);
        }

        private void InstanceOnApplicationInjectFailedException(Exception exception)
        {
            //An error occurred while accessing application information.
            //Note: For Sandbox uses, 'InitConfiguration.InjectApplication' must be set to false.
        }

        private void InstanceOnUnhandledException(UnhandledException unhandledException)
        {
            Output.WriteLine(unhandledException.ToString());
            if (unhandledException.Type == UnhandledExceptionType.Application)
            {
                //An error has occurred in the system.
                if (unhandledException.Object != null)
                {
                    //If the error occurred in the system occurs in the logging process, you can access the last log object from the 'Object' property.
                }
            }
            else
            {
                //An error occurred outside the system.
            }
            if (unhandledException.IsTerminating)
            {
                //The application will be terminated.
            }
            else
            {
                //The application can continue.
            }
        }
    }
}

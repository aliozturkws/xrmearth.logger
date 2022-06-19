using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Exceptions;

namespace XrmEarth.Logger.Logger
{
    public abstract class CoreLogger : IDisposable
    {
        protected CoreLogger(bool followApplication)
        {
            _followApplication = followApplication;
        }
        private readonly bool _followApplication;
        protected ApplicationContainer CurrentContainer;

        protected readonly Dictionary<string, object> DataDictionary = new Dictionary<string, object>();

        public abstract void Push(object value, string memberName = "");

        public void Info(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Info, logLevel, tag1, tag2, memberName);
        }

        public void Warning(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Warning, logLevel, tag1, tag2, memberName);
        }

        public void Error(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Error, logLevel, tag1, tag2, memberName);
        }

        public void Object(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Object, logLevel, tag1, tag2, memberName);
        }

        public void State(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.State, logLevel, tag1, tag2, memberName);
        }

        public void Trace(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Trace, logLevel, tag1, tag2, memberName);
        }
        
        public void Push(
            string message
            , LogType logType
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            var al = new ApplicationLog
            {
                ApplicationInstanceID = CurrentContainer != null ? CurrentContainer.ApplicationInstance.ID : Guid.Empty,
                Message = message,
                Type = logType,
                LogLevel = logLevel,
                Tag1 = tag1,
                Tag2 = tag2
            };

            Push(al, memberName);
        }


        protected void Initialize(IConnection connection)
        {
            CurrentContainer = ApplicationShared.GetApplicationContainer(connection);

            TryApplicationRegister();
            ApplicationStarted();

            ApplicationShared.ApplicationClosed += SharedApplicationClosed;
        }

        protected void HandleException(Exception exception, object logObject)
        {
            LogManager.Instance.OnCallUnhandledException(new UnhandledException("Loglama işlemi sırasında bir hata meydana geldi. Detay için 'UnhandledException.Object' özelliğine bakın.", exception) { Object = logObject });
        }



        private void TryApplicationRegister()
        {
            if (CurrentContainer != null && CurrentContainer.HasInjected)
            {
                OnTryApplicationRegister(CurrentContainer.Application);    
            }
        }

        private void ApplicationStarted()
        {
            if (CurrentContainer != null && CurrentContainer.HasInjected)
            {
                OnApplicationStarted(CurrentContainer.ApplicationInstance);    
            }
        }

        private void SharedApplicationClosed()
        {
            if (CurrentContainer != null && CurrentContainer.HasInjected)
            {
                OnApplicationClosed(CurrentContainer.ApplicationInstance);    
            }
        }


        protected virtual void OnTryApplicationRegister(Application application)
        {
            if (!_followApplication)
            {
                return;
            }

            Push(application);
        }

        protected virtual void OnApplicationStarted(ApplicationInstance applicationInstance)
        {
            if (!_followApplication)
            {
                return;
            }

            Push(new KeyValueContainer<ProcessType>(ProcessType.Insert, applicationInstance));
        }

        protected virtual void OnApplicationClosed(ApplicationInstance applicationInstance)
        {
            if (!_followApplication)
            {
                return;
            }

            Push(new KeyValueContainer<ProcessType>(ProcessType.Update, applicationInstance));
        }

        protected void OnPushSystemNotify(string message, LogType logType, int logLevel)
        {
            LogManager.Instance.OnCallSystemNotify(message, logLevel, logType);
        }


        public virtual void Dispose()
        {
            ApplicationShared.ApplicationClosed -= SharedApplicationClosed;
        }
    }
}

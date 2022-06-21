using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Exceptions;
using XrmEarth.Logger.Logger;
using XrmEarth.Logger.Target;

namespace XrmEarth.Logger
{
    /// <summary>
    /// Performs log system management. Logs to specified targets.
    /// <para></para>
    /// For configuration settings, first use the InitConfiguration static class. (XrmEarth.Logger.Configuration.InitConfiguration)
    /// <para></para>
    /// Loggers are created with the 'CreateLogger' methods after the configuration settings are made. Then, according to usage, these loggers can be used by using 'RegisterAll', 'Register', 'Unregister', 'UnregisterAll', 'ClearLoggers' methods.
    /// binds to types. In this way, warning and information logs can be sent to the SQL environment, while error-type logs can be sent by both SQL and Mail.
    /// <para></para>
    /// After the registration process is completed, you can perform your logging operations using the 'Push' methods.
    /// </summary>
    public class LogManager
    {
        #region - SingleTon -
        private class Nested
        {
            internal static readonly LogManager Instance;

            static Nested()
            {
                Instance = new LogManager();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static LogManager Instance
        {
            get { return Nested.Instance; }
        }
        #endregion

        internal readonly Dictionary<LogType, HashSet<CoreLogger>> RegisteredLoggers = new Dictionary<LogType, HashSet<CoreLogger>>();

        internal CoreLogger CreateCoreLogger(LogTarget target)
        {
            return target.CreateLogger();
        }

        internal CoreLogger CreateCoreLogger(IConnection connection)
        {
            var connType = connection.GetType();
            var attr = connType.GetCustomAttribute<DefaultTargetAttribute>();
            if (attr == null)
            {
                throw new Exception(string.Format("'{0}' no default destination defined for connection. ([DefaultTarget(typeof(LogTarget))])", connType.Name));
            }

            if (!attr.IsValid())
            {
                throw new Exception(string.Format("'{0}' connection default target defined invalid. Target must not be null and must derive from class '{1}'.", connType.Name, typeof(LogTarget).Name));
            }
            var lTarget = (LogTarget) Activator.CreateInstance(attr.Target);
            lTarget.Connection = connection;
            return lTarget.CreateLogger();
        }

        internal T CreateGenericLogger<T>(LogTarget target) where T : CoreLogger
        {
            return (T) CreateCoreLogger(target);
        }


        internal bool RegisterLogger(LogType logType, CoreLogger logger)
        {
            HashSet<CoreLogger> hs;
            if (!RegisteredLoggers.ContainsKey(logType))
            {
                hs = new HashSet<CoreLogger>();
                RegisteredLoggers[logType] = hs;
            }
            else
            {
                hs = RegisteredLoggers[logType];
            }

            return hs.Add(logger);
        }
        internal bool UnregisterLogger(LogType logType, CoreLogger logger)
        {
            if (RegisteredLoggers.ContainsKey(logType))
            {
                return RegisteredLoggers[logType].Remove(logger);
            }
            return false;
        }
        internal bool UnregisterLogger(CoreLogger logger)
        {
            var isRemoved = false;
            foreach (var loggers in RegisteredLoggers.Values)
            {
                if (loggers.Remove(logger))
                {
                    isRemoved = true;
                }
            }
            return isRemoved;
        }
        internal List<CoreLogger> GetRegisteredLoggers(LogType logType)
        {
            if (RegisteredLoggers.ContainsKey(logType))
            {
                return RegisteredLoggers[logType].ToList();
            }
            return null;
        }
        internal List<CoreLogger> GetRegisteredLoggers()
        {
            var loggers = new HashSet<CoreLogger>();
            foreach (var key in RegisteredLoggers.Keys)
            {
                foreach (var coreLogger in RegisteredLoggers[key])
                {
                    loggers.Add(coreLogger);
                }
            }
            return loggers.ToList();
        }
        internal bool ClearLoggers(LogType logType)
        {
            if (RegisteredLoggers.ContainsKey(logType))
            {
                RegisteredLoggers[logType].Clear();
                return true;
            }
            return false;
        }

        internal void Push(LogType logType, object value, [CallerMemberName] string memberName = "")
        {
            if (RegisteredLoggers.ContainsKey(logType))
            {
                foreach (var logger in RegisteredLoggers[logType])
                {
                    logger.Push(value, memberName);
                }
            }
        }


        internal void OnCallUnhandledException(UnhandledException unhandledException)
        {
            if(UnhandledException != null)
                UnhandledException(unhandledException);
        }

        internal void OnCallApplicationClosing(ApplicationContainer container, InstanceSummary summary)
        {
            if(ApplicationClosing != null)
                ApplicationClosing(summary);
        }

        internal void OnCallApplicationInjectFailedException(Exception exception)
        {
            if (ApplicationInjectFailedException != null)
                ApplicationInjectFailedException(exception);
        }

        internal void OnCallSystemNotify(string message, int logLevel, LogType logType, bool initializeCompleted = true)
        {
            if(SystemNotify != null)
                SystemNotify(new SystemNotifyArgs{InitializeCompleted = initializeCompleted, Message = message, Level = logLevel, Type = logType});
        }


        public event Action<Exception> ApplicationInjectFailedException = delegate { };
        public event Action<UnhandledException> UnhandledException = delegate { };
        public event Action<InstanceSummary> ApplicationClosing = delegate { };
        public event Action<SystemNotifyArgs> SystemNotify = delegate { };



        #region - Static [ PUBLIC ACCESSIBLE ] -

        /// <summary>
        /// Creates a logger over the connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static CoreLogger CreateLogger(IConnection connection)
        {
            return Instance.CreateCoreLogger(connection);
        }

        /// <summary>
        /// Creates a logger on the target.
        /// </summary>
        /// <typeparam name="T">It must derive from the CoreLogger object.</typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T CreateLogger<T>(LogTarget target) where T : CoreLogger
        {
            return (T)Instance.CreateCoreLogger(target);
        }

        /// <summary>
        /// The specified logger will record all types of logs, so the logger will send all logs to the system it is connected to.
        /// </summary>
        /// <param name="logger"></param>
        public static void RegisterAll(CoreLogger logger)
        {
            foreach (var logType in Enum.GetValues(typeof(LogType)))
            {
                Register((LogType) logType, logger);
            }
        }
        /// <summary>
        /// It binds the specified logger to the given type, so that the logs sent in the same type are sent to the system by the specified logger.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logger"></param>
        /// <returns>Kayıt işlemi sonucu</returns>
        public static bool Register(LogType logType, CoreLogger logger)
        {
            HashSet<CoreLogger> hs;
            if (!Instance.RegisteredLoggers.ContainsKey(logType))
            {
                hs = new HashSet<CoreLogger>();
                Instance.RegisteredLoggers[logType] = hs;
            }
            else
            {
                hs = Instance.RegisteredLoggers[logType];
            }

            return hs.Add(logger);
        }
        /// <summary>
        /// Disconnects the specified logger from the given type.
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool Unregister(LogType logType, CoreLogger logger)
        {
            if (Instance.RegisteredLoggers.ContainsKey(logType))
            {
                return Instance.RegisteredLoggers[logType].Remove(logger);
            }
            return false;
        }
        /// <summary>
        /// The specified logger clears all types, so that the logger is disabled something.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool UnregisterAll(CoreLogger logger)
        {
            var isRemoved = false;
            foreach (var loggers in Instance.RegisteredLoggers.Values)
            {
                if (loggers.Remove(logger))
                {
                    isRemoved = true;
                }
            }
            return isRemoved;
        }
        public static List<CoreLogger> GetLoggers(LogType logType)
        {
            return Instance.GetRegisteredLoggers(logType);
        }
        public static List<CoreLogger> GetLoggers()
        {
            return Instance.GetRegisteredLoggers();
        }
        /// <summary>
        /// Clears definitions for all types.
        /// <para></para>
        /// <code>After this method is called, any 'Push' operation called from 'LogManager' will not send.</code>
        /// </summary>
        public static void ClearLoggers()
        {
            foreach (var logType in Instance.RegisteredLoggers.Keys)
            {
                Instance.ClearLoggers(logType);
            }
        }

        /// <summary>
        /// Logs in information type.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void Info(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Info, logLevel, tag1, tag2, memberName);
        }

        /// <summary>
        /// It throws a warning type log.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void Warning(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Warning, logLevel, tag1, tag2, memberName);
        }

        /// <summary>
        /// It throws an error type log.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void Error(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Error, logLevel, tag1, tag2, memberName);
        }

        /// <summary>
        /// Throws log of object type. (You can use this type to store data or object)
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void Object(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Object, logLevel, tag1, tag2, memberName);
        }

        /// <summary>
        /// It throws a status type log.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void State(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.State, logLevel, tag1, tag2, memberName);
        }

        /// <summary>
        /// It logs in detail information type. (Usually it is only used for special cases, for example, you are processing 1000 records and you can use this type when logging for each record)
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void Trace(
            string message
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            Push(message, LogType.Trace, logLevel, tag1, tag2, memberName);
        }


        /// <summary>
        /// Logs.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="logType">Log type</param>
        /// <param name="logLevel">The level you can use to prioritize your logs within your application.</param>
        /// <param name="tag1">Tag 1 can be used to categorize your logs or store data.</param>
        /// <param name="tag2">Tag 2, you can use it to categorize your logs or store data.</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void Push(
            string message
            , LogType logType
            , int? logLevel = null
            , string tag1 = null
            , string tag2 = null
            , [CallerMemberName]string memberName = "")
        {
            var al = new ApplicationLog
            {
                Message = message,
                Type = logType,
                LogLevel = logLevel,
                Tag1 = tag1,
                Tag2 = tag2
            };

            Instance.Push(logType, al, memberName);
        }



        /// <summary>
        /// It throws an error type object log.
        /// </summary>
        /// <param name="value">Object to send. (There must be a Renderer defined to the object on the Logger. See XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void PushError(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Error);
        }

        /// <summary>
        /// Sets an object log of information type.
        /// </summary>
        /// <param name="value">Object to send. (There must be a Renderer defined to the object on the Logger. See XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void PushInfo(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Info);
        }

        /// <summary>
        /// Sets a status type object log.
        /// </summary>
        /// <param name="value">Object to send. (There must be a Renderer defined to the object on the Logger. See XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void PushState(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.State);
        }

        /// <summary>
        /// Sets object log in detail information type. (Usually it is only used for special cases, for example, you are processing 1000 records and you can use this type when logging for each record)
        /// </summary>
        /// <param name="value">Object to send. (There must be a Renderer defined to the object on the Logger. See XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void PushTrace(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Trace);
        }

        /// <summary>
        /// Throws a warning type object log.
        /// </summary>
        /// <param name="value">Object to send. (There must be a Renderer defined to the object on the Logger. See XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void PushWarnig(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Warning);
        }


        /// <summary>
        /// Throws the object log.
        /// </summary>
        /// <param name="value">Object to send. (There must be a Renderer defined to the object on the Logger. See XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="logType">Log type</param>
        /// <param name="memberName">If you leave this parameter blank, it will automatically retrieve the name of the calling method.</param>
        public static void PushObject(object value, LogType logType = LogType.Object, [CallerMemberName] string memberName = "")
        {
            Instance.Push(logType, value);
        }
        

        #endregion - Static [ PUBLIC ACCESSIBLE ] -
    }
}

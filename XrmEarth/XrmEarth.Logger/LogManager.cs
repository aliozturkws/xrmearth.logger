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
    /// Log sistemi yönetimini yapar. Belirtilen hedeflere log atar.
    /// <para></para>
    /// Yapılandırma ayarları için önce InitConfiguration static sınıfını kullanın. (XrmEarth.Logger.Configuration.InitConfiguration)
    /// <para></para>
    /// Yapılandırma ayarları yapıldıktan sonra 'CreateLogger' metodlarıyla Loglayıcılar oluşturulur. Daha sonra sonrasında kullanıma göre bu loglayıcılar 'RegisterAll', 'Register', 'Unregister', 'UnregisterAll', 'ClearLoggers' metodları kullanılarak
    /// tiplere bağlanır. Bu sayede Uyarı ve bilgi logları SQL ortamına atılırken, hata tipindeki logların hem SQL hemde Mail ile gönderimi sağlanabilir.
    /// <para></para>
    /// Kayıt işlemleri de tamamlandıktan sonra 'Push' metodlarını kullanarak loglama işlemlerinizi gerçekleştirebilirsiniz.
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
                throw new Exception(string.Format("'{0}' bağlantısı için varsayılan hedef tanımlanmamış. ([DefaultTarget(typeof(LogTarget))])", connType.Name));
            }

            if (!attr.IsValid())
            {
                throw new Exception(string.Format("'{0}' bağlantısı varsayılan hedef geçersiz tanımlanmış. Hedef boş olmamalıdır ve '{1}' sınıfından türemiş olmalıdır.", connType.Name, typeof(LogTarget).Name));
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
        /// Bağlantı üzerinden loglayıcı oluşturur.
        /// </summary>
        /// <param name="connection">Bağlantı</param>
        /// <returns></returns>
        public static CoreLogger CreateLogger(IConnection connection)
        {
            return Instance.CreateCoreLogger(connection);
        }

        /// <summary>
        /// Hedef üzerinden loglayıcı oluşturur.
        /// </summary>
        /// <typeparam name="T">CoreLogger nesnesinden türemiş olmalıdır.</typeparam>
        /// <param name="target">Hedef</param>
        /// <returns></returns>
        public static T CreateLogger<T>(LogTarget target) where T : CoreLogger
        {
            return (T)Instance.CreateCoreLogger(target);
        }

        /// <summary>
        /// Belirtilen loglayıcı bütün tiplerdeki logları kaydedecektir, bu sayede loglayıcı bütün logları bağlı olduğu sisteme gönderecektir.
        /// </summary>
        /// <param name="logger">Loglayıcı</param>
        public static void RegisterAll(CoreLogger logger)
        {
            foreach (var logType in Enum.GetValues(typeof(LogType)))
            {
                Register((LogType) logType, logger);
            }
        }
        /// <summary>
        /// Belirtilen loglayıcıyı verilen tipe bağlar, bu sayede aynı tipte gönderilen loglar belirtilmiş loglayıcı tarafından sisteme gönderilir.
        /// </summary>
        /// <param name="logType">Log tipi</param>
        /// <param name="logger">Loglayıcı</param>
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
        /// Belirtilen loglayıcıyı verilen tiple bağlılığını keser.
        /// </summary>
        /// <param name="logType">Log tipi</param>
        /// <param name="logger">Loglayıcı</param>
        /// <returns>Silme işlemi sonucu</returns>
        public static bool Unregister(LogType logType, CoreLogger logger)
        {
            if (Instance.RegisteredLoggers.ContainsKey(logType))
            {
                return Instance.RegisteredLoggers[logType].Remove(logger);
            }
            return false;
        }
        /// <summary>
        /// Belirtilen loglayıcı bütün tiplerden temizler, bu sayede loglayıcı bir neyi devre dışı bırakılmış olur.
        /// </summary>
        /// <param name="logger">Loglayıcı</param>
        /// <returns>Silme işlemi sonucu</returns>
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
        /// Bütün tipler için tanımlamaları temizler.
        /// <para></para>
        /// <code>Bu metod çağırıldıktan sonra 'LogManager' üzerinden çağırılan hiçbir 'Push' işlemi gönderim yapmayacaktır.</code>
        /// </summary>
        public static void ClearLoggers()
        {
            foreach (var logType in Instance.RegisteredLoggers.Keys)
            {
                Instance.ClearLoggers(logType);
            }
        }

        /// <summary>
        /// Bilgi tipinde log atar.
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Uyarı tipinde log atar.
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Hata tipinde log atar.
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Nesne tipinde log atar. (Veri veya nesne saklamak için bu tipi kullanabilirsiniz)
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Durum tipinde log atar.
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Detay bilgi tipinde log atar. (Genellikle sadece özel durumlar için kullanılır, örneğin; 1000 kayıt üzerinde işlem yapıyorsunuz ve her kayıt için log atarken bu tipi kullanabilirsiniz)
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Log atar.
        /// </summary>
        /// <param name="message">Mesaj</param>
        /// <param name="logType">Log tipi</param>
        /// <param name="logLevel">Seviye, uygulamanız içerisinde loglarınızı önceliklendirmek için kullanabilirsiniz.</param>
        /// <param name="tag1">Etiket 1, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="tag2">Etiket 2, loglarınızı kategorize etmek veya veri saklamak için kullanabilirsiniz.</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
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
        /// Hata tipinde nesne logu atar.
        /// </summary>
        /// <param name="value">Gönderilecek nesne. (Logger üzerinde nesneye tanımlanmış Renderer bulunmalıdır. Bkz. XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
        public static void PushError(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Error);
        }

        /// <summary>
        /// Bilgi tipinde nesne logu atar.
        /// </summary>
        /// <param name="value">Gönderilecek nesne. (Logger üzerinde nesneye tanımlanmış Renderer bulunmalıdır. Bkz. XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
        public static void PushInfo(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Info);
        }

        /// <summary>
        /// Durum tipinde nesne logu atar.
        /// </summary>
        /// <param name="value">Gönderilecek nesne. (Logger üzerinde nesneye tanımlanmış Renderer bulunmalıdır. Bkz. XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
        public static void PushState(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.State);
        }

        /// <summary>
        /// Detay bilgi tipinde nesne logu atar. (Genellikle sadece özel durumlar için kullanılır, örneğin; 1000 kayıt üzerinde işlem yapıyorsunuz ve her kayıt için log atarken bu tipi kullanabilirsiniz)
        /// </summary>
        /// <param name="value">Gönderilecek nesne. (Logger üzerinde nesneye tanımlanmış Renderer bulunmalıdır. Bkz. XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
        public static void PushTrace(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Trace);
        }

        /// <summary>
        /// Uyarı tipinde nesne logu atar.
        /// </summary>
        /// <param name="value">Gönderilecek nesne. (Logger üzerinde nesneye tanımlanmış Renderer bulunmalıdır. Bkz. XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
        public static void PushWarnig(object value, [CallerMemberName] string memberName = "")
        {
            PushObject(value, LogType.Warning);
        }


        /// <summary>
        /// Nesne logu atar.
        /// </summary>
        /// <param name="value">Gönderilecek nesne. (Logger üzerinde nesneye tanımlanmış Renderer bulunmalıdır. Bkz. XrmEarth.Logger.Renderer.IRenderer)</param>
        /// <param name="logType">Log tipi</param>
        /// <param name="memberName">Bu parametreyi boş geçerseniz, otomatik olarak çağıran metodun ismini çeker.</param>
        public static void PushObject(object value, LogType logType = LogType.Object, [CallerMemberName] string memberName = "")
        {
            Instance.Push(logType, value);
        }
        

        #endregion - Static [ PUBLIC ACCESSIBLE ] -
    }
}

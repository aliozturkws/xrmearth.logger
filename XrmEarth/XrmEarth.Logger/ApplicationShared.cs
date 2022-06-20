using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using XrmEarth.Logger.Configuration;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Exceptions;

namespace XrmEarth.Logger
{
    /// <summary>
    /// Sonraki sürümlerde Public olarak açılacak. Kütüphane iç dinamiklerine buradan müdahele edilecek.
    /// </summary>
    public static class ApplicationShared
    {
        static ApplicationShared()
        {
            CultureInfo = CultureInfo.InvariantCulture;

            ConnectionComparers = new Dictionary<Type, IConnectionComparer>
            {
                {typeof (MssqlConnection), new MssqlConnectionComparer()},
                {typeof (ConsoleConnection), new ConsoleConnectionComparer()},
                {typeof(FileConnection), new FileConnectionComparer() },
            };

            SharedInstances = new Dictionary<int, ApplicationContainer>();

            if (!InitConfiguration.InjectApplication)
            {
                if (InitConfiguration.OverrideAssembly != null)
                    Summary = new ApplicationSummary(InitConfiguration.OverrideAssembly);
                return;
            }

            AppDomain.CurrentDomain.ProcessExit += OnApplicationClosed;
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args)
            {
                var exception = args.ExceptionObject as Exception;
                if (exception != null)
                {
                    LogManager.Instance.OnCallUnhandledException(new UnhandledException(exception.Message, exception, args.IsTerminating, UnhandledExceptionType.Application));
                }
                else
                {
                    LogManager.Instance.OnCallUnhandledException(new UnhandledException("Beklenmedik bir hata meydana geldi ve hata tespit edilemedi. Nesne için 'UnhandledException.Object' özelliğine bakın.", null, args.IsTerminating, UnhandledExceptionType.Application) { Object = args.ExceptionObject });
                }
            };


            if (InitConfiguration.OverrideAssembly != null)
            {
                Summary = new ApplicationSummary(InitConfiguration.OverrideAssembly);
            }
            else
            {
                Summary = new ApplicationSummary();

                var callerPath = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var asmPath = assembly.Location;

                    if (callerPath == asmPath)
                    {
                        LogManager.Instance.OnCallSystemNotify(string.Format("Takip edilecek uygulama revize edildi. \r\nEski Uygulama: {0} \r\nYeni Uygulama: {1}", Summary.Assembly.Location, callerPath), 99, LogType.Warning, false);
                        Summary = new ApplicationSummary(assembly);
                    }
                }
            }

            //Summary = InitConfiguration.OverrideAssembly != null ? new ApplicationSummary(InitConfiguration.OverrideAssembly) : new ApplicationSummary(Assembly.GetCallingAssembly());
        }

        public static CultureInfo CultureInfo { get; set; }
        public static Dictionary<Type, IConnectionComparer> ConnectionComparers { get; private set; }
        public static Dictionary<int, ApplicationContainer> SharedInstances { get; private set; }

        private static Process _process;
        internal static Process Process
        {
            get
            {
                if (_process == null && InitConfiguration.InjectApplication)
                {
                    _process = Process.GetCurrentProcess();
                }

                return _process;
            }
        }
        public static ApplicationSummary Summary { get; internal set; }

        private static readonly DateTime StartTime = DateTime.Now;
        /// <summary>
        /// Uygulamanın başlama zamanı. Bir şekilde Process bilgisine erişilemezse, ilk erişim tarihini baz alarak başlama zamanı döner.
        /// </summary>
        public static DateTime SafeStartTime
        {
            get
            {
                if (Process == null || !InitConfiguration.InjectApplication)
                {
                    return StartTime;
                }
                else
                {
                    return Process.StartTime;
                }
            }
        }

        internal static IConnectionComparer GetConnectionComparer(IConnection connection)
        {
            if (ConnectionComparers == null || connection == null)
                return null;

            var conType = connection.GetType();
            if (!ConnectionComparers.ContainsKey(conType))
                return null;

            return ConnectionComparers[conType];
        }

        /// <summary>
        /// Gönderilen bağlantıyla ilişkilenmiş 'ApplicationContainer'i döner.<para/>
        /// Daha öncesinde bağlantı eşleştiricisinin 'ConnectionComparers' listesine kaydedilmiş olması gerekir, aksi takdirde yeni 'ApplicationContainer' nesnesi oluşturulup dönecektir.<para/>
        /// Varsıyalın eşleştiriciler eklenmiştir. 'MssqlConnection -> MssqlConnectionComparer', 'FileConnection -> FileConnectionComparer', 'ConsoleConnection -> ConsoleConnectionComparer'.<para/>
        /// <code>Not: XrmEarth.Logger kütüphanesi 'CrmConnection' eşleştiricisini otomatik ekler.</code>
        /// </summary>
        /// <param name="connection">Bağlantı</param>
        /// <returns></returns>
        public static ApplicationContainer GetApplicationContainer(IConnection connection)
        {
            var comp = GetConnectionComparer(connection);
            if (comp == null)
                return new ApplicationContainer { SourceConnection = connection };

            var key = comp.GetHashCode(connection);
            if (!SharedInstances.ContainsKey(key))
            {
                SharedInstances[key] = new ApplicationContainer { SourceConnection = connection };
            }
            return SharedInstances[key];
        }

        private static void OnApplicationClosed(object sender, EventArgs e)
        {
            var insSum = new InstanceSummary();
            LogManager.Instance.OnCallApplicationClosing(null, insSum);
            foreach (var instance in SharedInstances.Values)
            {
                instance.OnApplicationClosing();
                instance.ApplicationInstance.Summary = insSum.Summary;
                instance.ApplicationInstance.Result = insSum.Result;
            }
            ApplicationClosed();
        }

        public static event Action ApplicationClosed = delegate { };
    }
}
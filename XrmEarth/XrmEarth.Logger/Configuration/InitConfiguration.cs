using System.Reflection;
using System.Text;

namespace XrmEarth.Logger.Configuration
{
    /// <summary>
    /// Configuration settings.
    /// <para></para>
    /// <code>Note: It must be configured before logging operations.</code>
    /// </summary>
    public static class InitConfiguration
    {
        static InitConfiguration()
        {
            InjectApplication = true;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// The application you want the system to follow.
        /// <para></para>
        /// <code>Note: Since the system will now follow this application, the information of the current assembly will not be stored.</code>
        /// </summary>
        public static Assembly OverrideAssembly { get; set; }

        /// <summary>
        /// Will the application be tracked? (Default value is True)
        /// <para></para>
        /// By following the application; time it runs, time it shuts down, startup arguments, etc. information is stored.
        /// <para></para>
        /// <para></para>
        /// <code>!!!NOTE: Sandbox etc. It must be set to False for such uses.</code>
        /// </summary>
        public static bool InjectApplication { get; set; }

        public static Encoding Encoding { get; set; }
    }
}

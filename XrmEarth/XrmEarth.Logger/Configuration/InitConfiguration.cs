using System.Reflection;
using System.Text;

namespace XrmEarth.Logger.Configuration
{
    /// <summary>
    /// Yapılandırma ayarları.
    /// <para></para>
    /// <code>Not: Log atma işlemlerinden önce yapılandırılması gerekir.</code>
    /// </summary>
    public static class InitConfiguration
    {
        static InitConfiguration()
        {
            InjectApplication = true;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Sistemin takip etmesini istediğiniz uygulama.
        /// <para></para>
        /// <code>Not: Sistem artık bu uygulamayı takip edeceği için mevcut assembly'ye ait bilgiler saklanmayacaktır.</code>
        /// </summary>
        public static Assembly OverrideAssembly { get; set; }

        /// <summary>
        /// Uygulama takibi yapılacak mı? (Varsayılan değer, True)
        /// <para></para>
        /// Uygulamayı takip edilerek; çalıştığı saat, kapandığı saat, başlangıç argümanları vb. bilgiler saklanır. 
        /// <para></para>
        /// <para></para>
        /// <code>!!!NOT: Sandbox vb. gibi kullanımlar için False olarak atanması gerekir.</code>
        /// </summary>
        public static bool InjectApplication { get; set; }

        public static Encoding Encoding { get; set; }
    }
}

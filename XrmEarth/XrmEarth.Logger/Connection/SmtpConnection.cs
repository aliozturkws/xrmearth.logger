using System;
using XrmEarth.Logger.Target;

namespace XrmEarth.Logger.Connection
{
    [DefaultTarget(typeof(SmtpLogTarget))]
    public class SmtpConnection : IConnection
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        /// <summary>
        /// Format;<para/>
        /// From Name, From Address<para/>
        /// <code>Örn: Ali Öztürk, test@gmail.com<para/></code>
        /// </summary>
        public Tuple<string, string> FromAddress { get; set; }
        /// <summary>
        /// Format;
        /// <para/>
        /// From Name, From Address
        /// <para/>
        /// <code>Örn: [{Ali Öztürk, test@gmail.com}]</code>
        /// </summary>
        public Tuple<string, string>[] ToAddresses { get; set; }

        
    }
}

using Newtonsoft.Json;
using System.ComponentModel;
using System.Xml.Serialization;
using XrmEarth.Logger.Target;
using XrmEarth.Logger.Utility;

namespace XrmEarth.Logger.Connection
{
    [DefaultTarget(typeof(MssqlLogTarget))]
    public class MssqlConnection : IConnection
    {
        public string Server { get; set; }
        public string Database { get; set; }

        public bool TrustedConnection { get; set; }
        public string Authentication { get; set; }
        public bool Encrypt { get; set; }

        public string Username { get; set; }
        [XmlIgnore]
        [JsonIgnore]
        public string Password { get; set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string CryptedPassword
        {
            get { return CryptHelper.Crypt(Password); }
            set { Password = CryptHelper.Decrypt(value); }
        }


        public string CreateConnectionString()
        {
            if (TrustedConnection)
            {
                return string.Format("Server={0};Database={1};Trusted_Connection=SSPI;",
                    Server,
                    Database);
            }
            else
            {
                return $"Server={Server};Database={Database};User Id={Username};Password={Password};" +
                       $"{(!string.IsNullOrEmpty(Authentication) ? $"Authentication={Authentication};" : string.Empty)}" +
                       $"{(Encrypt ? "Encrypt=True;" : string.Empty)}";
            }
        }
    }
}

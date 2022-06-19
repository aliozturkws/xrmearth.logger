using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;
using System.Net;

namespace XrmEarth.Samples
{
    public class XrmConnection
    {
        private static CrmServiceClient _crmClient = null;

        private static readonly object lockthread = new object();

        private static DateTime _tokenExpireTime = DateTime.Now;

        public static CrmServiceClient AdminCrmClient
        {
            get
            {
                lock (lockthread)
                {
                    if (_crmClient == null || DateTime.Now.AddMinutes(10) >= _tokenExpireTime)
                    {
                        _crmClient = CreateAdminClient();

                        var adminServiceProxy = _crmClient.GetOrganizationService() as OrganizationServiceProxy;

                        if (adminServiceProxy != null && adminServiceProxy.SecurityTokenResponse != null && adminServiceProxy.SecurityTokenResponse.Token != null)
                        {
                            _tokenExpireTime = adminServiceProxy.SecurityTokenResponse.Token.ValidTo;
                        }

                        if (_tokenExpireTime <= DateTime.Now)
                        {
                            _tokenExpireTime = DateTime.Now.AddMinutes(60);
                        }
                    }

                    return _crmClient;
                }
            }
        }

        public static CrmServiceClient CreateAdminClient(Guid? callerId = null)
        {
            string organizationName = ConfigurationManager.AppSettings["OrganizationName"];
            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

            var crmClient = CreateCrmServiceClientOnlineByAppUser(organizationName, clientId, clientSecret);

            return crmClient;
        }

        public static CrmServiceClient CreateCrmServiceClientOnlineByAppUser(string organizationName, string clientId, string clientSecret, Guid? callerId = null, bool requireNewInstance = false)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException("clientId", "clientId parametresi NULL olamaz.");
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException("clientSecret", "clientSecret parametresi NULL olamaz.");
            }

            if (string.IsNullOrEmpty(organizationName))
            {
                throw new ArgumentNullException("organizationName", "organizationName parametresi NULL olamaz.");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            CrmServiceClient result = null;

            var connectionString = $"AuthType=ClientSecret;Url=https://{organizationName}.crm4.dynamics.com;ClientId={clientId};ClientSecret={clientSecret};";

            if (requireNewInstance || (callerId.HasValue && !callerId.Value.Equals(Guid.Empty)))
            {
                connectionString += " RequireNewInstance=true;";
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                result = new CrmServiceClient(connectionString);

                if (result.IsReady)
                {
                    CrmServiceClient.MaxConnectionTimeout = new TimeSpan(0, 5, 0);

                    if (callerId.HasValue && !callerId.Value.Equals(Guid.Empty))
                    {
                        result.CallerId = callerId.Value;
                    }
                }
                else
                {
                    throw result.LastCrmException;
                }
            }

            return result;
        }
    }

    public static class CrmServiceClientExtensions
    {
        public static IOrganizationService GetOrganizationService(this CrmServiceClient crmConnection)
        {
            IOrganizationService service = null;
            if (crmConnection != null && crmConnection.IsReady)
            {
                if (crmConnection.OrganizationServiceProxy != null)
                {
                    service = (IOrganizationService)crmConnection.OrganizationServiceProxy;
                }
                else if (crmConnection.OrganizationWebProxyClient != null)
                {
                    service = (IOrganizationService)crmConnection.OrganizationWebProxyClient;
                }
            }

            return service;
        }
    }
}

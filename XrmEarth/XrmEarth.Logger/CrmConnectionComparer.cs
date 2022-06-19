using System;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using XrmEarth.Logger.Connection;

namespace XrmEarth.Logger
{
    public class CrmConnectionComparer : IConnectionComparer
    {
        public bool Equals(IConnection x, IConnection y)
        {
            return false;
            //var connection1 = x as CrmConnection;
            //var connection2 = y as CrmConnection;

            //if (connection1 == null || connection2 == null)
            //{
            //    if (connection1 == null && connection2 == null)
            //    {
            //        return true;
            //    }
            //    return false;
            //}

            //var endpoint1 = GetEndpoint(connection1.Service);
            //var endpoint2 = GetEndpoint(connection2.Service);

            //if (endpoint1 == null || endpoint2 == null)
            //{
            //    if (endpoint1 == null && endpoint2 == null)
            //    {
            //        return true;
            //    }
            //    return false;
            //}

            //var uri1 = endpoint1.ListenUri.ToString();
            //var uri2 = endpoint2.ListenUri.ToString();

            //return uri1.Equals(uri2, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(IConnection obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
            //var connection = obj as CrmConnection;
            //if (connection == null) return 0;
            //var endpoint = GetEndpoint(connection.Service);

            //return endpoint != null ? endpoint.ListenUri.ToString().GetHashCode() : 0;
        }

        public ServiceEndpoint GetEndpoint(IOrganizationService service)
        {
            return null;
            //var serviceProxy = service as ServiceProxy<IOrganizationService>;
            //return serviceProxy == null ? null : serviceProxy.ServiceManagement.CurrentServiceEndpoint;
        }
    }
}

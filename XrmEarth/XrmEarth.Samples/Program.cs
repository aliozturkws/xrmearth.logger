using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmEarth.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient adminClientService = XrmConnection.AdminCrmClient;

            IOrganizationService orgService = adminClientService.GetOrganizationService();


        }
    }
}

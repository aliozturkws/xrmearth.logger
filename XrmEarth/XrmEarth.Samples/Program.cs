using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmEarth.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient adminClientService = XrmConnection.AdminCrmClient;

            IOrganizationService service = adminClientService.GetOrganizationService();

            SimpleCrm simpleCrm = new SimpleCrm(service);
            simpleCrm.Run();
        }
    }
}
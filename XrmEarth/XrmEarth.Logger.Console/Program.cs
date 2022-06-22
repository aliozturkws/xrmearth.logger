using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmEarth.Logger.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmServiceClient adminClientService = XrmConnection.AdminCrmClient;

            IOrganizationService service = adminClientService.GetOrganizationService();

            SimpleCrm simpleCrm = new SimpleCrm(service);
            simpleCrm.Run();

            System.Console.ReadKey();
        }
    }
}
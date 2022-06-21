using Microsoft.Xrm.Sdk;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Target;

namespace XrmEarth.Logger
{
    [DefaultTarget(typeof(CrmTarget))]
    public class CrmConnection : IConnection
    {
        static CrmConnection()
        {
            if (ApplicationShared.ConnectionComparers != null) ApplicationShared.ConnectionComparers[typeof(CrmConnection)] = new CrmConnectionComparer();
            //ApplicationShared.ConnectionComparers.Add(typeof(CrmConnection), new CrmConnectionComparer());
            //ApplicationShared.ConnectionComparers.Add(typeof(CrmConnection), new CrmConnectionCustomComparer());
        }

        public CrmConnection()
        {

        }

        public CrmConnection(IOrganizationService service)
        {
            Service = service;
        }

        public IOrganizationService Service { get; set; }
    }
}

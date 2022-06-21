using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Logger;

namespace XrmEarth.Logger
{
    public class CrmLogger : BaseLogger<CrmConnection, CrmRenderer>, XrmEarth.Logger.Initializer.ILogEnvironmentInitializer
    {
        public CrmLogger(CrmConnection connection, CrmRenderer renderer, bool followApplication = true) : base(connection, renderer, followApplication)
        {

        }
        public CrmLogger(CrmConnection connection, bool followApplication = true) : base(connection, new CrmRenderer(), followApplication)
        {

        }
        public CrmLogger(IOrganizationService service, bool followApplication = true) : base(new CrmConnection(service), new CrmRenderer(), followApplication)
        {

        }

        public string InitializeEnvironment()
        {
            return new InitializerNew(Connection).InitializeEnvironment();
        }

        protected override void OnPush(Dictionary<string, object> keyValuesDictionary)
        {
            var entity = TryGet<Microsoft.Xrm.Sdk.Entity>(keyValuesDictionary, CrmRendererBase.EntityKey);
            if (entity == null)
            {
                OnPushSystemNotify("Logging was canceled because the entity to send could not be found.", LogType.Warning, 100001);
                return;
            }

            if (entity.LogicalName == ApplicationLogicalName)
            {
                var id = TryGetApplicationID(entity);
                if (id != Guid.Empty)
                {
                    CurrentContainer.Application.ID = id;
                    return;
                }
            }

            var requestType = TryGet<RequestType>(keyValuesDictionary, CrmRendererBase.RequestTypeKey);

            switch (requestType)
            {
                case RequestType.Create:
                    entity.Id = Connection.Service.Create(entity);
                    break;
                case RequestType.Update:
                    Connection.Service.Update(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private Guid TryGetApplicationID(Microsoft.Xrm.Sdk.Entity applicationEntity)
        {
            var query = new QueryExpression(applicationEntity.LogicalName)
            {
                ColumnSet = new ColumnSet(LogEntities.GetPrimaryAttributeName(typeof(LogEntities.Application))),
                Criteria = new FilterExpression(LogicalOperator.And)
                {
                    Conditions =
                    {
                        new ConditionExpression(LogEntities.Application.Fields.Identifier, ConditionOperator.Equal, applicationEntity[LogEntities.Application.Fields.Identifier]),
                        new ConditionExpression(LogEntities.Application.Fields.Version, ConditionOperator.Equal, applicationEntity[LogEntities.Application.Fields.Version])
                    }
                },
                NoLock = true,
                TopCount = 1
            };

            var queryResult = Connection.Service.RetrieveMultiple(query);
            if (queryResult.Entities.Any())
            {
                return queryResult.Entities[0].Id;
            }
            return Guid.Empty;
        }

        protected T TryGet<T>(Dictionary<string, object> keyValuesDictionary, string key)
        {
            if (keyValuesDictionary.ContainsKey(key))
            {
                var val = keyValuesDictionary[key];
                if (val != null)
                    return (T)val;
            }
            return default(T);
        }

        static CrmLogger()
        {
            ApplicationLogicalName = "new_application";
            ApplicationInstanceLogicalName = "new_applicationinstance";
            ApplicationLogLogicalName = "new_applicationlog";
        }
        public static string ApplicationLogicalName { get; set; }
        public static string ApplicationInstanceLogicalName { get; set; }
        public static string ApplicationLogLogicalName { get; set; }
    }
}

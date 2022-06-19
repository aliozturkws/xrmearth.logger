using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace XrmEarth.Logger
{
    public class CrmRendererBase : ICrmRenderer
    {
        public const string EntityKey = "CrmRendererBase_Entity";
        public const string RequestTypeKey = "CrmRendererBase_RequestType";
        public const string AttributesKey = "CrmRendererBase_AttributesKey";


        public Dictionary<string, object> RenderObject(object value)
        {
            var reqType = RequestType.Create;
            if (GetRequestType != null)
                reqType = GetRequestType(value);

            return new Dictionary<string, object>
            {
                { EntityKey, GetLogEntity(value)},
                { RequestTypeKey, reqType}
            };
        }

        public Action<object, Dictionary<string, object>> ValidateAction { get; set; }
        public Func<object, Microsoft.Xrm.Sdk.Entity> GetLogEntity { get; set; }
        public Func<object, RequestType> GetRequestType { get; set; }
    }
}

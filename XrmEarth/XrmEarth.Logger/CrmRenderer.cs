using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Renderer.Base;

namespace XrmEarth.Logger
{
    public class CrmRenderer : MultiBaseRenderer<ICrmRenderer>
    {
        public CrmRenderer()
        {
            Register<Application>(new CrmRendererBase
            {
                GetLogEntity  = o =>
                {
                    var app = o as Application;
                    if (app == null)
                        return null;

                    return new Microsoft.Xrm.Sdk.Entity(CrmLogger.ApplicationLogicalName, app.ID)
                    {
                        Attributes =
                        {
                            {LogEntities.Application.Fields.Identifier, app.AssemblyGuid.ToString()},
                            {LogEntities.Application.Fields.Name, app.Name},
                            {LogEntities.Application.Fields.Description, app.Description},
                            {LogEntities.Application.Fields.Version, app.AssemblyVersion},
                            {LogEntities.Application.Fields.Namespace, app.Namespace},
                        }
                    };
                }
            });

            Register<ApplicationInstance>(new CrmMultiKeyRendererImplementation<ProcessType, ICrmRenderer>(new List<KeyValuePair<ProcessType, ICrmRenderer>>
            {
                new KeyValuePair<ProcessType, ICrmRenderer>(ProcessType.Insert, new CrmRendererBase
                {
                    GetLogEntity = o =>
                    {
                        var appIns = o as ApplicationInstance;
                        if (appIns == null)
                            return null;

                        var entity = new Microsoft.Xrm.Sdk.Entity(CrmLogger.ApplicationInstanceLogicalName, appIns.ID)
                        {
                            Attributes =
                            {
                                {LogEntities.ApplicationInstance.Fields.Path, appIns.Path},
                                {LogEntities.ApplicationInstance.Fields.StartAt, appIns.StartAt},
                                {LogEntities.ApplicationInstance.Fields.Parameters, appIns.Parameters},
                            }
                        };

                        if (appIns.ApplicationID != Guid.Empty)
                        {
                            entity.Attributes.Add(new KeyValuePair<string, object>(LogEntities.ApplicationInstance.Fields.AppicationID, new Microsoft.Xrm.Sdk.EntityReference(CrmLogger.ApplicationLogicalName, appIns.ApplicationID)));
                        }

                        return entity;
                    }
                }),
                    new KeyValuePair<ProcessType, ICrmRenderer>(ProcessType.Update, new CrmRendererBase
                    {
                        GetLogEntity = o =>
                        {
                            var appIns = o as ApplicationInstance;
                            if (appIns == null)
                                return null;

                            var entity = new Microsoft.Xrm.Sdk.Entity(CrmLogger.ApplicationInstanceLogicalName, appIns.ID)
                            {
                                Attributes =
                                {
                                    {LogEntities.ApplicationInstance.Fields.Path, appIns.Path},
                                    {LogEntities.ApplicationInstance.Fields.StartAt, appIns.StartAt},
                                    {LogEntities.ApplicationInstance.Fields.Parameters, appIns.Parameters},
                                }
                            };

                            if (appIns.ApplicationID != Guid.Empty)
                            {
                                entity.Attributes.Add(new KeyValuePair<string, object>(LogEntities.ApplicationInstance.Fields.AppicationID, new Microsoft.Xrm.Sdk.EntityReference(CrmLogger.ApplicationLogicalName, appIns.ApplicationID)));
                            }

                            return entity;
                        },
                        GetRequestType = o => RequestType.Update,
                    }) 
            }));

            Register<ApplicationLog>(new CrmRendererBase
            {
                GetLogEntity = o =>
                {
                    var appLog = o as ApplicationLog;
                    if (appLog == null)
                        return null;

                    var entity = new Microsoft.Xrm.Sdk.Entity(CrmLogger.ApplicationLogLogicalName, appLog.ID)
                    {
                        Attributes =
                        {
                            {LogEntities.ApplicationLog.Fields.ParentCallerMember, appLog.ParentCallerMember},
                            {LogEntities.ApplicationLog.Fields.CallerMember, appLog.CallerMember},
                            {LogEntities.ApplicationLog.Fields.Message, appLog.Message},
                            {LogEntities.ApplicationLog.Fields.Type, new OptionSetValue((int)appLog.Type)},
                            {LogEntities.ApplicationLog.Fields.LogLevel, appLog.LogLevel},
                            {LogEntities.ApplicationLog.Fields.Tag1, appLog.Tag1},
                            {LogEntities.ApplicationLog.Fields.Tag2, appLog.Tag2},
                        }
                    };

                    if (appLog.ApplicationInstanceID != Guid.Empty)
                    {
                        entity.Attributes.Add(new KeyValuePair<string, object>(LogEntities.ApplicationLog.Fields.ApplicationInstanceID, new Microsoft.Xrm.Sdk.EntityReference(CrmLogger.ApplicationInstanceLogicalName, appLog.ApplicationInstanceID)));
                    }

                    return entity;
                }
            });
        }
    }
}

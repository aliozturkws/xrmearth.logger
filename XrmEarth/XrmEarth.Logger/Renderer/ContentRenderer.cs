using System.Collections.Generic;
using System.Text;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Renderer.Base;
using XrmEarth.Logger.Renderer.Content;

namespace XrmEarth.Logger.Renderer
{
    public class ContentRenderer : MultiBaseRenderer<IContentRenderer>
    {
        public ContentRenderer()
        {
            Register<Application>(new ContentRendererBase
            {
                GetContent = o =>
                {
                    var app = o as Application;
                    if (app == null)
                        return string.Empty;

                    var sb = new StringBuilder();

                    sb
                        .Append("<APPLICATION>").AppendLine()
                        .Append("ID: ").Append(app.ID).AppendLine()
                        .Append("Name: ").Append(app.Name).AppendLine()
                        .Append("Description: ").Append(app.Description).AppendLine()
                        .Append("Version: ").Append(app.AssemblyVersion).AppendLine()
                        .Append("Namespace: ").Append(app.Namespace).AppendLine()
                        .Append("Guid: ").Append(app.AssemblyGuid).AppendLine()
                        .Append("CreatedAt: ").Append(app.CreatedAt).AppendLine()
                        .Append("</APPLICATION>").AppendLine();

                    return sb.ToString();
                }
            });

            var appInstanceRenderer = new ContentRendererBase
            {
                GetContent = o =>
                {
                    var appIns = o as ApplicationInstance;
                    if (appIns == null)
                        return string.Empty;

                    var sb = new StringBuilder();

                    sb
                        .Append("<APPLICATION_INSTANCE>").AppendLine()
                        .Append("ID: ").Append(appIns.ID).AppendLine()
                        .Append("Application ID: ").Append(appIns.ApplicationID).AppendLine()
                        .Append("Path: ").Append(appIns.Path).AppendLine()
                        .Append("Parameters: ").Append(appIns.Parameters).AppendLine()
                        .Append("Summary: ").Append(appIns.Summary).AppendLine()
                        .Append("Result: ").Append(appIns.Result).AppendLine()
                        .Append("StartAt: ").Append(appIns.StartAt).AppendLine()
                        .Append("FinishAt: ").Append(appIns.FinishAt).AppendLine()
                        .Append("CreatedAt: ").Append(appIns.CreatedAt).AppendLine()
                        .Append("</APPLICATION_INSTANCE>").AppendLine();

                    return sb.ToString();
                }
            };

            Register<ApplicationInstance>(new MultiKeyRenderer<ProcessType, IContentRenderer>(
                new Dictionary<ProcessType, IContentRenderer>
                {
                    {ProcessType.Insert, appInstanceRenderer},
                    {ProcessType.Update, appInstanceRenderer},
                }));
            
            Register<ApplicationLog>(new ContentRendererBase
            {
                GetContent = o =>
                {
                    var appLog = o as ApplicationLog;
                    if (appLog == null)
                        return string.Empty;

                    var sb = new StringBuilder();

                    sb
                        .Append("<APPLICATION_LOG>").AppendLine()
                        .Append(appLog.Type.ToString().ToUpper()).AppendLine()
                        .Append("ID: ").Append(appLog.ID).AppendLine()
                        .Append("Instance ID: ").Append(appLog.ApplicationInstanceID).AppendLine()
                        .Append("ParentCallerMember: ").Append(appLog.ParentCallerMember).AppendLine()
                        .Append("CallerMember: ").Append(appLog.CallerMember).AppendLine()
                        .Append("Type: ").Append(appLog.Type).AppendLine()
                        .Append("Message: ").Append(appLog.Message).AppendLine()
                        .Append("LogLevel: ").Append(appLog.LogLevel).AppendLine()
                        .Append("Tag1: ").Append(appLog.Tag1).AppendLine()
                        .Append("Tag2: ").Append(appLog.Tag2).AppendLine()
                        .Append("CreatedAt: ").Append(appLog.CreatedAt).AppendLine()
                        .Append("</APPLICATION_LOG>").AppendLine();

                    return sb.ToString();
                }
            });
        }
    }
}

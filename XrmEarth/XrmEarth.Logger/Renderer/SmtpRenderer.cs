using System.Collections.Generic;
using System.Text;
using XrmEarth.Logger.Entity;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Renderer.Base;
using XrmEarth.Logger.Renderer.Smtp;

namespace XrmEarth.Logger.Renderer
{
    public class SmtpRenderer : MultiBaseRenderer<ISmtpRenderer>
    {
        public SmtpRenderer()
        {
            Register<TestMailClazz>(new SmtpRendererBase
                                    {
                                        GetSubject = o => ((TestMailClazz) o).Subject, 
                                        GetBody = o => ((TestMailClazz) o).Body
                                    });

            Register<Application>(new SmtpRendererBase
            {
                GetSubject = o =>
                {
                    var app = (Application)o;
                    return string.Format("'{0}' - [{1}]", app.Name, app.ID);
                },
                GetBody = o =>
                {
                    var bodyBuilder = new StringBuilder();
                    var kvRenderer = new DefaultKeyValueRenderer<Application>();
                    var propDic = kvRenderer.RenderObject(o);
                    bodyBuilder.Append("<body>");
                    foreach (var o1 in propDic)
                    {
                        bodyBuilder.Append("<p>")
                            .Append(o1.Key)
                            .Append(" : <strong>")
                            .Append(o1.Value ?? "{NULL}")
                            .Append("</strong></p>")
                            .AppendLine();
                    }
                    bodyBuilder.Append("</body>"); ;
                    return bodyBuilder.ToString();
                }
            });
            Register<ApplicationLog>(new SmtpRendererBase
            {
                GetSubject = o =>
                {
                    var al = (ApplicationLog)o;
                    return string.Format("Notify {0} - [{1}]", al.Type, al.ID);
                },
                GetBody = o =>
                {
                    var bodyBuilder = new StringBuilder();
                    var kvRenderer = new DefaultKeyValueRenderer<ApplicationLog>();
                    var propDic = kvRenderer.RenderObject(o);
                    bodyBuilder.Append("<body>");
                    foreach (var o1 in propDic)
                    {
                        bodyBuilder.Append("<p>")
                            .Append(o1.Key)
                            .Append(" : <strong>")
                            .Append(o1.Value ?? "{NULL}")
                            .Append("</strong></p>")
                            .AppendLine();
                    }
                    bodyBuilder.Append("</body>"); ;
                    return bodyBuilder.ToString();
                }
            });
            Register<ApplicationInstance>(new MultiKeyRenderer<ProcessType, ISmtpRenderer>(
            new Dictionary<ProcessType, ISmtpRenderer>
            {
                {ProcessType.Insert, new SmtpRendererBase
                {
                    GetSubject = o =>
                    {
                        var appIns = (ApplicationInstance)o;
                        return string.Format("Application Started - [{0}] - [App ID : {1}]", appIns.ID, appIns.ApplicationID);
                    },
                    GetBody = o =>
                    {
                        var bodyBuilder = new StringBuilder();
                        var kvRenderer = new DefaultKeyValueRenderer<ApplicationInstance>();
                        var propDic = kvRenderer.RenderObject(o);
                        bodyBuilder.Append("<body>");
                        foreach (var o1 in propDic)
                        {
                            bodyBuilder.Append("<p>")
                                .Append(o1.Key)
                                .Append(" : <strong>")
                                .Append(o1.Value ?? "{NULL}")
                                .Append("</strong></p>")
                                .AppendLine();
                        }
                        bodyBuilder.Append("</body>"); ;
                        return bodyBuilder.ToString();
                    }
                }},
                {ProcessType.Update, new SmtpRendererBase
                {
                    GetSubject = o =>
                    {
                        var appIns = (ApplicationInstance)o;
                        return string.Format("Application Closed - [{0}] - [App ID : {1}]", appIns.ID, appIns.ApplicationID);
                    },
                    GetBody = o =>
                    {
                        var bodyBuilder = new StringBuilder();
                        var kvRenderer = new DefaultKeyValueRenderer<ApplicationInstance>();
                        var propDic = kvRenderer.RenderObject(o);
                        bodyBuilder.Append("<body>");
                        foreach (var o1 in propDic)
                        {
                            bodyBuilder.Append("<p>")
                                .Append(o1.Key)
                                .Append(" : <strong>")
                                .Append(o1.Value ?? "{NULL}")
                                .Append("</strong></p>")
                                .AppendLine();
                        }
                        bodyBuilder.Append("</body>"); ;
                        return bodyBuilder.ToString();
                    }
                }},
            }));
        }
    }

    public class TestMailClazz
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}

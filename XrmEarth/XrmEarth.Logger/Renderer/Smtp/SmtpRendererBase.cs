using System;
using System.Collections.Generic;
using XrmEarth.Logger.Renderer.Base;

namespace XrmEarth.Logger.Renderer.Smtp
{
    public class SmtpRendererBase : BaseRenderer, ISmtpRenderer
    {
        public const string BodyKey = "SmtpRendererBase_Body";
        public const string SubjectKey = "SmtpRendererBase_Subject";

        protected override Dictionary<string, object> OnRenderObject(object value)
        {
            return new Dictionary<string, object>
                   {
                       {SubjectKey, GetSubject(value)},
                       {BodyKey, GetBody(value)},
                   };
        }

        public override Action<object, Dictionary<string, object>> ValidateAction { get; set; }

        public Func<object, string> GetSubject { get; set; }
        public Func<object, string> GetBody { get; set; }
    }
}

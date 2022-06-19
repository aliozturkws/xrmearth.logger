using System;
using System.Collections.Generic;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Renderer;
using XrmEarth.Logger.Renderer.Content;

namespace XrmEarth.Logger.Logger
{
    public class ConsoleLogger : BaseLogger<ConsoleConnection, ContentRenderer>
    {
        public ConsoleLogger(bool followApplication) : this(new ContentRenderer(), followApplication)
        {
            
        }
        public ConsoleLogger(ContentRenderer renderer, bool followApplication) : base(new ConsoleConnection(), renderer, followApplication)
        {
        }

        protected override void OnPush(Dictionary<string, object> keyValuesDictionary)
        {
            Console.WriteLine();
            Console.WriteLine();

            var content = keyValuesDictionary[ContentRendererBase.ContentKey];
            if(content != null)
                Console.WriteLine(content.ToString());
        }
    }
}

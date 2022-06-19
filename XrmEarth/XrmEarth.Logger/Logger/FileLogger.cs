using System;
using System.Collections.Generic;
using System.IO;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Configuration;
using XrmEarth.Logger.Connection;
using XrmEarth.Logger.Renderer;
using XrmEarth.Logger.Renderer.Content;

namespace XrmEarth.Logger.Logger
{
    public class FileLogger : BaseLogger<FileConnection, ContentRenderer>
    {
        public FileLogger(FileConnection connection) : base(connection, new ContentRenderer(), true)
        {
            AutoFlush = true;
            AutoClose = false;
        }
        public FileLogger(FileConnection connection, ContentRenderer renderer) : base(connection, renderer, true)
        {
            AutoFlush = true;
            AutoClose = false;
        }
        public FileLogger(FileConnection connection, ContentRenderer renderer, bool followApplication) : base(connection, renderer, followApplication)
        {
            AutoFlush = true;
            AutoClose = false;
        }
        public FileLogger(FileConnection connection, bool followApplication) : base(connection, new ContentRenderer(), followApplication)
        {
            AutoFlush = true;
            AutoClose = false;
        }
        public FileLogger(string directory, int fileMBLimit = 0, bool followApplication = true) : base(new FileConnection
        {
            Directory = directory,
            NameGenerator = new FileNameGenerator(),
            FileMBLimit = fileMBLimit,
        }, new ContentRenderer(), followApplication)
        {
            AutoFlush = true;
            AutoClose = false;
        }
        public FileLogger(string directory, string filePrefix = "Log", string fileExtension = ".newlog", int fileMBLimit = 0, bool followApplication = true) : base(new FileConnection
        {
            Directory = directory,
            NameGenerator = new FileNameGenerator(filePrefix, fileExtension),
            FileMBLimit = fileMBLimit,
        }, new ContentRenderer(), followApplication)
        {
            AutoFlush = true;
            AutoClose = false;
        }
        public FileLogger(string directory, string fileName, bool followApplication = true) : base(new FileConnection
        {
            Directory = directory,
            GetFileNameFunc = s => fileName,
            FileMBLimit = 0,
        }, new ContentRenderer(), followApplication)
        {
            AutoFlush = true;
            AutoClose = false;
        }

        public bool AutoFlush { get; set; }
        public bool AutoClose { get; set; }

        protected override void OnPush(Dictionary<string, object> keyValuesDictionary)
        {
            var stream = Connection.Stream;
            WriteText(stream, Environment.NewLine);
            WriteText(stream, Environment.NewLine);

            var content = keyValuesDictionary[ContentRendererBase.ContentKey];
            if (content != null)
                WriteText(stream, content.ToString());

            if(AutoClose)
                Connection.CloseStream();
        }

        protected void WriteText(FileStream stream, string text)
        {
            var bytes = InitConfiguration.Encoding.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
            if(AutoFlush)
                stream.Flush();
        }
    }
}

using System;
using System.IO;
using XrmEarth.Logger.Common;
using XrmEarth.Logger.Enums;
using XrmEarth.Logger.Target;

namespace XrmEarth.Logger.Connection
{
    /// <summary>
    /// A feature such as always use a new file for the first time can be added.
    /// </summary>
    [DefaultTarget(typeof(FileTarget))]
    public class FileConnection : IConnection
    {
        public FileConnection(string filePrefix = "Log", string fileExtension = ".newlog", string fileSuffix = "-")
        {
            NameGenerator = new FileNameGenerator(filePrefix, fileExtension, fileSuffix);
        }

        public string Directory { get; set; }

        public int FileMBLimit { get; set; }

        private string _fileName;
        public string WorkingOnFileName { get { return _fileName;} }
        private Func<string, string> _getFileNameFunc;
        public Func<string, string> GetFileNameFunc
        {
            get
            {
                if (_getFileNameFunc == null)
                {
                    if (NameGenerator != null)
                        _getFileNameFunc = NameGenerator.GetFileName;
                }
                return _getFileNameFunc;
            }
            set { _getFileNameFunc = value; }
        }

        internal FileNameGenerator NameGenerator{get; set; }

        private FileStream _stream;
        public FileStream Stream
        {
            get { return GetFileStream(_fileName); }
        }

        public void CloseStream()
        {
            if (_stream != null)
            {
                _stream.Flush();
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }
        }

        private FileStream GetFileStream(string fileName = null)
        {
            var infinity = FileMBLimit <= 0;
            var limitBytes = FileMBLimit * 1024 * 1024;

            if (_stream == null)
            {
                if (!System.IO.Directory.Exists(Directory))
                    System.IO.Directory.CreateDirectory(Directory);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    _fileName = fileName = GetFileNameFunc(null);
                }
                else
                {
                    _fileName = fileName;
                }
                var path = Path.Combine(Directory, fileName);
                
                var stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                _stream = stream;
            }

            if (!infinity && _stream.Length >= limitBytes)
            {
                var newFileName = _fileName = GetFileNameFunc(_fileName);
                var newPath = Path.Combine(Directory, newFileName);
                CloseStream();
                LogManager.Instance.OnCallSystemNotify(string.Format("Creating new file '{2}' because file '{0}' has reached the specified limit ({1} MB). Index: {3}", fileName, FileMBLimit, newFileName, newPath), 0, LogType.Info, false);
                return GetFileStream(newFileName);
            }

            return _stream;
        }

        public static FileConnection Create(string directory, string filePrefix = "Log", string fileExtension = ".newlog", int fileMBLimit = 10)
        {
            return new FileConnection
            {
                Directory = directory,
                FileMBLimit = fileMBLimit,
                NameGenerator = new FileNameGenerator(filePrefix, fileExtension)
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using XrmEarth.Logger.Data;
using XrmEarth.Logger.Utility;

namespace XrmEarth.Logger
{
    public class ApplicationSummary
    {
        #region - Reserved Text -

        public const string Extension = "casum";

        public const string AppName = "%appname%";
        public const string AppVersion = "%appversion%";
        public const string AppGuid = "%appguid%";
        public const string AppTitle = "%apptitle%";
        public const string AppDescription = "%appdescription%";
        public const string AppExecutablePath = "%appexecpath%";
        public const string AppPath = "%apppath%";
        public const string AppDataPath = "%appdatapath%";
        public const string AppLogPath = "%applogpath%";
        public const string AppCorpDataPath = "%corpdatapath%";
        public const string AppSolutionName = "%solutionname%";
        #endregion - Reserved Text -

        /// <summary>
        /// Loaded from entry assembly.
        /// </summary>
        public ApplicationSummary()
        {
            LoadFromAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appPath">Application executable path.</param>
        public ApplicationSummary(string appPath)
        {
            LoadFromFile(appPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">Application assembly. </param>
        public ApplicationSummary(Assembly assembly)
        {
            LoadFromAssembly(assembly);
        }

        #region - Fields -
        private Dictionary<string, Type> _reservedTypes;
        protected Dictionary<string, Type> ReservedTypes
        {
            get
            {
                if (_reservedTypes == null)
                {
                    _reservedTypes = new Dictionary<string, Type>
                                         {
                                             {AppName, typeof(string)},
                                             {AppVersion, typeof(Version)},
                                             {AppGuid, typeof(Guid)},
                                             {AppTitle, typeof(string)},
                                             {AppDescription, typeof(string)},
                                             {AppExecutablePath, typeof(string)},
                                             {AppPath, typeof(string)},
                                             {AppDataPath, typeof(string)},
                                         };
                }

                return _reservedTypes;
            }
        }
        #endregion - Fields -

        #region - Properties -
        public Assembly Assembly { get; private set; }

        public string Name { get; private set; }

        public Version Version { get; private set; }
        public Guid Guid { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public string Path { get; private set; }
        public string ExecutablePath { get; private set; }
        public string DataPath { get; private set; }
        public string LogPath { get; set; }
        public string CorpDataPath { get; private set; }
        public string SolutionName { get; set; }
        #endregion

        #region - Methods -
        public object GetValue(string reservedText)
        {
            if (string.IsNullOrEmpty(reservedText))
                return string.Empty;

            reservedText = reservedText.ToLower().Trim();

            switch (reservedText)
            {
                case AppName:
                    return Name;
                case AppVersion:
                    return Version.ToString(4);
                case AppGuid:
                    return Guid;
                case AppTitle:
                    return Title;
                case AppDescription:
                    return Description;
                case AppExecutablePath:
                    return ExecutablePath;
                case AppPath:
                    return Path;
                case AppDataPath:
                    return DataPath;
                case AppCorpDataPath:
                    return CorpDataPath;
                case AppSolutionName:
                    return SolutionName;
                case AppLogPath:
                    return LogPath;
            }

            return string.Empty;
        }

        public string ScanConvertValue(string text)
        {
            foreach (var resT in ReservedTypes)
            {
                if (ReflectionUtil.IsWritable(resT.Value) || resT.Value == typeof(Version))
                {
                    var val = GetValue(resT.Key);
                    if (val != null)
                    {
                        text = text.Replace(resT.Key, val.ToString());
                    }
                }
                else
                {
                    if (text.IndexOf(resT.Key, StringComparison.InvariantCultureIgnoreCase) == -1)
                        continue;

                    var val = GetValue(resT.Key);
                    var jsonText = JsonSerializerUtil.Serialize(val);

                    text = text.Replace(resT.Key, jsonText);
                }
            }

            return text;
        }

        public void Save(string path, string fileName)
        {
            var fullPath = System.IO.Path.Combine(ScanConvertValue(path), string.Concat(ScanConvertValue(fileName), ".", Extension));

            JsonSerializerUtil.SerializeFile(this, fullPath);
        }

        public void Save()
        {
            JsonSerializerUtil.SerializeFile(this, GetExtensionFullPath());
        }
        #endregion - Methods -

        #region - Statics -
        public static bool LoadApplicationSummary(string filePath, out ApplicationSummary applicationSummary)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Dosya bulunamadı. Dizin : ", filePath);

            var dic = System.IO.Path.GetDirectoryName(filePath);
            var fileName = System.IO.Path.GetFileName(filePath);

            if (fileName != null && fileName.EndsWith(".exe", true, CultureInfo.InvariantCulture))
            {
                fileName = fileName.Remove(fileName.Length - 4);
            }

            applicationSummary = null;

            var extensionPath = GetExtensionFullPath(dic, fileName);
            if (File.Exists(extensionPath))
            {
                applicationSummary = Load(extensionPath);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">Application summary extension file path.</param>
        /// <returns></returns>
        public static ApplicationSummary Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Dosya bulunamadı. Dizin : ", filePath);

            return JsonSerializerUtil.Deserialize<ApplicationSummary>(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory">Parent directory path.</param>
        /// <param name="subDirectory">TopDirectoryOnly(false) or AllDirectories(true).</param>
        /// <returns></returns>
        public static List<ApplicationSummary> ScanLoad(string directory, bool subDirectory = false)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException("Dizin bulunamadı. Dizin : " + directory);

            var dicInfo = new DirectoryInfo(directory);

            var summaryFiles = dicInfo.GetFiles(string.Format("*.{0}", Extension), subDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var summaries = new List<ApplicationSummary>();
            foreach (var summaryFile in summaryFiles)
            {
                try
                {
                    summaries.Add(Load(summaryFile.FullName));
                }
                catch
                {

                }
            }

            return summaries;
        }

        public static string GetExtensionFullPath(string path, string name)
        {
            return System.IO.Path.Combine(path, string.Concat(name, ".", Extension));
        }
        #endregion - Statics -

        #region - Workers -
        private void LoadFromFile(string filePath)
        {
            var asm = Assembly.LoadFile(filePath);
            LoadFromAssembly(asm);
        }

        private void LoadFromAssembly(Assembly assembly)
        {
            Assembly = assembly;

            Name = assembly.GetName().Name;

            Version = assembly.GetName().Version;

            var guidAttr = assembly.GetCustomAttributes(typeof(GuidAttribute), false).FirstOrDefault() as GuidAttribute;
            Guid = guidAttr == null ? Guid.Empty : Guid.Parse(guidAttr.Value);

            var descriptionAttr = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault() as AssemblyDescriptionAttribute;
            Description = descriptionAttr == null ? null : descriptionAttr.Description;

            var titleAttr = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault() as AssemblyTitleAttribute;
            Title = titleAttr == null ? null : titleAttr.Title;

            ExecutablePath = new Uri(assembly.EscapedCodeBase).LocalPath;

            Path = System.IO.Path.GetDirectoryName(ExecutablePath);

            CorpDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.CorporationName);

            SolutionName = System.IO.Path.GetFileName(assembly.Location);

            DataPath = System.IO.Path.Combine(CorpDataPath, Name);
            LogPath = System.IO.Path.Combine(CorpDataPath, Name, "Logs");
        }

        private string GetExtensionFullPath()
        {
            return GetExtensionFullPath(Path, Name);
        }
        #endregion - Workers -
    }
}

using System;
using System.Linq;

namespace XrmEarth.Logger.Common
{
    public class FileNameGenerator
    {
        public FileNameGenerator(string filePrefix = "Log", string fileExtension = ".newlog", string fileSuffix = "-")
        {
            FilePrefix = filePrefix;
            FileSuffix = fileSuffix;
            FileExtension = fileExtension;
        }

        private string FilePrefix { get; set; }
        private string FileExtension { get; set; }
        private string FileSuffix { get; set; }

        public string GetFileName(string existFileName)
        {
            if (string.IsNullOrWhiteSpace(existFileName))
                return FilePrefix + FileExtension;

            var prepareName = existFileName;
            if (prepareName.EndsWith(FileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                prepareName = prepareName.Remove(prepareName.Length - FileExtension.Length, FileExtension.Length);
            }
            if (prepareName.Contains(FileSuffix))
            {
                var numeric = prepareName.Split(new []{FileSuffix}, StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                if (int.TryParse(numeric, out int index))
                {
                    index++;
                    return FilePrefix + FileSuffix + index + FileExtension;
                }
                else
                {
                    throw new Exception(string.Format("Dosya ismi geçersiz formattaydı. Dosya adı: {0}", existFileName));
                }
            }
            else
            {
                return FilePrefix + FileSuffix + "1" + FileExtension;
            }

            return existFileName;
        }
    }
}

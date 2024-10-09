using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarPDFSolutionLibrary.Utilities
{
    internal class FileUtility
    {
        public static bool AllFilesExist(IEnumerable<string> filePaths)
        {
            return filePaths.All(f => File.Exists(f) == true);
        }

        public static IEnumerable<string> GetExistingFiles(IEnumerable<string> filePaths)
        {
            var output = new List<string>();
            foreach (var file in filePaths)
                if (File.Exists(file))
                    output.Add(file);

            return output;
        }

        public static string IncrementFilePath(string filePath)
        {
            var parentDirectory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var fileExtension = Path.GetExtension(filePath);
            var fileCount = 1;
            while (File.Exists(filePath))
            {
                filePath = Path.Combine(parentDirectory, $"{fileName} ({fileCount}){fileExtension}");
                fileCount++;
            }

            return filePath;
        }
    }
}

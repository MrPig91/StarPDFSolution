using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarPDFSolutionLibrary.Models
{
    public class StarPDFDocument
    {
        public string FileName => Path.GetFileName(FilePath);
        public string FilePath { get; set; }
        public int PageCount { get; set; }

        public StarPDFDocument(string filePath)
        {
            FilePath = filePath;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarPDFSolutionLibrary.Models;

namespace StarPDFSolutionLibrary.Services.Editors
{
    public interface IPDFEditorService
    {
        StarPDFDocument CombineFiles(IEnumerable<string> filePaths, CombinePDFOptions options = CombinePDFOptions.None, string? destinationFilePath = null, IProgress<double>? progress = null);
        Task<StarPDFDocument> CombineFilesAsync(IEnumerable<string> filePaths, CombinePDFOptions options = CombinePDFOptions.None, string? destinationFilePath = null, IProgress<double>? progress = null);
        IEnumerable<StarPDFDocument> Split(string sourceFilePath, SplitPDFOptions options = SplitPDFOptions.None, IProgress<Tuple<StarPDFDocument, double>>? progress = null);
        Task<IEnumerable<StarPDFDocument>> SplitAsync(string sourceFilePath, SplitPDFOptions options = SplitPDFOptions.None, IProgress<Tuple<StarPDFDocument, double>>? progress = null);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using StarPDFSolutionLibrary.Models;
using StarPDFSolutionLibrary.Utilities;

namespace StarPDFSolutionLibrary.Services.Editors
{
    public class PDFSharpEditorService : IPDFEditorService
    {
        public StarPDFDocument CombineFiles(IEnumerable<string> filePaths, CombinePDFOptions options = CombinePDFOptions.None, string? destinationFilePath = null, IProgress<double>? progress = null)
        {
            var output = new PdfDocument();
            var existingFiles = FileUtility.GetExistingFiles(filePaths);
            double docCount = existingFiles.Count();
            // HasFlag is inefficient, so call it once
            var removeComments = options.HasFlag(CombinePDFOptions.RemoveComments);
            var removeBookmarks = options.HasFlag(CombinePDFOptions.RemoveBookmarks);

            if (destinationFilePath == null && existingFiles.Count() > 0)
                destinationFilePath = FileUtility.IncrementFilePath(Path.Combine(Path.GetDirectoryName(existingFiles.First()), "Combined PDF.pdf"));

            double addedDocCount = 0;
            foreach (var existingFile in existingFiles)
            {
                using (var pdf = PdfReader.Open(existingFile, PdfDocumentOpenMode.Import))
                {
                    var pageLookup = new Dictionary<PdfPage, PdfPage>();
                    foreach (var page in pdf.Pages)
                    {
                        if (removeComments)
                            page.Annotations.Clear();
                        var newPage = output.AddPage(page);
                        pageLookup.Add(page, newPage);
                    }
                    // sometimes outline is null, eat the exception
                    try
                    {
                        if (removeBookmarks == false)
                            CopyBookmarks(pdf, output, pageLookup);
                    }
                    catch { }
                    addedDocCount++;
                    if (progress is not null)
                    {
                        var percentComplete = addedDocCount / docCount;
                        progress.Report(percentComplete);
                    }
                }
            }

            output.Save(destinationFilePath);
            if (progress is not null)
                progress.Report(1);
            return new StarPDFDocument(destinationFilePath)
            {
                PageCount = existingFiles.Count()
            };
        }

        public async Task<StarPDFDocument> CombineFilesAsync(IEnumerable<string> filePaths, CombinePDFOptions options = CombinePDFOptions.None, string? destinationFilePath = null, IProgress<double>? progress = null)
        {
            return await Task<StarPDFDocument>.Run(() => CombineFiles(filePaths, options, destinationFilePath, progress));
        }

        public IEnumerable<StarPDFDocument> Split(string sourceFilePath, SplitPDFOptions options = SplitPDFOptions.None, IProgress<Tuple<StarPDFDocument, double>>? progress = null)
        {
            var outputFiles = new List<StarPDFDocument>();
            var pageNumber = 1;
            var fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            // HasFlag is inefficient, so call it once
            var removeComments = options.HasFlag(SplitPDFOptions.RemoveComments);
            var removeBookmarks = options.HasFlag(SplitPDFOptions.RemoveBookmarks);
            using (var pdf = PdfReader.Open(sourceFilePath, PdfDocumentOpenMode.Import))
            {
                double pageCount = pdf.Pages.Count;
                Dictionary<PdfPage, PdfPage> pageLookup;
                foreach (var page in pdf.Pages)
                {
                    using (var outputDocument = new PdfDocument())
                    {
                        var savePath = Path.Combine(Path.GetDirectoryName(sourceFilePath), $"{fileName} - Page {pageNumber}.pdf");
                        savePath = FileUtility.IncrementFilePath(savePath);
                        if (removeComments)
                            page.Annotations.Clear();
                        var newPage = outputDocument.AddPage(page);

                        // sometimes outline is null, eat the exception
                        try
                        {
                            if (removeBookmarks == false)
                                foreach (var outline in pdf.Outlines.Where(o => o.DestinationPage == page))
                                {
                                    outputDocument.Outlines.Add(outline.Title, newPage);
                                }
                        }
                        catch { }
                        outputDocument.Save(savePath);
                        StarPDFDocument newDoc = new(savePath) { PageCount = 1 };
                        outputFiles.Add(newDoc);
                        pageNumber++;
                        if (progress is not null)
                            progress.Report(Tuple.Create(newDoc, ((pageNumber - 1) / pageCount)));
                    }
                }
            }

            return outputFiles;
        }

        public async Task<IEnumerable<StarPDFDocument>> SplitAsync(string sourceFilePath, SplitPDFOptions options = SplitPDFOptions.None, IProgress<Tuple<StarPDFDocument, double>>? progress = null)
        {
            return await Task<IEnumerable<StarPDFDocument>>.Run(() => Split(sourceFilePath, options, progress));
        }

        private static void CopyBookmarks(PdfDocument source, PdfDocument target, Dictionary<PdfPage, PdfPage> pageLookup)
        {
            if (source.Outlines.Count > 0)
            {
                foreach (PdfOutline outline in source.Outlines)
                {
                    PdfOutline newOutline = target.Outlines.Add(outline.Title, pageLookup[outline.DestinationPage]);
                    CopySubBookmarks(outline, newOutline, pageLookup);
                }
            }
        }

        private static void CopySubBookmarks(PdfOutline sourceOutline, PdfOutline targetOutline, Dictionary<PdfPage, PdfPage> pageLookup)
        {
            if (sourceOutline.Outlines.Count > 0)
            {
                foreach (PdfOutline subOutline in sourceOutline.Outlines)
                {
                    PdfOutline newSubOutline = targetOutline.Outlines.Add(subOutline.Title, pageLookup[subOutline.DestinationPage]);
                    CopySubBookmarks(subOutline, newSubOutline, pageLookup);
                }
            }
        }
    }
}

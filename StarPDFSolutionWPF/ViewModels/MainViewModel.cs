using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarPDFSolutionLibrary.Services.Editors;

namespace StarPDFSolutionWPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public CombineFilesViewModel CombineFilesVM { get; }
        public SplitFileViewModel SplitFileVM { get; }

        public MainViewModel(IPDFEditorService pdfEditorService, CombineFilesViewModel combineFilesViewModel, SplitFileViewModel splitFileViewModel)
        {
            CombineFilesVM = combineFilesViewModel;
            SplitFileVM = splitFileViewModel;
        }
    }
}

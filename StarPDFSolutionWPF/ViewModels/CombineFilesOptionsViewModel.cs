using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarPDFSolutionWPF.ViewModels
{
    public class CombineFilesOptionsViewModel : BaseViewModel
    {
        public CombinePDFOptions GetPDFOptions()
        {
            var combineOptions = CombinePDFOptions.None;
            if (_removeComments)
                combineOptions = combineOptions.AddFlag(CombinePDFOptions.RemoveComments);
            if (_removeBookmarks)
                combineOptions = combineOptions.AddFlag(CombinePDFOptions.RemoveBookmarks);
            return combineOptions;
        }

        private bool _openFile;
		public bool OpenFile
		{
			get { return _openFile; }
			set { _openFile = value; OnPropertyChanged(); }
		}

		private bool _deleteSourceFiles;

		public bool DeleteSourceFiles
		{
			get { return _deleteSourceFiles; }
			set { _deleteSourceFiles = value; OnPropertyChanged(); }
		}

        private bool _removeComments;
        public bool RemoveComments
        {
            get => _removeComments;
            set { _removeComments = value; OnPropertyChanged(); }
        }
        private bool _removeBookmarks;
        public bool RemoveBookmarks
        {
            get { return _removeBookmarks; }
            set { _removeBookmarks = value; }
        }

        public CombineFilesOptionsViewModel()
		{
			OpenFile = true;
		}
	}
}

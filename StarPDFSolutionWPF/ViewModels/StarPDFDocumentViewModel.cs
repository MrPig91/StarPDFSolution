﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StarPDFSolutionLibrary.Models;

namespace StarPDFSolutionWPF.ViewModels
{
    public class StarPDFDocumentViewModel : BaseViewModel
    {
        public StarPDFDocument? SourceModel { get; private set; }
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            private set { _fileName = value; OnPropertyChanged(); }
        }

        public string NaturalSortFileName => Regex.Replace(FileName, @"\d+", m => m.Value.PadLeft(255, '0'));

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged(); FileName = Path.GetFileName(value); }
        }
        private int? _pageCount;
        public int? PageCount
        {
            get => _pageCount;
            set { _pageCount = value; OnPropertyChanged(); }
        }

        public StarPDFDocumentViewModel(StarPDFDocument sourceModel)
        {
            SourceModel = sourceModel;
            FilePath = sourceModel.FilePath;
            PageCount = sourceModel.PageCount;
        }
        public StarPDFDocumentViewModel(string filePath)
        {
            FilePath = filePath;
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}

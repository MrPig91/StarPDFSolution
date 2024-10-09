using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using StarPDFSolutionLibrary.Models;
using StarPDFSolutionLibrary.Services.Editors;
using StarPDFSolutionWPF.Commands;
using StarPDFSolutionWPF.Extensions;
using StarPDFSolutionWPF.Utilities;

namespace StarPDFSolutionWPF.ViewModels
{
    public class CombineFilesViewModel : BaseViewModel
    {
        private IPDFEditorService _pdfEditorService;
        private Progress<double> _progressUpdater = new();
        private SortDescription _naturalSortDescription = new SortDescription(nameof(StarPDFDocumentViewModel.NaturalSortFileName), ListSortDirection.Ascending);
        public bool IsNaturalSortEnabled => SourceFilesView.SortDescriptions.Contains(_naturalSortDescription);
        public ObservableCollection<StarPDFDocumentViewModel> SourceFiles { get; } = new();
        public ListCollectionView SourceFilesView { get; }
        private StarPDFDocumentViewModel _selectedSourceFile;
        public StarPDFDocumentViewModel SelectedSourceFile
        {
            get { return _selectedSourceFile; }
            set { _selectedSourceFile = value; OnPropertyChanged(); }
        }

        private StarPDFDocumentViewModel? _outputFile;
        public StarPDFDocumentViewModel? OutputFile
        {
            get => _outputFile;
            set { _outputFile = value; OnPropertyChanged(); }
        }

        private double? _progress;

        public double? Progress
        {
            get { return _progress; }
            set { _progress = value; OnPropertyChanged(); }
        }


        public CombineFilesOptionsViewModel Options { get; } = new();

        private void SelectFiles()
        {
            SourceFiles.Clear();
            var files = FileSelectorUtility.SelectMultipleFiles(FileSelectorUtility.FilterPDF);
            foreach (var file in files)
                SourceFiles.Add(new(file));
        }
        private ICommand? _selectFilesCommand;
        public ICommand SelectFilesCommand
        {
            get
            {
                if (_selectFilesCommand == null)
                    _selectFilesCommand = new RelayCommand(param => SelectFiles());
                return _selectFilesCommand;
            }
        }
        private async void CombineFiles()
        {
            try
            {
                if (SourceFiles.Count == 0)
                {
                    SelectFiles();
                    if (SourceFiles.Count == 0)
                        return;
                }
                var filePaths = new List<string>();
                foreach (StarPDFDocumentViewModel file in SourceFilesView)
                    filePaths.Add(file.FilePath);

                OutputFile = new(await _pdfEditorService.CombineFilesAsync(filePaths, options: Options.GetPDFOptions(), progress: _progressUpdater));
                if (Options.OpenFile)
                    Process.Start(new ProcessStartInfo(OutputFile.FilePath) { UseShellExecute = true });
                if (Options.DeleteSourceFiles)
                {
                    foreach (var file in SourceFiles)
                        File.Delete(file.FilePath);

                    SourceFiles.Clear();
                }
                Progress = null;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private ICommand _combineFilesCommand;
        public ICommand CombineFilesCommand
        {
            get
            {
                if (_combineFilesCommand == null)
                    _combineFilesCommand = new RelayCommand(param => CombineFiles());
                return _combineFilesCommand;
            }
        }
        public void MoveUp()
        {
            try
            {
                DisableNaturalSort();
                var oldIndex = SourceFiles.IndexOf(SelectedSourceFile);
                var newIndex = oldIndex - 1;
                if (oldIndex == 0)
                    return;
                SourceFiles.Move(oldIndex, newIndex);
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }
        private ICommand _moveUpCommand;

        public ICommand MoveUpCommand
        {
            get
            {
                if (_moveUpCommand == null)
                    _moveUpCommand = new RelayCommand(param => MoveUp(), param => SelectedSourceFile is not null);
                return _moveUpCommand;
            }
        }

        public void MoveDown()
        {
            try
            {
                DisableNaturalSort();
                var oldIndex = SourceFiles.IndexOf(SelectedSourceFile);
                var newIndex = oldIndex + 1;
                if (oldIndex == SourceFiles.Count - 1)
                    return;
                SourceFiles.Move(oldIndex, newIndex);
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }
        private ICommand _moveDownCommand;

        public ICommand MoveDownCommand
        {
            get
            {
                if (_moveDownCommand == null)
                    _moveDownCommand = new RelayCommand(param => MoveDown(), param => SelectedSourceFile is not null);
                return _moveDownCommand;
            }
        }
        private void DisableNaturalSort()
        {
            try
            {
                if (IsNaturalSortEnabled)
                    SourceFilesView.SortDescriptions.Remove(_naturalSortDescription);
                OnPropertyChanged(nameof(IsNaturalSortEnabled));
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }
        private void NaturalSort()
        {
            try
            {
                if (IsNaturalSortEnabled)
                    SourceFilesView.SortDescriptions.Remove(_naturalSortDescription);
                else
                    SourceFilesView.SortDescriptions.Add(_naturalSortDescription);

                OnPropertyChanged(nameof(IsNaturalSortEnabled));
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }
        private ICommand _naturalSortCommand;
        public ICommand NaturalSortCommand
        {
            get
            {
                if (_naturalSortCommand is null)
                    _naturalSortCommand = new RelayCommand(param => NaturalSort());
                return _naturalSortCommand;
            }
        }
        public void Remove()
        {
            try
            { 
                if (SourceFiles.Contains(SelectedSourceFile))
                    SourceFiles.Remove(SelectedSourceFile);
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }
        private ICommand _removeCommand;
        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand is null)
                    _removeCommand = new RelayCommand(param => Remove(), param => SelectedSourceFile is not null);
                return _removeCommand;
            }
        }
        public void Clear()
        {
            try
            {
                SourceFiles.Clear();
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }
        private ICommand _clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (_clearCommand is null)
                    _clearCommand = new RelayCommand(param => Clear());
                return _clearCommand;
            }
        }
        public CombineFilesViewModel(IPDFEditorService pdfEditorService) 
        {
            _pdfEditorService = pdfEditorService;
            _progressUpdater.ProgressChanged += ProgressUpdater_ProgressChanged1;
            SourceFilesView = new(SourceFiles);
            SourceFilesView.SortDescriptions.Add(_naturalSortDescription);
        }

        private void ProgressUpdater_ProgressChanged1(object? sender, double e)
        {
            Progress = e;
        }
    }
}

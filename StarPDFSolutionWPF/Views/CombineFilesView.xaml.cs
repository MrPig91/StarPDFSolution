using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StarPDFSolutionWPF.Extensions;
using StarPDFSolutionWPF.ViewModels;

namespace StarPDFSolutionWPF.Views
{
    /// <summary>
    /// Interaction logic for CombineFilesView.xaml
    /// </summary>
    public partial class CombineFilesView : UserControl
    {
        public CombineFilesView()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch (Exception ex) { ex.Message.ShowAsError(); }
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            dragDropGrid.Visibility = Visibility.Visible;
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            dragDropGrid.Visibility = Visibility.Collapsed;
        }


        private void addFilesBorder_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var viewModel = this.DataContext as CombineFilesViewModel;
                    if (viewModel is null)
                        return;
                    string fileName = string.Empty;
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    List<string> pdfsToAdd = new();
                    files.Where(f => f.EndsWith(".pdf")).ToList().ForEach(f => pdfsToAdd.Add(f));

                    if (pdfsToAdd.Count > 0)
                    {
                        foreach (var file in pdfsToAdd)
                        {
                            viewModel.SourceFiles.Add(new(file));
                        }
                        viewModel.SelectedSourceFile = viewModel.SourceFiles.FirstOrDefault();
                    }
                }

                dragDropGrid.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void combineFilesBorder_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var viewModel = this.DataContext as CombineFilesViewModel;
                    if (viewModel is null)
                        return;
                    string fileName = string.Empty;
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    List<string> pdfsToCombine = new();
                    files.Where(f => f.EndsWith(".pdf")).ToList().ForEach(f => pdfsToCombine.Add(f));
                    if (pdfsToCombine.Count > 0)
                    {
                        viewModel.SourceFiles.Clear();
                        foreach (var file in pdfsToCombine)
                        {
                            viewModel.SourceFiles.Add(new(file));
                        }
                        viewModel.SelectedSourceFile = viewModel.SourceFiles.FirstOrDefault();
                        viewModel.CombineFilesCommand.Execute(null);
                    }
                }
                dragDropGrid.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void dragDropGrid_Drop(object sender, DragEventArgs e)
        {
            dragDropGrid.Visibility= Visibility.Collapsed;
        }
    }
}

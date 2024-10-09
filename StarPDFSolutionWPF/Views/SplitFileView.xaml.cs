using System;
using System.Collections.Generic;
using System.IO;
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
using StarPDFSolutionWPF.ViewModels;

namespace StarPDFSolutionWPF.Views
{
    /// <summary>
    /// Interaction logic for SplitFileView.xaml
    /// </summary>
    public partial class SplitFileView : UserControl
    {
        public SplitFileView()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var viewModel = this.DataContext as SplitFileViewModel;
                    if (viewModel is null)
                        return;
                    string fileName = string.Empty;
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    List<string> pdfsToSplit = new();
                    files.Where(f => f.EndsWith(".pdf")).ToList().ForEach(f => pdfsToSplit.Add(f));

                    if (pdfsToSplit.Count > 0)
                        viewModel.SplitFileCommand.Execute(pdfsToSplit);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }

            dragDropBorder.Visibility = Visibility.Collapsed;
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            dragDropBorder.Visibility = Visibility.Visible;
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            dragDropBorder.Visibility = Visibility.Collapsed;
        }
    }
}

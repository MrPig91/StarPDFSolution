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

namespace StarPDFSolutionWPF.Views.StarPDFDocumentViews
{
    /// <summary>
    /// Interaction logic for StarPDFDocumentListItemViewModel.xaml
    /// </summary>
    public partial class StarPDFDocumentListItemViewModel : UserControl
    {
        public StarPDFDocumentListItemViewModel()
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
    }
}

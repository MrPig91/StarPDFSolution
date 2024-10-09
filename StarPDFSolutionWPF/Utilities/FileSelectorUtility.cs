using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace StarPDFSolutionWPF.Utilities
{
    static class FileSelectorUtility
    {
        public static List<string> SelectMultipleFiles(string filter = "All Files|*.*")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = filter;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                return new List<string>(openFileDialog.FileNames);
            }
            else
            {
                return new List<string>();
            }
        }

        public static string? SelectFile(string filter = "All Files|*.*")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = filter;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
                return openFileDialog.FileName;
            else
                return null;
        }

        public static string FilterPDF => "PDF | *.pdf";
    }
}

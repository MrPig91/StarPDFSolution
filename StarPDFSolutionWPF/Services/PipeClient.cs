using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StarPDFSolutionWPF.Services
{
    public static class PipeClient
    {
        public static void SendMessage(string msg)
        {
            var pipeClient = new NamedPipeClientStream(".", "StarPDFSolutionPipe", PipeDirection.Out);
            pipeClient.Connect(1000);

            try
            {
                // Read user input and send that to the client process.
                using var sw = new StreamWriter(pipeClient);
                sw.AutoFlush = true;
                sw.WriteLine(msg);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

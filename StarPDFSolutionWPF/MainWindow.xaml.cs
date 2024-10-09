using System.Diagnostics;
using System.Text;
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
using StarPDFSolutionWPF.Services;
using StarPDFSolutionWPF.ViewModels;

namespace StarPDFSolutionWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly PipeServer _pipeServer;
    public string LastCommand = "";
    public bool _resetUponNextMessage = false;

    public MainWindow(MainViewModel mainVM, PipeServer pipeServer)
    {
        InitializeComponent();
        DataContext = mainVM;
        _pipeServer = pipeServer;
        _pipeServer.MessageReceived += _pipeServer_MessageReceived;
        _pipeServer.LastMessagedReceived += _pipeServer_LastMessagedReceived;
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

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await _pipeServer.StartAsync();
    }

    private void _pipeServer_MessageReceived(object? sender, string e)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                var mainVM = DataContext as MainViewModel;
                if (mainVM is null)
                    return;
                var args = ArgumentParserService.ParseMessage(e);
                var parsedArgs = ArgumentParserService.ParseStartUpArgs(args);
                var mode = parsedArgs.Item1;
                var filePaths = parsedArgs.Item2;

                if (filePaths.Count > 0 && (mode == "split" || mode == "combine" || mode == "add-to-combine"))
                {
                    if (mode == "split")
                    {
                        if (_resetUponNextMessage == true)
                            mainVM.SplitFileVM.ClearCommand.Execute(null);
                        filePaths.ForEach(f => mainVM.SplitFileVM.SourceFilePaths.Add(f));
                    }
                    else if (mode == "combine")
                    {
                        if (_resetUponNextMessage == true)
                            mainVM.CombineFilesVM.ClearCommand.Execute(null);
                        filePaths.ForEach(f => mainVM.CombineFilesVM.SourceFiles.Add(new(f)));
                    }
                    else if (mode == "add-to-combine")
                    {
                        filePaths.ForEach(f => mainVM.CombineFilesVM.SourceFiles.Add(new(f)));
                    }
                    LastCommand = mode;
                    _resetUponNextMessage = false;
                }
            });
        }
        catch (Exception ex) { ex.Message.ShowAsError(); }
    }

    private void _pipeServer_LastMessagedReceived(object? sender, bool e)
    {
        try
        {
            Dispatcher.Invoke(() =>
            {
                var mainVM = DataContext as MainViewModel;
                if (mainVM is null)
                    return;
                if (LastCommand == "split")
                {
                    mainVM.SplitFileVM.SplitFileCommand.Execute(null);
                    _resetUponNextMessage = true;
                }
                else if (LastCommand == "combine")
                {
                    mainVM.CombineFilesVM.CombineFilesCommand.Execute(null);
                    _resetUponNextMessage = true;
                }
            });
        }
        catch (Exception ex) { ex.Message.ShowAsError(); }
    }
}
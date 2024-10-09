using System.IO.Pipes;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StarPDFSolutionLibrary.Services.Editors;
using StarPDFSolutionWPF.ViewModels;
using StarPDFSolutionWPF.Services;
using System.Diagnostics;

namespace StarPDFSolutionWPF;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        ServiceCollection services = new();
        CofigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void CofigureServices(ServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<CombineFilesViewModel>();
        services.AddTransient<SplitFileViewModel>();
        services.AddTransient<IPDFEditorService, PDFSharpEditorService>();
        services.AddSingleton<PipeServer>(new PipeServer("StarPDFSolutionPipe"));
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        bool createdNew;
        var appGuid = GetType().GUID.ToString();
        Mutex mutex = new Mutex(true, appGuid, out createdNew);
        var isPipeRunning = Directory.GetFiles(@"\\.\pipe\").Contains(@"\\.\pipe\StarPDFSolutionPipe");
        // close this instance if another instance is already open
        // mutex eventually gets released by the first app, so if the pipe is running then we don't have
        // to rely on the mutex anymore, but the mutex is required in the beginning as the other methods are too slow
        if (createdNew == false || isPipeRunning)
        {
            try
            {
                PipeClient.SendMessage(ArgumentParserService.PrepareMessage(e.Args));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            Current.Shutdown();
        }
        else
        {
            GC.KeepAlive(mutex);
            var parsedArgs = ArgumentParserService.ParseStartUpArgs(e.Args);
            string? mode = parsedArgs.Item1;
            List<string> filePaths = parsedArgs.Item2;

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();

            var mainVM = mainWindow?.DataContext as MainViewModel;
            if (mainVM is null)
                return;
            if (filePaths.Count > 0 && (mode == "split" || mode == "combine" || mode == "add-to-combine"))
            {
                if (mode == "split")
                {
                    filePaths.ForEach(f => mainVM.SplitFileVM.SourceFilePaths.Add(new(f)));
                }
                else if (mode == "combine")
                {
                    filePaths.ForEach(f => mainVM.CombineFilesVM.SourceFiles.Add(new(f)));
                }
                else if (mode == "add-to-combine")
                {
                    filePaths.ForEach(f => mainVM.CombineFilesVM.SourceFiles.Add(new(f)));
                }
                mainWindow.LastCommand = mode;
            }
        }

    }
}

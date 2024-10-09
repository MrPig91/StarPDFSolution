using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace StarPDFSolutionWPF.Services
{
    public class PipeServer
    {
        private readonly string _pipeName;
        private NamedPipeServerStream _pipeServer;

        public event EventHandler<string> MessageReceived;
        public event EventHandler<bool> LastMessagedReceived;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public PipeServer(string pipeName)
        {
            _pipeName = pipeName;
        }

        public async Task StartAsync()
        {
            Stopwatch stopwatch = new();
            // Start the timer monitoring task
            var monitoringTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (stopwatch.ElapsedMilliseconds > 1000)
                    {
                        LastMessagedReceived?.Invoke(this, true);
                        stopwatch.Reset();
                    }

                    await Task.Delay(100); // Check every 100 milliseconds
                }
            }, _cancellationTokenSource.Token);
            stopwatch.Start();

            await Task.Run(async () =>
            {
                while (true)
                {
                    _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    await _pipeServer.WaitForConnectionAsync();
                    stopwatch.Restart();
                    using (var reader = new StreamReader(_pipeServer, Encoding.UTF8))
                    {
                        string message = await reader.ReadToEndAsync();
                        OnMessageReceived(message.Trim());
                    }
                }
            });

            _cancellationTokenSource.Cancel();
            await monitoringTask;
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(this, message);
        }
    }
}

using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using System.Threading.Tasks;


using System.Windows.Threading;
using System.ServiceModel.Channels;
using System.Windows;

namespace Nt.Utility
{
    public class UpdateService
    {
        private const string UpdateServiceUrl = "update.novacom.at";
        private readonly SolidColorBrush greenBrush;
        private readonly SolidColorBrush redBrush;
        private readonly MainWindow mainWindow;

        public bool IsUpdateServiceReachable { get; private set; }

        public UpdateService(SolidColorBrush green, SolidColorBrush red, MainWindow window)
        {
            greenBrush = green;
            redBrush = red;
            mainWindow = window;
        }

        public void CheckUpdateServiceStatus(object state)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(UpdateServiceUrl);

                    if (reply != null && reply.Status == IPStatus.Success)
                    {
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            mainWindow.statusEllipse_updateservice.Fill = greenBrush;
                        });
                        IsUpdateServiceReachable = true;
                    }
                    else
                    {
                        mainWindow.Dispatcher.Invoke(() =>
                        {
                            mainWindow.statusEllipse_updateservice.Fill = redBrush;
                        });
                        IsUpdateServiceReachable = false;
                    }
                }
            }
            catch (Exception)
            {
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.statusEllipse_updateservice.Fill = redBrush;
                });
                IsUpdateServiceReachable = false;
            }
        }
        public void StartUpdateNtFiscal()
        {
            string command = "powershell -ExecutionPolicy Bypass -Command \"Invoke-Expression (New-Object Net.WebClient).DownloadString('https://update.novacom.at/install_script/nt.fiscal.ps1')\"";
            Task.Run(() => ExecuteCommandAsync(command));
            Debug.WriteLine("StartUpdateNtFiscal method called.");
        }

        private async Task ExecuteCommandAsync(string command)
        {
            Debug.WriteLine("ExecuteCommandAsync started with command: " + command);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            Debug.WriteLine("Starting process...");
            process.Start();

            // Set up event handlers to read output in real time
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.NtFiscal_EventLog.Text += $"{DateTime.Now}: {args.Data}\n";
                        mainWindow.NtFiscal_EventLog.ScrollToEnd();
                    });
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.NtFiscal_EventLog.Text += $"{DateTime.Now}: ERROR - {args.Data}\n";
                        mainWindow.NtFiscal_EventLog.ScrollToEnd();
                    });
                }
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());
        }

    }
}

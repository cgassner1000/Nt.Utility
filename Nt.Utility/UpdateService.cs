using System;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.Windows.Threading;

namespace Nt.Utility
{
    public class UpdateService
    {
        private const string UpdateServiceUrl = "google.ad";
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
    }
}

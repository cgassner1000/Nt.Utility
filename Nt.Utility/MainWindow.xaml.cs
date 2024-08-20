using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Net;
using System.Reflection;
using Nt.Utility.Pages;

namespace Nt.Utility
{
    /// <summary>
    /// Interaktionslogik für MainMenuWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private NavigationHandler _navigationHandler;
        public MainWindow()
        {
            InitializeComponent();
            StartClock();
            UpdateHostInfo();
            MainFrame.Navigate(new MainMenu());
            //MainFrame.Navigated += Frame_Navigated;
            _navigationHandler = new NavigationHandler(MainFrame, FooterContentControl);
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "Version Unbekannt";
            this.Title = $"Nt.Utility - {versionString}";
        }
        //************************************************** HEADER **************************************************//
        private void StartClock()
        {
            // DispatcherTimer initialisieren
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += UpdateClock;
            _timer.Start();
        }

        private void UpdateClock(object sender, EventArgs e)
        {
            // Aktuelles Datum und Uhrzeit holen und im TextBlock anzeigen
            DateTimeTextBlock.Text = DateTime.Now.ToString("dd.MM.yyyy - HH:mm:ss");
        }

        private void UpdateHostInfo()
        {
            try
            {
                var hostName = Dns.GetHostName();
                var ipAddress = string.Empty;

                foreach (var ip in Dns.GetHostAddresses(hostName))
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ipAddress = ip.ToString();
                        break;
                    }
                }

                HostInfoTextBlock.Text = $"💻 {hostName} | IP: {ipAddress}";
            }
            catch (Exception ex)
            {
                HostInfoTextBlock.Text = $"Error retrieving host info: {ex.Message}";
            }
        }

        //private void Frame_Navigated(object sender, NavigationEventArgs e)
        //{
        //    if (e.Content is Page page)
        //    {
        //        FooterContentControl.Children.Clear();

        //        if (page is MainMenu) // Assuming MainMenu is the first page
        //        {
        //            //FooterContentControl.Content = null;
        //            Button Ende_Button = new Button
        //            {
        //                Content = "Beenden",
        //                Width = 150,
        //                Height = 40,
        //                HorizontalAlignment = HorizontalAlignment.Center,
        //                Margin = new Thickness(5)
        //            };
        //            Ende_Button.Click += Ende_Button_Click;
        //            FooterContentControl.Children.Add(Ende_Button);
        //        }
        //        else if (page is FiskaltrustConnection) //FiskaltrustMain
        //        {
        //            FooterContentControl.Children.Clear();
        //            Button Back_Button = new Button
        //            {
        //                Content = "Zurück",
        //                Width = 150,
        //                Height = 40,
        //                HorizontalAlignment = HorizontalAlignment.Center,
        //                Margin = new Thickness(5)
        //            };
        //            Back_Button.Click += Back_Button_Click;
        //            FooterContentControl.Children.Add(Back_Button);

        //            Button FiscalConnectButton = new Button
        //            {
        //                Content = "Verbinden",
        //                Width = 150,
        //                Height = 40,
        //                HorizontalAlignment = HorizontalAlignment.Right,
        //                Margin = new Thickness(5)
        //            };
        //            FiscalConnectButton.Click += Fiscal_Connect_Button_Click;
        //            FooterContentControl.Children.Add(FiscalConnectButton);

        //        }
        //        else
        //        {
        //            // Set different content for other pages, e.g., a button or another control
        //            Button Ende_Button = new Button
        //            {
        //                Content = "Beenden",
        //                Width = 150,
        //                Height = 40,
        //                HorizontalAlignment = HorizontalAlignment.Center,
        //                Margin = new Thickness(5)
        //            };
        //            Ende_Button.Click += Ende_Button_Click;
        //            FooterContentControl.Children.Add(Ende_Button);
        //        }
        //    }
        //}


        private void Ende_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Back_Button_Click(Object sender, RoutedEventArgs e)
        {
            MainFrame.GoBack();
        }



    }
}

using System.CodeDom;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Nt.Utility.IRISDatabase;
using Nt.Utility.Pages.Novatouch;

namespace Nt.Utility.Pages
{
    public class NavigationHandler
    {
        private readonly Frame _frame;
        private readonly StackPanel _footerContentControl;
        private FiskaltrustMain fiskaltrustMainPage;
        private FiskaltrustConnection fiskaltrustConnectionPage;
        private NovatouchMain novatouchMainPage;
        private DBConnection dbConnectionPage; 

        private IRISDBConnect database;

        public NavigationHandler(Frame frame, StackPanel footerContentControl)
        {
            _frame = frame;
            _footerContentControl = footerContentControl;
            _frame.Navigated += Frame_Navigated;
            //database = new Database();
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                _footerContentControl.Children.Clear();
                Debug.WriteLine($"Navigated to: {page.GetType().Name}"); // Debug output

                if (page is MainMenu) // Assuming MainMenu is the first page
                {
                    Button endeButton = new Button
                    {
                        Content = "Beenden",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    endeButton.Click += Ende_Button_Click;
                    _footerContentControl.Children.Add(endeButton);
                }
                else if (page is DBConnection) // DBConnection
                {
                    Button backButton = new Button
                    {
                        Content = "Zurück",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    backButton.Click += Back_Button_Click;
                    _footerContentControl.Children.Add(backButton);

                    Button DBconnectButton = new Button
                    {
                        Content = "Verbinden",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    DBconnectButton.Click += DB_Connect_Button_Click;
                    _footerContentControl.Children.Add(DBconnectButton);
                }
                else if (page is NovatouchMain) // NovatouchMain
                {
                    Button backButton = new Button
                    {
                        Content = "Zurück",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    backButton.Click += Back_Button_Click;
                    _footerContentControl.Children.Add(backButton);
                }
                else if (page is FiskaltrustConnection) // FiskaltrustConnection
                {
                    Button backButton = new Button
                    {
                        Content = "Zurück",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    backButton.Click += Back_Button_Click;
                    _footerContentControl.Children.Add(backButton);

                    Button FiscalconnectButton = new Button
                    {
                        Content = "Verbinden",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    FiscalconnectButton.Click += Fiscal_Connect_Button_Click;
                    _footerContentControl.Children.Add(FiscalconnectButton);
                }
                else if (page is FiskaltrustMain) // FiskaltrustMain
                {
                    Button backButton = new Button
                    {
                        Content = "Zurück",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    backButton.Click += Back_Button_Click;
                    _footerContentControl.Children.Add(backButton);
                }
                else
                {
                    Button endeButton = new Button
                    {
                        Content = "Beenden",
                        Width = 150,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    endeButton.Click += Ende_Button_Click;
                    _footerContentControl.Children.Add(endeButton);
                }
            }
        }

        private void Ende_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        private async void DB_Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("NavigationHandler.Database_Connect_Button_Click");
            if (_frame.Content is DBConnection dbConnectionPage)
            {
                    try
                    {
                    dbConnectionPage.SaveConnectionData();
                    var irisConnection = IRISDBConnect.Instance;
                    irisConnection.Connect();
                    _frame.Navigate(new NovatouchMain());

                    }
                    catch (Exception ex)
                    {
                        // Fehlerbehandlung, falls die Navigation fehlschlägt
                        MessageBox.Show($"Fehler beim Navigieren zur Hauptseite: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                
            }
        }

        private async void Fiscal_Connect_Button_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen, ob die aktuelle Seite eine FiskaltrustConnection-Seite ist
            if (_frame.Content is FiskaltrustConnection fiskaltrustConnectionPage)
            {
                // Fiskaltrust-Instanz erstellen und verbinden
                var fiskaltrust = fiskaltrustConnectionPage.InitializeFiskaltrust();

                if (fiskaltrust != null)
                {
                    try
                    {
                        // EchoAsync-Methode aufrufen, um die Verbindung zu testen
                        string response = await fiskaltrust.EchoAsync("Hello fiskaltrust.Middleware!");

                        if (!string.IsNullOrEmpty(response))
                        {
                            // Erfolgreich, navigieren zur FiskaltrustMain-Seite
                            _frame.Navigate(new FiskaltrustMain());
                        }
                        else
                        {
                            // Leere Antwort, Fehlermeldung anzeigen
                            MessageBox.Show("Die Verbindung zum Fiskaltrust-Server war nicht erfolgreich. Bitte überprüfen Sie Ihre Eingaben.",
                                            "Fehler",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Fehlerbehandlung
                        MessageBox.Show($"Fehler beim Testen der Verbindung: {ex.Message}",
                                        "Fehler",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Fehler bei der Initialisierung
                    MessageBox.Show("Fehler bei der Initialisierung der Fiskaltrust-Verbindung. Bitte überprüfen Sie Ihre Eingaben.",
                                    "Fehler",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            else
            {
                // Fehlerbehandlung, wenn die aktuelle Seite keine FiskaltrustConnection-Seite ist
                MessageBox.Show("Die aktuelle Seite ist nicht die Fiskaltrust-Verbindung.",
                                "Fehler",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

    }
}

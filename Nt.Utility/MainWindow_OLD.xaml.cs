using InterSystems.Data.IRISClient.ADO;
using InterSystems.Data.IRISClient;
using System.CodeDom;
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
using System.IO;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using InterSystems.Data.IRISClient.List;
using System.Printing;
using System.Security.AccessControl;
using Nt.Utility.Fiskaltrust;
using fiskaltrust.ifPOS.v1;
using System.Reflection;

namespace Nt.Utility
{
    /// <summary>
    /// Interaction logic for MainWindow_OLD.xaml
    /// </summary>
    public partial class MainWindow_OLD : Window
    {
        //private const string UpdateServiceUrl = "google.at";
        private readonly SolidColorBrush GreenBrush = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush RedBrush = new SolidColorBrush(Colors.Red);
        //private bool isUpdateServiceReachable = false;
        private System.Threading.Timer timer;
        private DispatcherTimer openTableTimer = new DispatcherTimer();
        private DispatcherTimer updatetimer = new DispatcherTimer();
        private Table table;
        private Database database;
        private UpdateService updateService;
        //private string server;
        //private string port;
        //private IRISConnection conn;
        //private IRIS iris;

        public MainWindow_OLD()

        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var versionString = version != null ? $"Version {version.Major}.{version.Minor}.{version.Build}" : "Version Unbekannt";
            this.Title = $"Nt.Utility - {versionString}";

            database = new Database();
            table = new Table(database);

            updatetimer = new DispatcherTimer();
            updateService = new UpdateService(GreenBrush, RedBrush, this);
            timer = new System.Threading.Timer(updateService.CheckUpdateServiceStatus, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            openTableTimer.Interval = TimeSpan.FromSeconds(5);
            //openTableTimer.Tick += (s, e) => UpdateOpenTablesUI();
           
            fiscal_startetTransactionNumber.TextChanged += Fiscal_startetTransactionNumber_TextChanged;
            fiscal_cashboxID.TextChanged += FiskalQueue_TextChanged;

            StartUpdateServiceStatusCheck();

            this.Closed += MainWindow_Closed;


        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            database.Disconnect();
        }

        private void MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTab.SelectedItem == manageTable)
            {
                //UpdateOpenTablesUI();
                openTableTimer.Start();
            }
            else
            {
                openTableTimer.Stop();
            }
        }



        // ************************************************ CONNECT TO DB ************************************************//


        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string DBserver = connserver.Text;
                string DBport = connport.Text;
                string DBnamespace = connnamespace.Text;
                string password = "SYS";
                string userId = "_SYSTEM";

                database.Connect(DBserver, DBport, DBnamespace, password, userId);
                statusEllipse_DB.Fill = database.StatusBrush;

            }
            catch (Exception ex)
            {
                statusEllipse_DB.Fill = new SolidColorBrush(Colors.Red);
                MessageBox.Show(ex.Message);
            }
        }

        // ************************************************ OPEN TABLES ************************************************//

        public class TableData
        {
            public string TISCH { get; set; }
            public string KEY { get; set; }
        }

        //public List<TableData> GetOpenTables(string FA)
        //{
        //    return table.GetOpenTables(FA);
        //}

        //private void UpdateOpenTablesUI()
        //{
        //    if (!database.CheckConnection())
        //    {
        //        opentablesWrapPanel.Children.Clear();
        //        return;
        //    }

        //    //var openTablesList = GetOpenTables(Nt_FA.Text);
        //    opentablesWrapPanel.Children.Clear();

        //    foreach (var table in openTablesList)
        //    {
        //        // Erstelle das Border-Element für jeden Tisch
        //        Border border = new Border
        //        {
        //            Width = 50,
        //            Height = 50,
        //            Background = Brushes.LightGray,
        //            BorderBrush = Brushes.Black,
        //            BorderThickness = new Thickness(1),
        //            Margin = new Thickness(5)
        //        };

        //        // Erstelle ein Grid, um die Tischnummer und Kellnernummer vertikal und horizontal zu zentrieren
        //        Grid grid = new Grid();
        //        grid.RowDefinitions.Add(new RowDefinition());
        //        grid.RowDefinitions.Add(new RowDefinition());

        //        // Tischnummer
        //        TextBlock tischTextBlock = new TextBlock
        //        {
        //            Text = table.TISCH,
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center
        //        };
        //        Grid.SetRow(tischTextBlock, 0);

        //        // Kellnernummer
        //        TextBlock kellnerTextBlock = new TextBlock
        //        {
        //            Text = table.KEY,
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center
        //        };
        //        Grid.SetRow(kellnerTextBlock, 1);

        //        // Füge die TextBlocks dem Grid hinzu
        //        grid.Children.Add(tischTextBlock);
        //        grid.Children.Add(kellnerTextBlock);

        //        // Setze das Grid als Child des Border-Elements
        //        border.Child = grid;

        //        // Speichere den TISCH als Tag im Border-Element
        //        border.Tag = table.TISCH;

        //        // Erstelle das Kontextmenü für dieses Border-Element
        //        ContextMenu contextMenu = new ContextMenu();
        //        MenuItem OpenTableMenuItemMerge = new MenuItem { Header = "Umbelegen ↹", Tag = table.TISCH };
        //        MenuItem OpenTableMenuItemKill = new MenuItem { Header = "Löschen 💀", Tag = table.TISCH };
        //        OpenTableMenuItemMerge.Click += Button_TableMerge;
        //        OpenTableMenuItemKill.Click += Button_KillTable_Click;
        //        contextMenu.Items.Add(OpenTableMenuItemMerge);
        //        contextMenu.Items.Add(OpenTableMenuItemKill);

        //        // Weise das Kontextmenü dem Border-Element zu
        //        border.ContextMenu = contextMenu;

        //        // Füge das Border-Element dem WrapPanel hinzu
        //        opentablesWrapPanel.Children.Add(border);
        //    }
        //}

        //        // Erstelle ein StackPanel, um die Tischnummer und Kellnernummer untereinander anzuzeigen
        //        StackPanel stackPanel = new StackPanel();

        //        // Tischnummer
        //        TextBlock tischTextBlock = new TextBlock
        //        {
        //            Text = table.TISCH,
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center
        //        };

        //        // Kellnernummer
        //        TextBlock kellnerTextBlock = new TextBlock
        //        {
        //            Text = table.KEY,
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center
        //        };

        //        // Füge die TextBlocks dem StackPanel hinzu
        //        stackPanel.Children.Add(tischTextBlock);
        //        stackPanel.Children.Add(kellnerTextBlock);

        //        // Setze das StackPanel als Child des Border-Elements
        //        border.Child = stackPanel;

        //        // Speichere den TISCH als Tag im Border-Element
        //        border.Tag = table.TISCH;

        //        // Erstelle das Kontextmenü für dieses Border-Element
        //        ContextMenu contextMenu = new ContextMenu();
        //        MenuItem OpenTableMenuItemMerge = new MenuItem { Header = "Umbelegen ↹", Tag = table.TISCH };
        //        MenuItem OpenTableMenuItemKill = new MenuItem { Header = "Löschen 💀", Tag = table.TISCH };
        //        OpenTableMenuItemMerge.Click += Button_TableMerge;
        //        OpenTableMenuItemKill.Click += Button_KillTable_Click;
        //        contextMenu.Items.Add(OpenTableMenuItemMerge);
        //        contextMenu.Items.Add(OpenTableMenuItemKill);

        //        // Weise das Kontextmenü dem Border-Element zu
        //        border.ContextMenu = contextMenu;

        //        // Füge das Border-Element dem WrapPanel hinzu
        //        opentablesWrapPanel.Children.Add(border);
        //    }
        //}

        //    {
        //        // Erstelle das Border-Element für jeden Tisch
        //        Border border = new Border
        //        {
        //            Width = 50,
        //            Height = 50,
        //            Background = Brushes.LightGray,
        //            BorderBrush = Brushes.Black,
        //            BorderThickness = new Thickness(1),
        //            Margin = new Thickness(5),
        //            Child = new TextBlock
        //            {
        //                Text = table.TISCH,
        //                HorizontalAlignment = HorizontalAlignment.Center,
        //                VerticalAlignment = VerticalAlignment.Center
        //            },
        //            // Speichere den TISCH als Tag im Border-Element
        //            Tag = table.TISCH
        //        };

        //        // Erstelle das Kontextmenü für dieses Border-Element
        //        ContextMenu contextMenu = new ContextMenu();
        //        MenuItem OpenTableMenuItemMerge = new MenuItem { Header = "Umbelegen ↹", Tag = table.TISCH };
        //        MenuItem OpenTableMenuItemKill = new MenuItem { Header = "Löschen 💀", Tag = table.TISCH };
        //        OpenTableMenuItemMerge.Click += Button_TableMerge;
        //        OpenTableMenuItemKill.Click += Button_KillTable_Click;
        //        contextMenu.Items.Add(OpenTableMenuItemMerge);
        //        contextMenu.Items.Add(OpenTableMenuItemKill);

        //        // Weise das Kontextmenü dem Border-Element zu
        //        border.ContextMenu = contextMenu;

        //        // Füge das Border-Element dem WrapPanel hinzu
        //        opentablesWrapPanel.Children.Add(border);
        //    }
        //}


        private ContextMenu CreateContextMenu(string tisch)
        {
            ContextMenu contextMenu = new ContextMenu();

            // MenuItem zum Bearbeiten
            MenuItem OpenTableMenuItemMerge = new MenuItem
            {
                Header = "Umbelegen",
                Tag = tisch // Speichere den Wert von 'tisch' in der Tag-Eigenschaft des MenuItems
            };
            OpenTableMenuItemMerge.Click += Button_TableMerge; // Eventhandler für Bearbeiten

            // MenuItem zum Löschen
            MenuItem OpenTableMenuItemkill = new MenuItem
            {
                Header = "Tisch Löschen",
                Tag = tisch // Speichere den Wert von 'tisch' in der Tag-Eigenschaft des MenuItems
            };
            OpenTableMenuItemkill.Click += (sender, e) => Button_KillTable_Click(sender, e); // Direkter Aufruf von Button_KillTable_Click

            // Füge die MenuItem zum Kontextmenü hinzu
            contextMenu.Items.Add(OpenTableMenuItemMerge);
            contextMenu.Items.Add(OpenTableMenuItemkill);

            return contextMenu;
        }

        // ************************************************ OPEN TABLES MENU ************************************************//
        //private void SetupContextMenu(string tisch)
        //{
        //    // Erzeuge ein neues Kontextmenü
        //    ContextMenu contextMenu = new ContextMenu();

        //    // MenuItem zum Löschen
        //    MenuItem OpenTableMenuItemKill = new MenuItem
        //    {
        //        Header = "Löschen2"
        //    };
        //    MessageBox.Show($"Tisch {tisch} ausgewählt");
        //    OpenTableMenuItemKill.Click += (sender, e) => Button_KillTable_Click(sender, e); // Eventhandler für Löschen

        //    // Füge die MenuItem zum Kontextmenü hinzu
        //    contextMenu.Items.Add(OpenTableMenuItemKill);

        //    // Setze das Kontextmenü für das UI-Element (z.B. Border)
        //    if (opentablesWrapPanel.Children.Count > 0)
        //    {
        //        if (opentablesWrapPanel.Children[0] is Border border)
        //        {
        //            border.ContextMenu = contextMenu;
        //        }
        //    }
        //}



        //private void OpenTable_MenuItem_kill_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show($"Tisch ausgewählt");

        //}
        //private void OpenTable_MenuItem_merge_Click(object sender, RoutedEventArgs e)
        //{

        //}



        // ************************************************ TABLE KILL ************************************************//
        private void Button_KillTable_Click(object sender, RoutedEventArgs e)
        {
            if (!database.CheckConnection())
            {
                MessageBox.Show("Keine Verbindung zur Datenbank. Bitte zuerst verbinden.");
                return; // Verlasse die Methode, wenn keine Verbindung besteht
            }

            MenuItem menuItem = sender as MenuItem;

            

            try
            {
                string tisch = menuItem?.Tag as string;
                string FA = Nt_FA.Text;
                string TI = tisch ?? fromTable.Text; // Wenn 'tisch' null ist, verwende 'fromTable.Text'

                table.Kill(FA, TI);

                //UpdateOpenTablesUI();
            }
            catch (Exception ex)
            {
                statusEllipse_DB.Fill = new SolidColorBrush(Colors.Red);
                MessageBox.Show($"Fehler beim Verbindungsaufbau zur Datenbank: {ex.Message}");
            }
        }



        // ************************************************ TABLE MERGE START ************************************************//


        private void Button_TableMerge(object sender, RoutedEventArgs e)
        {
            if (!database.CheckConnection())
            {
                return;
            }
            MenuItem menuItem = sender as MenuItem;

            try
            {
                string tisch = menuItem?.Tag as String;
                string FA = Nt_FA.Text;
                string KASSA = ManageTableInput_KASSA.Text;
                string KEY = ManageTableInput_KEY.Text;
                string von_TISCH = tisch ?? fromTable.Text;
                


                // Eingabeaufforderung für auf_TISCH

                //TODO: Die Abfrage soll nur kommen, wenn beide Textboxen leer sind. Sie kommt aber auch, wenn beide befüllt sind
                string auf_TISCH = nach_TISCH_inputBOX.Show("Bitte geben Sie den Ziel-Tisch ein:", "Tisch Umbelegen");

                if (string.IsNullOrWhiteSpace(auf_TISCH))
                {
                    MessageBox.Show("Ziel-Tisch darf nicht leer sein.");
                    return;
                }

                table.Merge(von_TISCH, auf_TISCH, FA, KASSA, KEY);

                //UpdateOpenTablesUI();

                MessageBox.Show($"Tisch {von_TISCH} erfolgreich auf Tisch {auf_TISCH} umbelegt");
                LogEvent($"NOVACOM: Tisch {von_TISCH} -> Kellner {KEY} Tisch {auf_TISCH}");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Kopieren der Subglobals: {ex.Message}");
            }
        }

        public static class nach_TISCH_inputBOX
        {
            public static string Show(string prompt, string title)
            {
                Window window = new Window
                {
                    Title = title,
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                StackPanel stackPanel = new StackPanel();

                TextBlock textBlock = new TextBlock
                {
                    Text = prompt,
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(textBlock);

                TextBox textBox = new TextBox
                {
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(textBox);

                Button buttonOk = new Button
                {
                    Content = "OK",
                    Width = 60,
                    Height = 25,
                    Margin = new Thickness(10),
                    IsDefault = true,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                buttonOk.Click += (sender, e) => { window.DialogResult = true; window.Close(); };
                stackPanel.Children.Add(buttonOk);

                window.Content = stackPanel;
                window.ShowDialog();

                return textBox.Text;
            }
        }


        // ************************************************ CHECK CONNECTION ************************************************//
        private void StartUpdateServiceStatusCheck()
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            timer.Tick += (s, e) => updateService.CheckUpdateServiceStatus(null);
            timer.Start();
        }

        //private void CheckUpdateServiceStatus(object state)
        //{
        //    try
        //    {
        //        using (Ping ping = new Ping())
        //        {
        //            PingReply reply = ping.Send(UpdateServiceUrl);

        //            if (reply != null && reply.Status == IPStatus.Success)
        //            {
        //                Dispatcher.Invoke(() =>
        //                {
        //                    statusEllipse_updateservice.Fill = GreenBrush;
        //                });
        //                isUpdateServiceReachable = true;
        //            }
        //            else
        //            {
        //                Dispatcher.Invoke(() =>
        //                {
        //                    statusEllipse_updateservice.Fill = RedBrush;
        //                });
        //                isUpdateServiceReachable = false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Dispatcher.Invoke(() =>
        //        {
        //            statusEllipse_updateservice.Fill = RedBrush;
        //        });
        //        isUpdateServiceReachable = false;
        //    }
        //}
        // ************************************************ xxx ************************************************//


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Clear();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RequirementsButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Button_disconnect_Click(object sender, RoutedEventArgs e)
        {
            database.Disconnect();
            Dispatcher.Invoke(() =>
            {
                statusEllipse_DB.Fill = database.StatusBrush;
            });

        }
        private void connserver_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "127.0.0.1"; // Setzen Sie hier den ursprünglichen Standardtext ein
            }
        }
        private void connport_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "1972"; // Setzen Sie hier den ursprünglichen Standardtext ein
            }
        }
        private void connnamespace_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "USER"; // Setzen Sie hier den ursprünglichen Standardtext ein
            }
        }
        private void connNt_FA_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "1001"; // Setzen Sie hier den ursprünglichen Standardtext ein
            }
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!database.CheckConnection())
            {
                return;
            }
            //UpdateOpenTablesUI();
        }


        // ********************************************** FISKALTRUST **************************************************** //

        private Nt.Utility.Fiskaltrust.Fiskaltrust InitializeFiskaltrust()
        {
            string server = fiscal_server.Text;
            string port = fiscal_port.Text;
            string cashboxID = fiscal_cashboxID.Text;
            string terminalID = fiscal_terminalID.Text;
            string userID = fiscal_userID.Text;
            string openTransaktions = fiscal_startetTransactionNumber.Text;

            try
            {
                var fiskaltrust = new Nt.Utility.Fiskaltrust.Fiskaltrust(server, port, cashboxID, terminalID, userID, openTransaktions);
                fiskaltrust.FiskalConnect();

                return fiskaltrust;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                return null;
            }

                
        }


        private async void fiskal_echo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fiskaltrust = InitializeFiskaltrust();
                // Call the EchoAsync method and pass the message
                string responseMessage = await fiskaltrust.EchoAsync("Hello fiskaltrust.Middleware!");


                MessageBox.Show($"Echo erfolgreich: {responseMessage}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: " + responseMessage);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private async void create_start_receipt_Click(object sender, RoutedEventArgs e)
        {
            
            var result = MessageBox.Show("Inbetriebnahme wirklich durchführen?", "Frage für einen Freund", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.CreateStartReceiptAsync();

                MessageBox.Show($"Startbeleg erfolgreich erstellt: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Startbeleg erstellt " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogEvent("FISKATRUST ERROR: Startbeleg nicht erstellt " + ex.Message);
            }
        }

        private async void create_stop_receipt_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Außerbetriebnahme!!!\n\nDabei wird die Queue und SCU deaktiviert.\n\nEin Rollback ist nicht mehr möglich.", "Frage für einen Freund", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.OutOfOperationAsync();

                MessageBox.Show($"Außerbetriebnahme erfolgreich: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Außerbetriebnahme erfolgreich: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void zero_receipt_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.ZeroReceiptAsync();

                MessageBox.Show($"Nullbeleg erfolgreich erstellt: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Nullbeleg erstellt: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void zero_receipt_TSEInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.ZeroReceiptTSEInfoAsync();

                MessageBox.Show($"Nullbeleg mit TSEInfo erfolgreich erstellt: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Nullbeleg + TSE Info: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void daily_closing_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Soll ein Tagesabschluss durchgeführt werden?", "Wichtige Information", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.DailyClosingAsync();

                MessageBox.Show($"Tagesende erfolgreich erstellt: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Tagesabschluss erstellt " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void zero_receipt_closeTransaktions_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.ZeroReceiptCloseTransaktionsAsync();

                MessageBox.Show($"Nullbeleg erfolgreich erstellt: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Offene Transaktionen geschlossen: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void initiate_SCU_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Initialisiert einen SCU tausch!\n\nDabei wird die aktuell zugewiesene SCU von der Queue getrennt und NICHT deaktiviert. Die SCU kann (später) weiterhin verwendet werden.\n\nVor durchführung muss der SCU-tausch im Portal vorbereitet werden #targetSCU.\n\nUm den Vorgang abzuschließen, muss im Anschluss Finish-SCU ausgeführt werden!", "Wichtige Information", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.InitiateSCUAsync();

                MessageBox.Show($"INITIATE SCU erfolgreich: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Initiate SCU erfolgreich: " + receiptID);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void initiate_SCU_deaktivate_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Initialisiert einen SCU tausch!\n\nDabei wird die aktuell zugewiesene SCU von der Queue getrennt und deaktiviert.\n\nVor durchführung muss der SCU-tausch im Portal vorbereitet werden #targetSCU.\n\nUm den Vorgang abzuschließen, muss im Anschluss Finish-SCU ausgeführt werden!", "Wichtige Information", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.InitiateSCUDeaktivateAsync();

                MessageBox.Show($"INITIATE SCU erfolgreich: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Initiate SCU schwitch mit SCU deaktivierung erfolgreich: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void finish_SCU_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Finalisiert den SCU-tausch!\n\nDieser Prozess verbindet Queue mit der neuen SCU.\n\nZuvor muss ein Initiate SCU durchgeführt werden.", "Wichtige Information", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.FinishSCUAsync();

                MessageBox.Show($"Finish SCU erfolgreich: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LogEvent("FISKATRUST: Finish SCU erfolgreich: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DSFinVK_export_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TAR_middleware_export_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void fail_multi_transaction_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fiscal_startetTransactionNumber.Text) || fiscal_startetTransactionNumber.Text == "1, 2, 3, 4, etc.")
            {
                MessageBox.Show("Bitte gültige (offene) Transaktionsnummern eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                fiscal_startetTransactionNumber.Focus();
                fiscal_startetTransactionNumber.Background = new SolidColorBrush(Colors.Yellow);
                return;
            }

            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                string receiptID = await fiskaltrust.FailTransaktionsMultiReceiptAsync();

                MessageBox.Show($"Offene Transaktionen erfolgreich geschlossen: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //private async void ExportJournal_Click(object sender, RoutedEventArgs e)
        //{
        //    var fromDate = DateTime.Parse(export_from_date.Text);
        //    var toDate = DateTime.Parse(export_to_date.Text);

        //    try
        //    {
        //        CheckFiscalQueueInput();
        //        var fiskaltrust = InitializeFiskaltrust();
        //        var journalData = await fiskaltrust.ExportJournalAsync(fromDate, toDate);

        //        MessageBox.Show($"DSFinV-K erfolgreich erstellt: {journalData}", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
        //        LogEvent("FISKATRUST: DSFinV-K export erstellt: " + journalData);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        // }

        private void Fiscal_startetTransactionNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(fiscal_startetTransactionNumber.Text) || fiscal_startetTransactionNumber.Text != "1, 2, 3, etc.")
            {
                fiscal_startetTransactionNumber.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void FiskalQueue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(fiscal_cashboxID.Text) || fiscal_cashboxID.Text != "00000000-0000-0000-0000-000000000000")
            {
                fiscal_cashboxID.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void CheckFiscalQueueInput()
        {
            if (string.IsNullOrWhiteSpace(fiscal_cashboxID.Text) || fiscal_cashboxID.Text == "00000000-0000-0000-0000-000000000000")
            {
                fiscal_cashboxID.Focus();
                fiscal_cashboxID.Background = new SolidColorBrush(Colors.Yellow);
                throw new InvalidFiscalQueueInputException("Bitte gültige CashboxID eingeben.");
                
            }

            fiscal_cashboxID.Background = new SolidColorBrush(Colors.White);
        }
        public class InvalidFiscalQueueInputException : Exception
        {
            public InvalidFiscalQueueInputException(string message) : base(message)
            {
            }
        }


        private void NtFiscalUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateService.StartUpdateNtFiscal();
        }

        public void LogEvent(string eventMessage)
        {
            EventLog.Text += $"{DateTime.Now}: {eventMessage}\n";
            EventLog.ScrollToEnd();
        }

        private void ExportJournal_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
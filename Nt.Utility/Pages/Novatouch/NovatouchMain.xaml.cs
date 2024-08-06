using Nt.Utility.IRISDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Nt.Utility.Pages.Novatouch
{
    /// <summary>
    /// Interaktionslogik für NovatouchMain.xaml
    /// </summary>
    public partial class NovatouchMain : Page
    {
        private Printer printer;
        private Dictionary<string, string> printerDictionary;
        private ManageTable _manageTable;
        private DispatcherTimer dispatcherTimer;
        public NovatouchMain()
        {
            InitializeComponent();
            FA.Text = IRISDBData.Instance.FA;
            printer = new IRISDatabase.Printer();
            LoadPrinters();
            _manageTable = new ManageTable();
            LoadTables();
            LoadTextBoxData();
            InitializeTimer();

        }

        private void InitializeTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }


        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            _manageTable.UpdateOpenTables(TableCanvas, FA.Text);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }


        private void Kill_Table_Button_Click(object sender, RoutedEventArgs e)
        {

             try
             { 
                 var manageTable = new ManageTable();
                 manageTable.Kill(FA.Text, TI.Text);
                 MessageBox.Show($"Tisch {TI.Text} erfolgreich gelöscht");
             }
                 catch (Exception ex)
             {
                 MessageBox.Show($"Fehler beim Löschen des Tischs {TI}: {ex.Message}");
             }
        }

        private void LoadTables()
        {
            try
            {
                var manageTable = new ManageTable();
                var openTables = manageTable.GetOpenTables(FA.Text);
                manageTable.DrawTables(TableCanvas, openTables);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Tische: {ex.Message}");
            }
        }

        private void TableCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _manageTable.UpdateOpenTables(TableCanvas, FA.Text);
        }





        // **********************************************************************************************************//
        // ***************** PRINTER *******************************************************************************//
        // **********************************************************************************************************//
        private void LoadPrinters()
        {
            try
            {
                var printers = new Printer();
                printerDictionary = printers.GetPrinterList($"{FA}"); // Verwende die entsprechende FA-Nummer

                if (printerDictionary.Count == 0)
                {
                    Printer_List.Items.Add("Keinen Drucker gefunden");
                    Printer_List.SelectedIndex = 0;
                }
                else
                {
                    Printer_List.ItemsSource = printerDictionary.Keys;
                    Printer_List.SelectedIndex = 0; // Der erste Drucker wird standardmäßig ausgewählt
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Druckerliste: {ex.Message}");
            }
        }

        private void Printer_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Sicherstellen, dass eine Auswahl getroffen wurde
            if (Printer_List.SelectedItem != null)
            {
                string selectedPrinterName = Printer_List.SelectedItem.ToString();

                // Überprüfen, ob der ausgewählte Drucker im Dictionary vorhanden ist
                if (printerDictionary.ContainsKey(selectedPrinterName))
                {
                    try
                    {
                        // Setze den ausgewählten Drucker
                        printer.SetSelectedPrinter(selectedPrinterName, printerDictionary);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fehler beim Setzen des Druckers: {ex.Message}");
                    }
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Verwende den Dispatcher, um die Markierung nach dem Fokusereignis zu setzen
                textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Save_Values();
        }

        private void Test_Print_Button_Click(object sender, RoutedEventArgs e)
        {
            printer.Print_Test_Page();
        }

        private void LoadTextBoxData()
        {
            Debug.WriteLine("Werte holen: FA, KASSA, KEY");
            FA.Text = IRISDBData.Instance.FA;
            RK.Text = IRISDBData.Instance.KASSA;
            KEY.Text = IRISDBData.Instance.KEY;
        }

        private void Save_Values_Button_Click(object sender, RoutedEventArgs e)
        {
            Save_Values();
        }

        private void Save_Values()
        {
            // Speichere die Werte der TextBoxen in IRISDBData, wenn der Fokus verloren wird
            Debug.WriteLine("Werte speichern: FA, KASSA, KEY");
            IRISDBData.Instance.FA = FA.Text;
            IRISDBData.Instance.KASSA = RK.Text;
            IRISDBData.Instance.KEY = KEY.Text;
        }
    }
}

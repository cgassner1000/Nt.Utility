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

namespace Nt.Utility.Pages.Novatouch
{
    /// <summary>
    /// Interaktionslogik für DBConnection.xaml
    /// </summary>
    public partial class DBConnection : Page
    {
        public DBConnection()
        {
            InitializeComponent();
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

        public bool SaveConnectionData()
        {
            Debug.WriteLine("SaveConnectionData");
            try
            {
                IRISDBData.Instance.SaveIRISDBData(
                    db_server.Text,
                    db_port.Text,
                    db_namespace.Text,
                    db_FA.Text,
                    null, // Hier kannst du weitere Parameter entsprechend hinzufügen
                    null,
                    null,
                    null
                );
                return true;
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung, falls das Speichern fehlschlägt
                MessageBox.Show($"Fehler beim Speichern der Verbindungsdaten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}

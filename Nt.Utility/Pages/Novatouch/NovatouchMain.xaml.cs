using Nt.Utility.IRISDatabase;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nt.Utility.Pages.Novatouch
{
    /// <summary>
    /// Interaktionslogik für NovatouchMain.xaml
    /// </summary>
    public partial class NovatouchMain : Page
    {
        public NovatouchMain()
        {
            InitializeComponent();
            FA.Text = IRISDBData.Instance.FA;
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
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Verwende den Dispatcher, um die Markierung nach dem Fokusereignis zu setzen
                textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
            }
        }
    }
}

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

namespace Nt.Utility
{
    /// <summary>
    /// Interaktionslogik für FiskaltrustConnection.xaml
    /// </summary>
    public partial class FiskaltrustConnection : Page
    {
        private FiskaltrustMain fiskaltrustMainPage;
        public FiskaltrustConnection()
        {
            InitializeComponent();
        }

        public Nt.Utility.Fiskaltrust.Fiskaltrust InitializeFiskaltrust()
        {
            string server = fiscal_server.Text;
            string port = fiscal_port.Text;
            string cashboxID = fiscal_cashboxID.Text;
            string terminalID = "001";
            string userID = "Novacom";
            string openTransactions = "1,2,3,etc";

            FiskaltrustData.Instance.SaveFiskaltrustData(server, port, cashboxID, terminalID, userID, openTransactions);


            //string terminalID = fiskaltrustMainPage.fiscal_terminalID.Text;
            //string userID = fiskaltrustMainPage.fiscal_userID.Text;
            //string openTransaktions = fiskaltrustMainPage.fiscal_startetTransactionNumber.Text;

 
            try
            {
                var fiskaltrust = new Nt.Utility.Fiskaltrust.Fiskaltrust(server, port, cashboxID, terminalID, userID, openTransactions);
                fiskaltrust.FiskalConnect();

                return fiskaltrust;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;
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

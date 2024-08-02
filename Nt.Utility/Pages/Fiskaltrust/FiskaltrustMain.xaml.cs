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
    /// Interaktionslogik für Fiskaltrust.xaml
    /// </summary>
    public partial class FiskaltrustMain : Page
    {
        public FiskaltrustMain()
        {
            InitializeComponent();
            LoadFiskaltrustData();
        }

        private void LoadFiskaltrustData()
        {
            var data = FiskaltrustData.Instance;
            fiscal_server.Text = data.Server;
            fiscal_port.Text = data.Port;
            fiscal_cashboxID.Text = data.CashboxID;
            fiscal_terminalID.Text = data.TerminalID;
            fiscal_userID.Text = data.UserID;
            fiscal_startetTransactionNumber.Text = data.OpenTransactions;
            // Set other properties as needed
        }

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
        private void CheckFiscalQueueInput()
        {
            if (string.IsNullOrWhiteSpace(fiscal_cashboxID.Text) || fiscal_cashboxID.Text == "00000000-0000-0000-0000-000000000000")
            {
                fiscal_cashboxID.Focus();
                fiscal_cashboxID.Background = new SolidColorBrush(Colors.Yellow);
                //throw new InvalidFiscalQueueInputException("Bitte gültige CashboxID eingeben.");

            }

            fiscal_cashboxID.Background = new SolidColorBrush(Colors.White);
        }

        private async void fiskal_echo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fiskaltrust = InitializeFiskaltrust();
                // Call the EchoAsync method and pass the message
                string responseMessage = await fiskaltrust.EchoAsync("Hello fiskaltrust.Middleware!");


                MessageBox.Show($"Echo erfolgreich: {responseMessage}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //LogEvent("FISKATRUST: " + responseMessage);

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
                //LogEvent("FISKATRUST: Startbeleg erstellt " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //LogEvent("FISKATRUST ERROR: Startbeleg nicht erstellt " + ex.Message);
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
                //LogEvent("FISKATRUST: Außerbetriebnahme erfolgreich: " + receiptID);
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
                //LogEvent("FISKATRUST: Nullbeleg erstellt: " + receiptID);
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

                UpdateStartedTransactionNumberTextBox();

                MessageBox.Show($"Nullbeleg mit TSEInfo erfolgreich erstellt: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //LogEvent("FISKATRUST: Nullbeleg + TSE Info: " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStartedTransactionNumberTextBox()
        {
            fiscal_startetTransactionNumber.Text = FiskaltrustData.Instance.CurrentStartedTransactionNumbers ?? "keine offenen Transaktionen";
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
                //LogEvent("FISKATRUST: Tagesabschluss erstellt " + receiptID);
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
                //LogEvent("FISKATRUST: Offene Transaktionen geschlossen: " + receiptID);
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
                //LogEvent("FISKATRUST: Initiate SCU erfolgreich: " + receiptID);

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
                //LogEvent("FISKATRUST: Initiate SCU schwitch mit SCU deaktivierung erfolgreich: " + receiptID);
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
                //LogEvent("FISKATRUST: Finish SCU erfolgreich: " + receiptID);
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
        //        //LogEvent("FISKATRUST: DSFinV-K export erstellt: " + journalData);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

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


        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
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
        private void Fiscal_Custom_Command_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void Fiscal_Custom_Command_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Eigener Befehl - Bitte Vorsicht!!!\n\nDabei können irreversible Schäden an der Middleware verursacht werden!!!", "Wichtige Information", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                // Lese den FtReceiptCase-Wert aus der TextBox aus
                if (long.TryParse(Fiscal_Custom_Command_TextBox.Text, out long ftReceiptCase))
                {
                    string receiptID = await fiskaltrust.CustomCommandAsync(ftReceiptCase);
                    MessageBox.Show($"Eigener Befehl erfolgreich: {receiptID}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Ungültiger Wert für FtReceiptCase", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Fiscal_Export_DSFinVK_Click(object sender, RoutedEventArgs e)
        {
            //var result = MessageBox.Show("Soll ein DSFinV-K durchgeführt werden?", "Wichtige Information", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Cancel)
            //{
            //    return;
            //}
            try
            {
                CheckFiscalQueueInput();
                var fiskaltrust = InitializeFiskaltrust();

                DateTime fromDate = fiscal_export_from.SelectedDate ?? DateTime.MinValue;
                DateTime toDate = fiscal_export_to.SelectedDate ?? DateTime.MinValue;

                string receiptID = await fiskaltrust.FiscalExportDSFinVKAsync(fromDate, toDate);

                MessageBox.Show($"DSFinV-K erfolgreich exportiert: {receiptID}\n\nExport im ServiceFolder der Middleware (zb. C:\\Fiskaltrust\\service\\Exports)", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //LogEvent("FISKATRUST: Tagesabschluss erstellt " + receiptID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

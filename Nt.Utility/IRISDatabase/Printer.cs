using InterSystems.Data.IRISClient;
using InterSystems.Data.IRISClient.Gateway;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static Grpc.Core.Metadata;

namespace Nt.Utility.IRISDatabase
{
    public class Printer
    {
        private readonly IRISDBConnect irisConnection;
        private string selectedDevice;

        public Printer()
        {
            irisConnection = IRISDBConnect.Instance;

            if (!irisConnection.CheckConnection())
            {
                throw new InvalidOperationException("Keine IRIS-Verbindung vorhanden.");
            }
        }

        public Dictionary<string, string> GetPrinterList(string FA)
        {
            Debug.WriteLine("GetPrinterList");
            var printerDictionary = new Dictionary<string, string>();

            if (!irisConnection.CheckConnection())
            {
                throw new InvalidOperationException("Keine IRIS-Verbindung vorhanden.");
            }

            try
            {
                // SQL-Abfrage zum Abrufen der Druckerinformationen
                string query = "SELECT FA, KassenNr, KASSA, bez, devtype, Device, VKO, bonstorno, bonaendformat, AlternativDevice, passiv, bemerkung, Sort, KassenType FROM NTQRY.QryDrucker";

                using (IRISCommand command = new IRISCommand(query, irisConnection.conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Debug.WriteLine("Keinen Drucker gefunden.");
                        }

                        while (reader.Read())
                        {
                            string deviceName = reader.GetString(2);
                            string deviceValue = reader.GetString(5); // Annahme: Device ist die 6. Spalte (Index 5)

                            Debug.WriteLine($"Gefundener Drucker: {deviceName}, Device-Wert: {deviceValue}");
                            printerDictionary[deviceName] = deviceValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Abrufen der Druckerliste: {ex.Message}");
            }

            // Sortieren der Drucker-Liste
            var sortedPrinters = printerDictionary.Keys.OrderBy(p => GetSortOrder(p).sortOrder).ThenBy(p => GetNumber(p)).ToList();

            return sortedPrinters.ToDictionary(p => p, p => printerDictionary[p]);
        }

        private (int sortOrder, int number) GetSortOrder(string deviceName)
        {
            // Bestimmt den Sortierwert basierend auf dem Präfix
            int sortOrder;
            if (deviceName.StartsWith("RD"))
                sortOrder = 1;
            else if (deviceName.StartsWith("SB"))
                sortOrder = 2;
            else if (deviceName.StartsWith("KB"))
                sortOrder = 3;
            else
                sortOrder = 4; // Optional für nicht definierte Präfixe

            return (sortOrder, GetNumber(deviceName));
        }

        private int GetNumber(string deviceName)
        {
            // Extrahiert die Nummer aus dem Druckernamen
            var match = Regex.Match(deviceName, @"\d+");
            return match.Success ? int.Parse(match.Value) : 0;
        }

        public void SetSelectedPrinter(string printerName, Dictionary<string, string> printerDictionary)
        {
            Debug.WriteLine($"SetSelectedPrinter Aufgerufen: {printerName}");

            if (printerDictionary.TryGetValue(printerName, out string deviceValue))
            {
                selectedDevice = deviceValue;
                Debug.WriteLine($"SelectedDevice gesetzt auf: {selectedDevice}");
            }
            else
            {
                throw new InvalidOperationException("Der ausgewählte Drucker hat keinen zugehörigen Device-Wert.");
            }
        }


        public void Print_Test_Page()
        {
            //string DEV = "file://c:\\print2file\\RD92.txt";
            Debug.WriteLine($"Ausgewählter Drucker: {selectedDevice}");
            string ZEILEN = "3";

            Debug.WriteLine("Print Test Page");
            var printers = new List<string>();

            if (!irisConnection.CheckConnection())
            {
                throw new InvalidOperationException("Keine IRIS-Verbindung vorhanden.");
            }

            var iris = irisConnection.iris ?? throw new InvalidOperationException("IRIS-Instanz ist null.");

            iris.FunctionString("TEST", "awk1STDR", selectedDevice, ZEILEN);

        }
    }
}

using System;
using InterSystems.Data.IRISClient;
using System.Windows.Media;
using InterSystems.Data.IRISClient.ADO;
using System.Windows;

namespace Nt.Utility
{
    internal class Database
    {
        public IRISConnection? conn { get; private set; }
        public IRIS? iris { get; private set; }

        public IRISConnection? Connection => conn;
        public bool IsConnected => iris != null;

        public SolidColorBrush? StatusBrush { get; private set; }

        public IRIS Connect(string server, string port, string dbNamespace, string password, string userId)
        {
            try
            {
                conn = new IRISConnection
                {
                    ConnectionString = $"Server={server};Port={port};Namespace={dbNamespace};Password={password};User ID={userId};"
                };
                conn.Open();
                iris = IRIS.CreateIRIS(conn);
                StatusBrush = new SolidColorBrush(Colors.Green);
                return iris;
            }
            catch (Exception ex)
            {
                StatusBrush = new SolidColorBrush(Colors.Red);
                throw new Exception($"Fehler beim Verbindungsaufbau zur Datenbank: {ex.Message}");
            }
        }

        public bool CheckConnection()
        {
            if (iris == null)
            {
                MessageBox.Show("Keine Verbindung zur Datenbank. Bitte zuerst verbinden.");
                StatusBrush = new SolidColorBrush(Colors.Red);

                return false;
            }
            return true;
        }

        public void Disconnect()
        {
            if (iris != null)
            {
                iris.Close();
                iris = null;
            }

            if (conn != null)
            {
                conn.Close();
                conn = null;
            }

            StatusBrush = new SolidColorBrush(Colors.Red);
        }
        public static string GetIRISTimestamp()
        {
            // Datum für den Startpunkt des IRIS $H-Timestamps
            DateTime irisEpoch = new DateTime(1840, 12, 31);

            // Aktuelles Datum und Uhrzeit
            DateTime now = DateTime.UtcNow;

            // Berechnung der Anzahl der Tage seit dem Startpunkt
            TimeSpan timeSinceEpoch = now - irisEpoch;
            int daysSinceEpoch = (int)timeSinceEpoch.TotalDays;

            // Berechnung der Anzahl der Sekunden seit Mitternacht
            TimeSpan timeOfDay = now.TimeOfDay;
            int secondsSinceMidnight = (int)timeOfDay.TotalSeconds;

            // Formatierung als $H-Timestamp
            return $"{daysSinceEpoch},{secondsSinceMidnight:D5}"; // Die Sekunden werden auf 5 Stellen aufgefüllt
        }
    }
}

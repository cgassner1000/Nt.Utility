using System;
using InterSystems.Data.IRISClient;
using System.Windows.Media;
using InterSystems.Data.IRISClient.ADO;
using System.Windows;
using System.Diagnostics;

namespace Nt.Utility.IRISDatabase
{
    public class IRISDBConnect
    {
        private static IRISDBConnect _instance;

        public static IRISDBConnect Instance => _instance ?? (_instance = new IRISDBConnect());

        public IRISConnection? conn { get; private set; }
        public IRIS? iris { get; private set; }

        private IRISDBConnect() { }

        public void Connect()
        {
            try
            {
                string PWS = "SYS";
                string USERID = "_SYSTEM";
 
                Disconnect();
                Debug.WriteLine("Mitten im Connect");
                var data = IRISDBData.Instance;
                conn = new IRISConnection
                {
                    ConnectionString = $"Server={data.Server};Port={data.Port};Namespace={data.Nt_Namepsace};Password={PWS}; User ID={USERID};"
                };
                conn.Open();
                iris = IRIS.CreateIRIS(conn);
                Debug.WriteLine($"IRIS: Database.Connect hergestellt: {iris}");
                //return iris;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Verbindungsaufbau zur Datenbank: {ex.Message}");
            }
        }

        public bool CheckConnection()
        {
            Debug.WriteLine("IRIS: Database.CheckConnection");
            if (iris == null || conn == null || conn.State != System.Data.ConnectionState.Open)
            {
                Debug.WriteLine("Verbindung ist nicht vorhanden oder geschlossen, erneuter Verbindungsaufbau.");
                Connect();
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

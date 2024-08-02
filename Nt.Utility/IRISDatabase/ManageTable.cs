using InterSystems.Data.IRISClient;
using InterSystems.Data.IRISClient.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nt.Utility.IRISDatabase
{
    public class ManageTable
    {
        private readonly IRIS iris;
        private readonly IRISDBConnect irisConnection;

        public ManageTable()
        {
            // Instanziiere und überprüfe die IRIS-Verbindung
            var irisConnection = IRISDBConnect.Instance;

            if (!irisConnection.CheckConnection())
            {
                throw new InvalidOperationException("Keine IRIS-Verbindung vorhanden.");
            }

            // Setze die Iris-Instanz
            iris = irisConnection.iris ?? throw new InvalidOperationException("IRIS-Instanz ist null.");
        }

        public void Kill(string FA, string TI)
        {
            try
            {
                iris.TStart();
                iris.Kill($"^KASSA({FA},7.01,{TI})");
                iris.Kill($"^KASSA({FA},10,{TI})");
                iris.Kill($"^KASSA({FA},10.3,{TI})");
                iris.Kill($"^KASSA({FA},10.5,{TI})");
                iris.Kill($"^KASSA({FA},10.6,{TI})");
                iris.Kill($"^KASSA({FA},11,{TI})");
                iris.TCommit();



                //MessageBox.Show($"Tisch {TI} erfolgreich gelöscht");
                //mainWindow.LogEvent($"NOVACOM: {TI} gelöscht");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Fehler beim Löschen des Tischs {TI}: {ex.Message}");
            }
        }
    }


}

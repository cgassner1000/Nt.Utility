using InterSystems.Data.IRISClient.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using InterSystems.Data.IRISClient;
using static Nt.Utility.MainWindow;

namespace Nt.Utility
{
    internal class Table
    {
        private Database database;

        public Table(Database db)
        {
            this.database = db;
        }
        public void Unlock(string tisch)
        {
            // Implementiere die Methode zum Entfernen des Locks von einem Tisch
            MessageBox.Show($"Lock von Tisch {tisch} entfernt.");
        }

        public List<TableData> GetOpenTables(string FA)
        {
            var tableList = new List<TableData>();

            string query = "SELECT b.TISCH, b.VKO, b.tianz, b.offen, b.offenvormerk, b.bez, b.pers, b.zinr, b.copa, b.buchnr, b.status, b.statusfarbe, b.pnr, MIN(a.tstmp) as timstampMIN, MAX(a.tstmp) as timstampMAX " +
                           "FROM NTQRY.QryOffenBon as a " +
                           "RIGHT JOIN NTQRY.QryOffeneTische as b on (b.FA=@FA AND a.TISCH=b.TISCH) " +
                           "WHERE b.TISCH IS NOT NULL " +
                           "GROUP BY b.TISCH";

            try
            {
                using (IRISCommand cmd = new IRISCommand(query, database.conn))
                {
                    cmd.Parameters.AddWithValue("@FA", FA);

                    using (IRISDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableData = new TableData
                            {
                                TISCH = reader["TISCH"].ToString()
                                // Füge hier die anderen Eigenschaften hinzu
                            };
                            tableList.Add(tableData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Abrufen der offenen Tische: {ex.Message}");
            }

            return tableList;
        }
        public void Merge(string von_TISCH, string auf_TISCH, string FA, string KASSA, string KEY)
        {
            try
            {
                database.iris.ClassMethodVoid("cmNT.SplittOman", "SetUmbelegung", FA, KASSA, KEY, von_TISCH, auf_TISCH);
                database.iris.ClassMethodVoid("cmNT.Tisch", "TischUnlock", FA, KASSA, KEY, auf_TISCH);
                Console.WriteLine("Classmethod TischUmbelegenStat erfolgreich aufgerufen.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Kopieren der Subglobals: {ex.Message}");
            }
        }

        public void Kill(string FA, string TI)
        {
            try
            {
                database.iris.TStart();
                database.iris.Kill($"^KASSA({FA},7.01,{TI})");
                database.iris.Kill($"^KASSA({FA},10,{TI})");
                database.iris.Kill($"^KASSA({FA},10.3,{TI})");
                database.iris.Kill($"^KASSA({FA},10.5,{TI})");
                database.iris.Kill($"^KASSA({FA},10.6,{TI})");
                database.iris.Kill($"^KASSA({FA},11,{TI})");
                database.iris.TCommit();

                MessageBox.Show($"Tisch {TI} erfolgreich gelöscht");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Löschen des Tischs {TI}: {ex.Message}");
            }
        }
    }
}

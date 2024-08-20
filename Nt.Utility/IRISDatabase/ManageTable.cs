using InterSystems.Data.IRISClient;
using InterSystems.Data.IRISClient.ADO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;


namespace Nt.Utility.IRISDatabase
{
    public class ManageTable
    {
        private readonly IRIS iris;
        private readonly IRISDBConnect irisConnection;

        public ManageTable()
        {
            // Instanziiere und überprüfe die IRIS-Verbindung
            irisConnection = IRISDBConnect.Instance;

            if (!irisConnection.CheckConnection())
            {
                throw new InvalidOperationException("Keine IRIS-Verbindung vorhanden.");
            }

            // Setze die Iris-Instanz
            iris = irisConnection.iris ?? throw new InvalidOperationException("IRIS-Instanz ist null.");
        }

        public void UpdateOpenTables(Canvas tableCanvas, string FA)
        {
            var tables = GetOpenTables(FA);
            DrawTables(tableCanvas, tables);
        }

        public List<TableData> GetOpenTables(string FA)
        {
            Debug.WriteLine("GetOpenTables");

            if (!irisConnection.CheckConnection())
            {
                throw new InvalidOperationException("Keine IRIS-Verbindung vorhanden.");
            }

            var tableList = new List<TableData>();

            string query = "SELECT b.TISCH, b.VKO, b.tianz, b.offen, b.offenvormerk, b.bez, b.pers, b.zinr, b.copa, b.buchnr, b.status, b.statusfarbe, b.pnr, MIN(a.tstmp) as timstampMIN, MAX(a.tstmp) as timstampMAX " +
                           "FROM NTQRY.QryOffenBon as a " +
                           "RIGHT JOIN NTQRY.QryOffeneTische as b on (b.FA=@FA AND a.TISCH=b.TISCH) " +
                           "WHERE b.TISCH IS NOT NULL " +
                           "GROUP BY b.TISCH";

            try
            {
                using (IRISCommand cmd = new IRISCommand(query, irisConnection.conn))
                {
                    cmd.Parameters.Add(new IRISParameter("@FA", IRISDbType.NVarChar) { Value = FA });

                    using (IRISDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableData = new TableData
                            {
                                TISCH = reader["TISCH"].ToString(),
                                VKO = reader["VKO"].ToString(),
                                offen = reader["offen"].ToString(),
                                KEY = "K" + reader["PNR"].ToString(),
                                Status = reader["Status"].ToString()
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

        public void PopulateTableListBox(ListBox listBox, string FA)
        {
            var openTables = GetOpenTables(FA);

            listBox.Items.Clear();
            foreach (var table in openTables)
            {
                listBox.Items.Add(table);
            }
        }

        public class TableData
        {
            public string TISCH { get; set; }
            public string VKO { get; set; }
            public string KEY { get; set; }
            public string offen {  get; set; }
            public string Status { get; set; }
            
            // Füge hier weitere Eigenschaften hinzu

            public override string ToString()
            {
                return $"{TISCH} ({KEY})"; // Darstellung in der ListBox
            }


        }

        public void DrawTables(Canvas tableCanvas, List<TableData> tables)
        {
            //var data = IRISDBData.Instance;

            //tableCanvas.Children.Clear();

            //double x = 10; // Anfangs-X-Position für das erste Rechteck
            //double y = 10; // Anfangs-Y-Position für das erste Rechteck
            //double width = 100; // Breite der Rechtecke
            //double height = 45; // Höhe der Rechtecke
            //double margin = 3; // Abstand zwischen den Rechtecken

            //double canvasWidth = tableCanvas.ActualWidth;

            //foreach (var table in tables)

            var data = IRISDBData.Instance;

            tableCanvas.Children.Clear();

            // Gruppiere die Tische nach VKO
            var groupedTables = tables.GroupBy(t => t.VKO).OrderBy(g => g.Key);

            double x = 10; // Anfangs-X-Position für das erste Rechteck
            double y = 10; // Anfangs-Y-Position für das erste Rechteck
            double width = 100; // Breite der Rechtecke
            double height = 45; // Höhe der Rechtecke
            double margin = 3; // Abstand zwischen den Rechtecken
            double headerHeight = 20; // Höhe der Überschrift
            double headerMargin = 4; // Abstand zwischen Überschrift und Tischen
            double groupMargin = 20; // Abstand zwischen den Tischgruppen

            double canvasWidth = tableCanvas.ActualWidth;

            foreach (var group in groupedTables)
            {
                string vkobez = iris.ClassMethodString("cmWW.VKO", "GetVKOBez", data.FA, group.Key);
                // Überschrift für den Verkaufsort erstellen
                var headerTextBlock = new TextBlock
                {
                    Text = $"{vkobez}:",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 10)
                };

                // Fügen Sie die Überschrift zum Canvas hinzu
                tableCanvas.Children.Add(headerTextBlock);
                Canvas.SetLeft(headerTextBlock, x);
                Canvas.SetTop(headerTextBlock, y);

                // Aktualisieren der Y-Position für die Tische unter der Überschrift
                y += headerHeight + headerMargin;

                foreach (var table in group)
                {
                    string ti = iris.ClassMethodString("cmNT.Tisch", "GetTischDisplayFormat", data.FA, table.TISCH, 0);
                    // Bestimme die Hintergrundfarbe basierend auf dem Status
                    Brush backgroundColor = table.Status switch
                    {
                        "1" => new SolidColorBrush(Colors.DarkSeaGreen),
                        "2" => new SolidColorBrush(Colors.Gold),
                        "3" => new SolidColorBrush(Colors.Tomato),
                        _ => new SolidColorBrush(Colors.LightGray) // Default-Farbe
                    };

                    // StackPanel für Rechteck und Textblöcke erstellen
                    var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = width,
                        Height = height,
                        Background = backgroundColor // Hintergrundfarbe des StackPanels setzen
                    };

                    // Rechteck erstellen und Hintergrundfarbe setzen
                    var rect = new Rectangle
                    {
                        Width = width,
                        Height = height,
                        Fill = backgroundColor,
                        Stroke = new SolidColorBrush(Colors.Black),
                        StrokeThickness = 1
                    };

                    // Textblock für Tischnummer erstellen
                    var textBlock1 = new TextBlock
                    {
                        // OLD: Text = $"{table.TISCH}",
                        Text = $"T{table.TISCH} | {table.KEY}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 0),
                        Foreground = new SolidColorBrush(Colors.Black) // Textfarbe setzen
                    };

                    // Textblock für Kellnernummer erstellen
                    var textBlock2 = new TextBlock
                    {
                        Text = table.offen,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Colors.Black) // Textfarbe setzen
                    };

                    // Container für die TextBlöcke erstellen, um sie vertikal zentriert zu halten
                    var innerStackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    innerStackPanel.Children.Add(textBlock1);
                    innerStackPanel.Children.Add(textBlock2);

                    // Inneres StackPanel zum äußeren StackPanel hinzufügen
                    stackPanel.Children.Add(innerStackPanel);

                    // Schatteneffekt hinzufügen
                    var dropShadowEffect = new DropShadowEffect
                    {
                        Color = Colors.Black,
                        Direction = 320,
                        ShadowDepth = 5,
                        Opacity = 0.5,
                        BlurRadius = 10
                    };
                    stackPanel.Effect = dropShadowEffect;



                    // Kontextmenü erstellen
                    var contextMenu = new ContextMenu();

                    // Menüeintrag "Umbelegen" erstellen
                    var umbelegenMenuItem = new MenuItem { Header = "Umbelegen" };
                    umbelegenMenuItem.Click += (s, e) => TableMerge(table.TISCH, "", data.FA, data.KASSA, data.KEY, table.VKO);
                    Debug.WriteLine($"FA {data.FA} | KASSA: {data.KASSA} | KEY: {data.KEY} | T{table.TISCH}");

                    // Menüeintrag "Tisch löschen" erstellen
                    var tischLoeschenMenuItem = new MenuItem { Header = "Tisch löschen" };
                    tischLoeschenMenuItem.Click += (s, e) => Kill(data.FA, table.TISCH);

                    // Menüeinträge zum Kontextmenü hinzufügen
                    contextMenu.Items.Add(umbelegenMenuItem);
                    contextMenu.Items.Add(tischLoeschenMenuItem);

                    // Kontextmenü dem StackPanel hinzufügen
                    stackPanel.ContextMenu = contextMenu;

                    // StackPanel zu Canvas hinzufügen
                    tableCanvas.Children.Add(stackPanel);

                    // Position für das StackPanel aktualisieren
                    Canvas.SetLeft(stackPanel, x);
                    Canvas.SetTop(stackPanel, y);

                    // Berechnen der nächsten Position
                    x += width + margin;
                    if (x + width > canvasWidth) // Zeilenumbruch
                    {
                        x = 10;
                        y += height + margin;
                    }
                }

                // Zeilenumbruch für den nächsten Verkaufsort
                x = 10;
                y += height + groupMargin;
            }
        }





        //public void DrawTables(Canvas tableCanvas, List<TableData> tables)
        //{
        //    var data = IRISDBData.Instance;

        //    tableCanvas.Children.Clear();

        //    double x = 10; // Anfangs-X-Position für das erste Rechteck
        //    double y = 10; // Anfangs-Y-Position für das erste Rechteck
        //    double width = 100; // Breite der Rechtecke
        //    double height = 50; // Höhe der Rechtecke
        //    double margin = 3; // Abstand zwischen den Rechtecken

        //    double canvasWidth = tableCanvas.ActualWidth;

        //    foreach (var table in tables)
        //    {
        //        // Bestimme die Hintergrundfarbe basierend auf dem Status
        //        Brush backgroundColor = table.Status switch
        //        {
        //            "1" => new SolidColorBrush(Colors.DarkSeaGreen),
        //            "2" => new SolidColorBrush(Colors.Gold),
        //            "3" => new SolidColorBrush(Colors.Tomato),
        //            _ => new SolidColorBrush(Colors.LightGray) // Default-Farbe
        //        };

        //        // Rechteck erstellen und Hintergrundfarbe setzen
        //        var rect = new Rectangle
        //        {
        //            Width = width,
        //            Height = height,
        //            Fill = backgroundColor,
        //            Stroke = new SolidColorBrush(Colors.Black),
        //            StrokeThickness = 1
        //        };

        //        // Untermenü BEGIN
        //        // Kontextmenü erstellen
        //        var contextMenu = new ContextMenu();

        //        // Menüeintrag "Umbelegen" erstellen
        //        var umbelegenMenuItem = new MenuItem { Header = "Umbelegen" };
        //        umbelegenMenuItem.Click += (s, e) => TableMerge(table.TISCH,"null","1001","RK99", table.KEY);

        //        // Menüeintrag "Tisch löschen" erstellen
        //        var tischLoeschenMenuItem = new MenuItem { Header = "Tisch löschen" };
        //        tischLoeschenMenuItem.Click += (s, e) => Kill(data.FA,table.TISCH);

        //        // Menüeinträge zum Kontextmenü hinzufügen
        //        contextMenu.Items.Add(umbelegenMenuItem);
        //        contextMenu.Items.Add(tischLoeschenMenuItem);

        //        // Kontextmenü dem Rechteck hinzufügen
        //        rect.ContextMenu = contextMenu;


        //        // Untermenü ENDE

        //        // Rechteck zu Canvas hinzufügen
        //        Canvas.SetLeft(rect, x);
        //        Canvas.SetTop(rect, y);
        //        tableCanvas.Children.Add(rect);

        //        // Textblock für Tischnummer erstellen
        //        var textBlock1 = new TextBlock
        //        {
        //            Text = $"{table.TISCH}",
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center,
        //            Margin = new Thickness(0, 5, 0, 0)
        //        };

        //        // Textblock für Kellnernummer erstellen
        //        var textBlock2 = new TextBlock
        //        {
        //            Text = table.KEY,
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center
        //        };

        //        // StackPanel für Textblöcke erstellen
        //        var stackPanel = new StackPanel
        //        {
        //            Orientation = Orientation.Vertical,
        //            HorizontalAlignment = HorizontalAlignment.Center,
        //            VerticalAlignment = VerticalAlignment.Center,
        //            Width = width // Breite des StackPanels an die Breite des Rechtecks anpassen
        //        };
        //        stackPanel.Children.Add(textBlock1);
        //        stackPanel.Children.Add(textBlock2);

        //        // StackPanel zu Canvas hinzufügen
        //        tableCanvas.Children.Add(stackPanel);

        //        // Position für das StackPanel aktualisieren
        //        Canvas.SetLeft(stackPanel, x);
        //        Canvas.SetTop(stackPanel, y);

        //        // Berechnen der nächsten Position
        //        x += width + margin;
        //        if (x + width > canvasWidth) // Zeilenumbruch
        //        {
        //            x = 10;
        //            y += height + margin;
        //        }
        //    }
        //}


        // KILL TABLE

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



                MessageBox.Show($"Tisch {TI} erfolgreich gelöscht");
                //mainWindow.LogEvent($"NOVACOM: {TI} gelöscht");
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Fehler beim Löschen des Tischs {TI}: {ex.Message}");
            }
        }

        public void TableMerge(string von_TISCH, string auf_TISCH, string FA, string KASSA, string KEY, string VKO)
        {
            string IRIS_Timestamp = IRISDBConnect.GetIRISTimestamp();

            string to_TABLE = to_TABLE_inputBOX.Show("Bitte geben Sie den Ziel-Tisch ein:", "Tisch Umbelegen");

            try
            {
                iris.ClassMethodVoid("cmNT.SplittOman", "SetUmbelegung", FA, KASSA, KEY, von_TISCH, to_TABLE);
                Debug.WriteLine($"{FA} | {KASSA} | {KEY} | {von_TISCH} | {to_TABLE}");
                iris.Set($"{KEY}`{IRIS_Timestamp}``{VKO}", "^KASSA", $"{FA}", "7", $"{to_TABLE}"); //zweites KEY muss VKO sein!
                if (von_TISCH != to_TABLE)
                {
                    iris.Set("", "^KASSA", $"{FA}", "7", $"{von_TISCH}");
                }

                iris.ClassMethodVoid("cmNT.Tisch", "TischUnlock", FA, KASSA, KEY, to_TABLE);

                Debug.WriteLine("Classmethod TischUmbelegenStat erfolgreich aufgerufen.");
                MessageBox.Show($"Tisch von {von_TISCH} erfolgreich auf Tisch {to_TABLE} umbelegt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Kopieren der Subglobals: {ex.Message}");
            }
        }

        public static class to_TABLE_inputBOX
        {
            public static string Show(string prompt, string title)
            {
                Window window = new Window
                {
                    Title = title,
                    Width = 300,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                StackPanel stackPanel = new StackPanel();

                TextBlock textBlock = new TextBlock
                {
                    Text = prompt,
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(textBlock);

                TextBox textBox = new TextBox
                {
                    Margin = new Thickness(10)
                };
                stackPanel.Children.Add(textBox);

                Button buttonOk = new Button
                {
                    Content = "OK",
                    Width = 60,
                    Height = 25,
                    Margin = new Thickness(10),
                    IsDefault = true,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                buttonOk.Click += (sender, e) => { window.DialogResult = true; window.Close(); };
                stackPanel.Children.Add(buttonOk);

                window.Content = stackPanel;
                window.ShowDialog();

                return textBox.Text;
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Nt.Utility.Pages.Novatouch
{
    public partial class LogSelection : Page
    {
        public event EventHandler<List<string>> LogFilesSelected;

        public LogSelection(List<string> logFiles)
        {
            InitializeComponent();
            LogFilesList.ItemsSource = logFiles.Select(file => new LogFileItem { FilePath = file }).ToList();
        }

        public List<string> GetSelectedFiles()
        {
            return LogFilesList.Items.Cast<LogFileItem>()
                                     .Where(item => item.IsSelected)
                                     .Select(item => item.FilePath)
                                     .ToList();
        }

        private void ZipButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedFiles = GetSelectedFiles();
            LogFilesSelected?.Invoke(this, selectedFiles);
        }
    }

    public class LogFileItem
    {
        public string FilePath { get; set; }
        public bool IsSelected { get; set; }
        public string DisplayName { get; set; }
    }
}

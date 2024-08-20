using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;

namespace Nt.Utility.Tools
{
    public class Log
    {
        public List<string> FindLogFiles(List<string> directories)
        {
            List<string> logFiles = new List<string>();

            foreach (var dir in directories)
            {
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir, "*.log", SearchOption.AllDirectories);
                    logFiles.AddRange(files);
                }
            }

            return logFiles;
        }

        public void ShowLogSelectionPage(List<string> logFiles, Frame navigationFrame)
        {
            var logSelectionPage = new Nt.Utility.Pages.Novatouch.LogSelection(logFiles);
            logSelectionPage.LogFilesSelected += (sender, selectedFiles) =>
            {
                ZipSelectedFiles(selectedFiles, @"C:\Path\To\Save\Logs.zip");
                navigationFrame.GoBack();
            };
            navigationFrame.Navigate(logSelectionPage);
        }

        public void ZipSelectedFiles(List<string> files, string zipPath)
        {
            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (var file in files)
                {
                    zip.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            MessageBox.Show("Selected files have been zipped successfully.");
        }
    }
}

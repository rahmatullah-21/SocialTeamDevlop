using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DominatorHouseCore.Utility
{
    public class FileUtilities
    {

        /// <summary>
        /// FileBrowseAndReader() is used to browse and read the file data from OpenFileDialog
        /// </summary>
        /// <returns>Returns unique list of item from all files</returns>
        public static List<string> FileBrowseAndReader()
        {
            var fileData = new List<string>();

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text documents (.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            var openFileDialogResult = openFileDialog.ShowDialog();

            if (openFileDialogResult != true) return new List<string>();

            foreach (var fileName in openFileDialog.FileNames)
            {
                try
                {
                    var extension = Path.GetExtension(fileName);

                    if (extension != null && (!extension.Contains(".txt") && !extension.Contains(".csv"))) continue;

                     fileData.AddRange(GetFileContent(fileName));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return fileData;
        }



        /// <summary>
        /// Read the file data from specified files
        /// </summary>
        /// <param name="fileName">given input file</param>
        /// <returns>Unique file details</returns>
        public static List<string> GetFileContent(string fileName)
        {
            const int bufferSize = 16384;

            var listFileContent = new List<string>();

            var stringBuilder = new StringBuilder();

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        var fileContents = new char[bufferSize];
                        var charsRead = streamReader.Read(fileContents, 0, bufferSize);

                        if (charsRead == 0)
                            throw new Exception("File is 0 bytes");

                        while (charsRead > 0)
                        {
                            stringBuilder.Append(fileContents);
                            charsRead = streamReader.Read(fileContents, 0, bufferSize);
                        }

                        var contentArray = stringBuilder.ToString().Split('\r', '\n');

                        var data = contentArray.Select(line => line.EndsWith("\0") ? line.Replace("\0", "") : line);
                  
                        listFileContent.AddRange(data.Distinct(StringComparer.CurrentCultureIgnoreCase));

                        listFileContent.RemoveAll(string.IsNullOrEmpty);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return listFileContent.Distinct().ToList();

        }




        /// <summary>
        /// GetExportPath is used to get the selected path
        /// </summary>
        /// <returns></returns>
        public static string GetExportPath()
        {

            var exportPath = string.Empty;

            var openBrowserDialog = new WPFFolderBrowser.WPFFolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            
            var result = openBrowserDialog.ShowDialog(Application.Current.MainWindow);

            if (result == true)
            {
                exportPath = openBrowserDialog.FileName;
            }

            return exportPath;
        }

        public static string GetExportPath(Window OwnerWindow)
        {

            var exportPath = string.Empty;

            var openBrowserDialog = new WPFFolderBrowser.WPFFolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var result = openBrowserDialog.ShowDialog(OwnerWindow);

            if (result == true)
            {
                exportPath = openBrowserDialog.FileName;
            }

            return exportPath;
        }
    }
}

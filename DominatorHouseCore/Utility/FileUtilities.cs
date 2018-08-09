using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using DominatorHouseCore.LogHelper;
using Microsoft.Win32;
using WPFFolderBrowser;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.VisualBasic.FileIO;

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
                    if (!string.IsNullOrEmpty(extension))
                    {
                        if (extension.Contains(".xls") || extension.Contains(".xlsx"))
                            fileData.AddRange(GetExcelFileContent(fileName));
                        else if (extension.Contains(".csv"))
                            fileData.AddRange(GetCsvFileContent(fileName));
                        else if (extension.Contains(".txt"))
                            fileData.AddRange(GetTextFileContent(fileName));
                        else continue;

                        //if (!extension.Contains(".txt") && !extension.Contains(".csv"))
                        //        continue;

                        //fileData.AddRange(GetFileContent(fileName));

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return fileData;
        }


        /// <summary>
        /// FileBrowseAndReader() is used to browse and read the file data from OpenFileDialog
        /// </summary>
        /// <returns>Returns unique list of item from all files</returns>
        public static async Task<List<string>> FileBrowseAndReaderAsync()
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
                    if (!string.IsNullOrEmpty(extension))
                    {
                        if (extension.Contains(".xls") || extension.Contains(".xlsx"))
                        {
                            var value = await Task.Factory.StartNew(() => GetExcelFileContent(fileName));
                            fileData.AddRange(value);
                        }
                        else if (extension.Contains(".csv"))
                            fileData.AddRange(GetCsvFileContent(fileName));
                        else if (extension.Contains(".txt"))
                            fileData.AddRange(GetTextFileContent(fileName));
                        else continue;

                        //if (!extension.Contains(".txt") && !extension.Contains(".csv"))
                        //        continue;

                        //fileData.AddRange(GetFileContent(fileName));

                    }
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

        public static void AddHeaderToCsv(string filename, string header)
        {
            using (var streamWriter = new StreamWriter(filename, false))
            {
                streamWriter.WriteLine(header);
            }
        }

        public static dynamic GetImageOrVideo(bool multiselect, string filter)
        {

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = multiselect,
                Filter = filter
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return null;
            if (multiselect)
                return openFileDialog.FileNames.ToList();
            else
                return openFileDialog.FileNames[0];
        }

        public static List<string> GetExcelFileContent(string fileName)
        {
            Excel.Application excel = new Excel.Application();
            Excel.Workbook workBook = excel.Workbooks.Open(fileName, 0, true, 5, "", "", true,
                Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            Excel.Worksheet workSheet = (Excel.Worksheet)workBook.Worksheets.Item[1];
            Excel.Range range = workSheet.UsedRange;

            List<string> content = new List<string>();
            try
            {
                for (int row = 1; row <= range.Rows.Count; row++)
                {
                    string rowContent = String.Empty;
                    for (int colum = 1; colum <= range.Columns.Count; colum++)
                    {
                        var val = Convert.ToString(range.Cells[row, colum].Value2);
                        rowContent += (String.IsNullOrEmpty(val) ? String.Empty : val.ToString()) + "\t";
                    }
                    var data = rowContent.Trim();
                    if (!string.IsNullOrEmpty(data))
                    {
                        var substring = rowContent.Substring(0, rowContent.Length - 1);
                        content.Add(substring);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                workBook.Close(true);
                excel.Quit();
            }

            return content;
        }

        public static List<string> GetCsvFileContent(string fileName)
        {
            using (TextReader reader = File.OpenText(fileName))
            {
                try
                {
                    var csv = new CsvReader(reader);
                    List<string> csvSplitList = new List<string>();

                    while (csv.Read())
                    {
                        string rowContent = String.Empty;
                        int columnCount = 0;
                        bool hasColumn = true;
                        string columnValue = String.Empty;

                        while (hasColumn)
                        {
                            hasColumn = csv.TryGetField(columnCount, out columnValue);

                            if (hasColumn)
                            {
                                columnCount += 1;
                                rowContent += columnValue + "\t";
                            }
                        }

                        var data = rowContent.Trim();
                        if (!string.IsNullOrEmpty(data))
                            csvSplitList.Add(rowContent.Substring(0, rowContent.Length - 1));
                    }
                    return csvSplitList;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return new List<string>();
                }
            }
        }

        public static List<string> GetTextFileContent(string fileName)
        {
            using (StreamReader file = new StreamReader(fileName))
            {
                List<string> csvSplitList = new List<string>();
                var line = String.Empty;
                while ((line = file.ReadLine()) != null)
                {
                    csvSplitList.Add(ImageExtracter.CheckUrlValid(line) ? line : line.Replace(":", "\t"));

                }
                return csvSplitList;
            }
        }

    }
}

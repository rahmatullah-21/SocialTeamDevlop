using System;
using DominatorHouseCore.LogHelper;
using System.IO;
using System.IO.Compression;

namespace DominatorHouseCore.Utility
{
   public class DirectoryUtilities
    {
        public static void CreateDirectory(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch (IOException ex)
            {
                GlobusLogHelper.log.Error($"Unable to create directory {folder} - {ex.Message}");                
                throw;        
            }
        }

        public static void Compress()
        {
            try
            {
                var extractPath =
                    $"{ConstantVariable.GetPlatformTodayBackupDirectory()}\\{ConstantVariable.GetDate()}.zip";

                DeleteOldBackupFile();

                if (!File.Exists(extractPath))
                    ZipFile.CreateFromDirectory(ConstantVariable.GetPlatformBaseDirectory(), extractPath);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void DeleteOldBackupFile()
        {
            try
            {
                var directoryPath = ConstantVariable.GetPlatformTodayBackupDirectory();

                if (!Directory.Exists(directoryPath))
                    return;

                var directoryFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                directoryFiles.ForEach(file =>
                {
                    var fileInfo = new FileInfo(file);
                    var daysCount = DateTime.Today - fileInfo.CreationTime.Date;
                    if (daysCount.Days > 7)
                        File.Delete(file);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public static void DeleteOldLogsFile()
        {
            try
            {
                var directoryPath = ConstantVariable.GetPlatformLogDirectory();

                if (!Directory.Exists(directoryPath))
                    return;

                var directoryFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                directoryFiles.ForEach(file =>
                {
                    var fileInfo = new FileInfo(file);
                    var daysCount = DateTime.Today - fileInfo.CreationTime.Date;
                    if (daysCount.Days > 7)
                        File.Delete(file);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}

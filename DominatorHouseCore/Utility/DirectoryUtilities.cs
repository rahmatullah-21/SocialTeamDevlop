using DominatorHouseCore.LogHelper;
using System;
using System.IO;

namespace DominatorHouseCore.Utility
{
   public  class DirectoryUtilities
    {

        internal static void CreateDirectory(string folder)
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

    }
}

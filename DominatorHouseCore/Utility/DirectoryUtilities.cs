using DominatorHouseCore.LogHelper;
using System.IO;

namespace DominatorHouseCore.Utility
{
   public  class DirectoryUtilities
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

    }
}

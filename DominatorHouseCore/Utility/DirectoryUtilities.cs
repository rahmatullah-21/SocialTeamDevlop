using System;
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
            catch (Exception ex)
            {

            }
        }






    }
}

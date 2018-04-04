using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    public class EmbeddedBrowserSettingsFileManager
    {
        public static bool SaveEmbeddedBrowserSettings<T>(T embeddedBrowserSettings) where T : class
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherEmbeddedBrowserSettingsFile()))
                {
                    Serializer.Serialize(stream, embeddedBrowserSettings);
                    GlobusLogHelper.log.Debug($"Setting successfully saved");
                    return true;
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update Embedded Browser Settings error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
            
        }
        public static EmbeddedBrowserSettingsModel GetEmbeddedBrowserSettings()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherEmbeddedBrowserSettingsFile()))
                {
                    return Serializer.Deserialize<EmbeddedBrowserSettingsModel>(stream);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
            return null;
        }
    }
}

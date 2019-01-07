using System;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using CommonServiceLocator;

namespace DominatorHouseCore.FileManagers
{
    public class EmbeddedBrowserSettingsFileManager
    {
        public static bool SaveEmbeddedBrowserSettings<T>(T embeddedBrowserSettings) where T : class
        {
            try
            {
                using (var stream = File.Create(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherEmbeddedBrowserSettingsFile()))
                {
                    Serializer.Serialize(stream, embeddedBrowserSettings);
                    GlobusLogHelper.log.Debug("Setting successfully saved");
                    return true;
                }

            }
            catch (Exception ex)
            {
                
                ex.DebugLog();
                return false;
            }
            
        }
        public static EmbeddedBrowserSettingsModel GetEmbeddedBrowserSettings()
        {
            try
            {
                using (var stream = File.OpenRead(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherEmbeddedBrowserSettingsFile()))
                {
                    return Serializer.Deserialize<EmbeddedBrowserSettingsModel>(stream);
                }
            }
            catch (Exception ex)
            {
                 ex.DebugLog();
            }
            return null;
        }
    }
}

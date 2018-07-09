using System;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    public class SoftwareSettingsFileManager
    {
        public static bool SaveSoftwareSettings(SoftwareSettingsModel softwareSetting)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherSoftwareSettingsFile()))
                {
                    Serializer.Serialize(stream, softwareSetting);
                    SoftwareSettings.Settings = softwareSetting;
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update software settings error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
           
        }
        public static SoftwareSettingsModel GetSoftwareSettings()
        {
            try
            {
               using (var stream = File.OpenRead(ConstantVariable.GetOtherSoftwareSettingsFile()))
                {
                    return Serializer.Deserialize<SoftwareSettingsModel>(stream);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug(ex.Message);
            }
            return null;
        }
    }
}

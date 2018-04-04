using System;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    class SoftwareSettingsFileManager
    {
        public static bool SaveSoftwareSettings<T>(T softwareSetting) where T : class
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherSoftwareSettingsFile()))
                {
                    Serializer.Serialize(stream, softwareSetting);
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
                GlobusLogHelper.log.Error(ex.Message);
            }
            return null;
        }
    }
}

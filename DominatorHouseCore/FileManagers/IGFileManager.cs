using System;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    class IGFileManager
    {
        public static bool SaveInstagramConfig(InstagramModel InstagramModel)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherInstagramSettingsFile()))
                {
                    Serializer.Serialize(stream, InstagramModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update Instagram error - " + ex.Message);
                ex.DebugLog();
                return false;
            }

        }
        public static InstagramModel GetInstagramConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherInstagramSettingsFile()))
                {
                    return Serializer.Deserialize<InstagramModel>(stream);
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
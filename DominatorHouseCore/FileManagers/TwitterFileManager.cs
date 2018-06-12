using System;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    class TwitterFileManager
    {
        public static bool SaveTwitterConfig(TwitterModel twitterModel)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherDir() + @"\Twitter.bin"))
                {
                    Serializer.Serialize(stream, twitterModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update twitter configuration error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }

        public static TwitterModel GetTwitterConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherDir() + @"\Twitter.bin"))
                {
                    return Serializer.Deserialize<TwitterModel>(stream);
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

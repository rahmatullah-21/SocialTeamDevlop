using System;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    class TumblrFileManager
    {
        internal static TumblrModel GetTumblrConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherDir() + @"\Tumblr.bin"))
                {
                    return Serializer.Deserialize<TumblrModel>(stream);
                }
            }
            catch (Exception ex)
            {
                 ex.DebugLog();
            }
            return null;
        }

        internal static bool SaveTumblrConfig(TumblrModel tumblrModel)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherDir() + @"\Tumblr.bin"))
                {
                    Serializer.Serialize(stream, tumblrModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
               
                ex.DebugLog();
                return false;
            }
        }
    }
}

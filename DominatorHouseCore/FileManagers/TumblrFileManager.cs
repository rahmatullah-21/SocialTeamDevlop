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
                GlobusLogHelper.log.Error(ex.Message);
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
                GlobusLogHelper.log.Error("Update tumblr configuration error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }
    }
}

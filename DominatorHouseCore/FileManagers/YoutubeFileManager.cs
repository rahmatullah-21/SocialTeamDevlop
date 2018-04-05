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
    class YoutubeFileManager
    {
        internal static YoutubeModel GetYoutubeConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherDir() + @"\Youtube.bin"))
                {
                    return Serializer.Deserialize<YoutubeModel>(stream);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
            return null;
        }

        public static bool SaveYoutubeConfig(YoutubeModel youtubeModel)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherDir() + @"\Youtube.bin"))
                {
                    Serializer.Serialize(stream, youtubeModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update youtube configuration error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }
    }
}

using System;
using System.IO;
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
                 ex.DebugLog();
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
               
                ex.DebugLog();
                return false;
            }
        }
    }
}

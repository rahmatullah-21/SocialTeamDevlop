using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using CommonServiceLocator;

namespace DominatorHouseCore.FileManagers
{
    class YoutubeFileManager
    {
        internal static YoutubeModel GetYoutubeConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherDir() + @"\Youtube.bin"))
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
                using (var stream = File.Create(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherDir() + @"\Youtube.bin"))
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

using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using CommonServiceLocator;

namespace DominatorHouseCore.FileManagers
{
    class TwitterFileManager
    {
        public static bool SaveTwitterConfig(TwitterModel twitterModel)
        {
            try
            {
                using (var stream = File.Create(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherDir() + @"\Twitter.bin"))
                {
                    Serializer.Serialize(stream, twitterModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
               
                ex.DebugLog();
                return false;
            }
        }

        public static TwitterModel GetTwitterConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherDir() + @"\Twitter.bin"))
                {
                    return Serializer.Deserialize<TwitterModel>(stream);
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

using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using CommonServiceLocator;

namespace DominatorHouseCore.FileManagers
{
    class IGFileManager
    {
        public static bool SaveInstagramConfig(InstagramModel InstagramModel)
        {
            try
            {
                using (var stream = File.Create(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherInstagramSettingsFile()))
                {
                    Serializer.Serialize(stream, InstagramModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
               
                ex.DebugLog();
                return false;
            }

        }
        public static InstagramModel GetInstagramConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ServiceLocator.Current.GetInstance<IConstantVariable>().GetOtherInstagramSettingsFile()))
                {
                    return Serializer.Deserialize<InstagramModel>(stream);
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
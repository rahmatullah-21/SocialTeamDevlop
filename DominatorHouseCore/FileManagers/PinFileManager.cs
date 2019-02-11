using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    class PinFileManager
    {
        internal static PinterestModel GetPinterestConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherDir() + @"\Pinterest.bin"))
                {
                    return Serializer.Deserialize<PinterestModel>(stream);
                }
            }
            catch (Exception ex)
            {
                 ex.DebugLog();
            }
            return null;
        }

        internal static bool SavePinterestConfig(PinterestModel pinterestModel)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherDir() + @"\Pinterest.bin"))
                {
                    Serializer.Serialize(stream, pinterestModel);
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

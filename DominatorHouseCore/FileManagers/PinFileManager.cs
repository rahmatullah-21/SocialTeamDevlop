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
                GlobusLogHelper.log.Error(ex.Message);
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
                GlobusLogHelper.log.Error("Update Pinterest configuration error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }
    }
}

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
    class FBFileManager
    {
        public static bool SaveFacebookConfig(FacebookModel facebookModel)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetOtherFacebookSettingsFile()))
                {
                    Serializer.Serialize(stream, facebookModel);
                    return true;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update Facebook error - " + ex.Message);
                ex.DebugLog();
                return false;
            }

        }
        public static FacebookModel GetFacebookConfig()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetOtherFacebookSettingsFile()))
                {
                    return Serializer.Deserialize<FacebookModel>(stream);
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

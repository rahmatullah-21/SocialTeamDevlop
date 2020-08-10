using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.IO;

namespace DominatorHouseCore.FileManagers
{
    public class SocinatorKeyHelper
    {
        public static FatalErrorHandler Key;
        public static bool SaveKey(FatalErrorHandler keyDetails)
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetConfigurationKey()))
                {
                    Key = keyDetails;
                    Serializer.Serialize(stream, keyDetails);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        public static void InitilizeKey()
        {
            try
            {
                var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                Key= genericFileManager.GetModel<FatalErrorHandler>(ConstantVariable.GetConfigurationKey());
                Key.FatalErrorMessage = "SOC-YZBYVGND1UY1MJT8PYMHRHH6V";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
          
        }

    }
}
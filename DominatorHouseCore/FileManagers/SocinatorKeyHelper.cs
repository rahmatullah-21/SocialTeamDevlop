#region

using System;
using System.IO;
using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

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
                Key = genericFileManager.GetModel<FatalErrorHandler>(ConstantVariable.GetConfigurationKey());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
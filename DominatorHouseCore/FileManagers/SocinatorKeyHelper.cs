using System;
using System.IO;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.FileManagers
{
    public class SocinatorKeyHelper
    {
        public static bool SaveKey<T>(T keyDetails) where T : class
        {
            try
            {
                using (var stream = File.Create(ConstantVariable.GetConfigurationKey()))
                {
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

        public static FatalErrorHandler GetKey()
        {
            try
            {
                using (var stream = File.OpenRead(ConstantVariable.GetConfigurationKey()))
                {
                    return Serializer.Deserialize<FatalErrorHandler>(stream);
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
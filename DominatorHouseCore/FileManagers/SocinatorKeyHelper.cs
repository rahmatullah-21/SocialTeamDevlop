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
                var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                return genericFileManager.GetModel<FatalErrorHandler>(ConstantVariable.GetConfigurationKey());
                //using (var stream = File.OpenRead(ConstantVariable.GetConfigurationKey()))
                //{
                //    return Serializer.Deserialize<FatalErrorHandler>(stream);
                //}
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

    }
}
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
                using (var stream = File.Create(ServiceLocator.Current.GetInstance<IConstantVariable>().GetConfigurationKey()))
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
                return genericFileManager.GetModel<FatalErrorHandler>(ServiceLocator.Current.GetInstance<IConstantVariable>().GetConfigurationKey());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

    }
}
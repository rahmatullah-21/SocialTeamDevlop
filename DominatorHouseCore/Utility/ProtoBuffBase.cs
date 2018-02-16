using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DominatorHouseCore.Utility
{
    internal class ProtoBuffBase
    {
        #region Serialize

        
        /// <summary>
        /// SerializeObjects<T>() method is used to serialize the LIST of objects
        /// </summary>
        /// <typeparam name="T">Specify the object is belongs to which Type </typeparam>
        /// <param name="objectType">The object which is going to serialize</param>
        /// <param name="filePath">Specify the filepath where the serialized object is going to save </param>
        internal static bool SerializeObjects<T>(T objects, string filePath) where T : IEnumerable
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            if (!(objects is IList))
                throw new ArgumentException(nameof(objects));

            try
            {                
                using (var stream = File.OpenWrite(filePath))
                {
                    Serializer.Serialize(stream, objects);                 
                }                
            }
            catch (Exception ex)
            {
                ex.DebugLog($"ProtobufError: Unable to serialize object of type {typeof(T).FullName} to {filePath}");                
                throw;        
            }


            return true;
        }

        // Method to append new object to file
        internal static void AppendObject<T>(T obj, string filePath)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            try
            {
                if (!File.Exists(filePath))
                    using (File.Create(filePath)) { }

                using (var stream = File.Open(filePath, FileMode.Append))
                {
                    Serializer.Serialize(stream, obj);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog($"ProtobufError: Unable to append object of type {typeof(T).Name} to {filePath}");
                throw;
            }
        }

        #endregion


        #region Deserialize 


        /// <summary>
        /// DeserializeObjects<T>() Method is used to deserialize the file and return  List ofType(T)
        /// </summary>
        /// <typeparam name="T">Class which is goes convert back</typeparam>
        /// <param name="filePath">Source of the file </param>
        /// <returns>List of Type T</returns>
        internal static List<T> DeserializeObjects<T>(string filePath) where T : class
        {
            try
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return Serializer.DeserializeItems<T>(stream, PrefixStyle.Base128, 1).ToList();
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog($"Unable to deserialize object of type {typeof(T).FullName} from {filePath}");
                return new List<T>();
            }            
        }

        #endregion        
    }
}

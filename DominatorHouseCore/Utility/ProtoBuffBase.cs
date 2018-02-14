using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DominatorHouseCore.Utility
{
   public class ProtoBuffBase
    {
        #region Serialize

        
        /// <summary>
        /// SerializeObjects<T>() method is used to serialize the LIST of objects
        /// </summary>
        /// <typeparam name="T">Specify the object is belongs to which Type </typeparam>
        /// <param name="objectType">The object which is going to serialize</param>
        /// <param name="filePath">Specify the filepath where the serialized object is going to save </param>
        public static bool SerializeObjects<T>(T objects, string filePath) where T : IEnumerable
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            if (!(objects is IList))
                throw new ArgumentException(nameof(objects));

            try
            {
                using (Stream fileStream = File.Create(filePath))
                {
                    Serializer.Serialize(fileStream, objects);                 
                }                
            }
            catch (Exception ex)
            {
                ex.DebugLog($"ProtobufError: Unable to serialize object of type {typeof(T).FullName}");                
                throw;        
            }


            return true;
        }        


        public static void SerializeOneObject<T>(T obj, string filePath) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            try
            {
                using (Stream fileStream = File.Create(filePath))
                {
                    Serializer.Serialize(fileStream, obj);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog($"ProtobufError: Unable to serialize object of type {typeof(T).FullName}");
                throw;
            }
        }

        #endregion


        #region Deserialize 

        static Dictionary<string, Stream> _openedFiles = new Dictionary<string, Stream>();

        /// <summary>
        /// DeserializeObjects<T>() Method is used to deserialize the file and return  List ofType(T)
        /// </summary>
        /// <typeparam name="T">Class which is goes convert back</typeparam>
        /// <param name="filePath">Source of the file </param>
        /// <returns>List of Type T</returns>
        public static IEnumerable<T> DeserializeObjects<T>(string filename) where T : class
        {
            try
            {
                Stream stream = null;
                if (_openedFiles.ContainsKey(filename))
                    stream = _openedFiles[filename];
                else
                {
                    stream = File.OpenRead(filename);
                    _openedFiles.Add(filename, stream);
                }

                return Serializer.DeserializeItems<T>(stream, PrefixStyle.Base128, 1);                
            }
            catch (Exception ex)
            {
                ex.ErrorLog($"Unable to deserialize object of type {typeof(T).FullName} from {filename}");
                return new List<T>();
            }            
        }

        #endregion        
    }
}

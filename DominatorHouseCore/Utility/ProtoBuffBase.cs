using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DominatorHouseCore.Utility
{
   public class ProtoBuffBase
    {
        #region Serialize

        /// <summary>
        /// SerializeObjects<T>() method is used to serialize the object and appended in the specified filepath
        /// </summary>
        /// <typeparam name="T">Specify the object is belongs to which Type </typeparam>
        /// <param name="objectType">The object which is going to serialize</param>
        /// <param name="filePath">Specify the filepath where the serialized object is going to save </param>
        public static bool SerializeObjects<T>(T objectType, string filePath) where T : class
        {
            if (objectType == null) return false;

            if (!File.Exists(filePath))
            {
                try
                {
                    using (Stream fileStream = File.Create(filePath))
                    {
                        Serializer.SerializeWithLengthPrefix(fileStream, objectType, PrefixStyle.Base128, 1);
                        fileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            else
            {
                try
                {
                    using (Stream fileStream = File.Open(filePath, FileMode.Append))
                    {
                        Serializer.SerializeWithLengthPrefix(fileStream, objectType, PrefixStyle.Base128, 1);
                        fileStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            return true;
        }

        public static bool SerializeListObject<T>(IEnumerable<T> objectType, string filePath) where T : class
        {
            foreach (T singleobject in objectType)
            {
                try
                {
                    var status = SerializeObjects<T>(singleobject, filePath);
                    if (!status)
                        return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
            return true;
        }

        #endregion




        #region Deserialize 
        /// <summary>
        /// DeserializeObjects<T>() Method is used to deserialize the file and return  List ofType(T)
        /// </summary>
        /// <typeparam name="T">Class which is goes convert back</typeparam>
        /// <param name="filePath">Source of the file </param>
        /// <returns>List of Type T</returns>
        public static List<T> DeserializeObjects<T>(string filename) where T : class
        {
            try
            {
                using (Stream file = File.Open(filename, FileMode.Open))
                {
                    return Serializer.DeserializeItems<T>(file, PrefixStyle.Base128, 1).ToList();
                }
            }
            catch (Exception e)
            {
            }
            return new List<T>();
        }

        #endregion




        #region UpdateObject 
       /// <summary>
       /// 
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="filename"></param>
       /// <returns></returns>
        public static void UpdateObjects<T>(string filename , T obj) where T : class
        {
            try
            {
                using (Stream file = File.Open(filename, FileMode.Open))
                {
                    Serializer.MergeWithLengthPrefix(file, obj, PrefixStyle.Base128);
                }
            }
            catch (Exception e)
            {
            }

        }

        #endregion

    }
}

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using DominatorHouseCore.Models;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Utility
{
    [ProtoContract]
    internal class ListWrapper<T>
    {
        [ProtoMember(1)] public List<T> List { get; set; } = new List<T>();

        public ListWrapper()
        {
        }
        public ListWrapper(List<T> list)
        {
            List = list;
        }
    }

    public interface IProtoBuffBase
    {
        /// <summary>
        ///     SerializeObjects<T>() method is used to serialize the LIST of objects
        /// </summary>
        /// <typeparam name="T">Specify the object is belongs to which Type </typeparam>
        /// <param name="objectType">The object which is going to serialize</param>
        /// <param name="filePath">Specify the filepath where the serialized object is going to save </param>
        bool SerializeList<T>(List<T> list, string filePath) where T : class;

        void AppendObject<T>(T obj, string filePath);

        /// <summary>
        ///     DeserializeObjects<T>() Method is used to deserialize the file and return  List ofType(T)
        /// </summary>
        /// <typeparam name="T">Class which is goes convert back</typeparam>
        /// <param name="filePath">Source of the file </param>
        /// <returns>List of Type T</returns>
        List<T> DeserializeList<T>(string filePath) where T : class;

        T Deserialize<T>(string filePath) where T : class, new();
    }


    internal class ProtoBuffBase : IProtoBuffBase
    {
        #region Serialize

        /// <summary>
        ///     SerializeObjects<T>() method is used to serialize the LIST of objects
        /// </summary>
        /// <typeparam name="T">Specify the object is belongs to which Type </typeparam>
        /// <param name="objectType">The object which is going to serialize</param>
        /// <param name="filePath">Specify the filepath where the serialized object is going to save </param>
        public bool SerializeList<T>(List<T> list, string filePath) where T : class
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!(list is IList))
                throw new ArgumentException(nameof(list));

            try
            {
                DirectoryUtilities.CreateDirectory(Path.GetDirectoryName(filePath));
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();

                using (var stream = File.Open(filePath, FileMode.Truncate))
                {
                    Serializer.Serialize(stream, new ListWrapper<T>(list));
                    stream.SetLength(stream.Position);
                }
            }
            catch (Exception ex)
            {
                //ex.DebugLog($"ProtobufError: Unable to serialize object of type {typeof(T).FullName} to {filePath}");
                ex.DebugLog();
                throw;
            }


            return true;
        }

        // Method to append new object to file
        public void AppendObject<T>(T obj, string filePath)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is IEnumerable)
                throw new ArgumentException("AppendObjects does not work for collection");

            Stream stream = null;
            try
            {
                if (filePath.ToLower().Contains("account"))
                    Debug.Assert(typeof(T) == typeof(DominatorAccountModel));

                if (!File.Exists(filePath))
                    stream = File.Create(filePath);
                else
                    stream = File.Open(filePath, FileMode.Append);

                Serializer.SerializeWithLengthPrefix(stream, obj, PrefixStyle.Base128, 1);
                stream.SetLength(stream.Position);
            }
            catch (Exception ex)
            {
                // ex.DebugLog($"ProtobufError: Unable to append object of type {typeof(T).Name} to {filePath}");
                ex.DebugLog();
                throw;
            }
            finally
            {
                stream?.Close();
            }
        }

        #endregion


        #region Deserialize 

        /// <summary>
        ///     DeserializeObjects<T>() Method is used to deserialize the file and return  List ofType(T)
        /// </summary>
        /// <typeparam name="T">Class which is goes convert back</typeparam>
        /// <param name="filePath">Source of the file </param>
        /// <returns>List of Type T</returns>
        public List<T> DeserializeList<T>(string filePath) where T : class
        {
            try
            {
                if (File.Exists(filePath))
                    using (var stream = File.OpenRead(filePath))
                    {
                        if (filePath.ToLower().Contains("account"))
                            Debug.Assert(typeof(T) ==
                                         typeof(DominatorAccountModel
                                         )); // account model have to be only DominatorAccountModel
                        var wrapper = Serializer.Deserialize<ListWrapper<T>>(stream);

                        return wrapper.List ?? new List<T>();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return new List<T>();
            }

            return new List<T>();
        }

        public T Deserialize<T>(string filePath) where T : class, new()
        {
            try
            {
                if (File.Exists(filePath))
                    using (var stream = File.OpenRead(filePath))
                    {
                        // account model have to be only DominatorAccountModel

                        var wrapper = Serializer.Deserialize<T>(stream);

                        return wrapper ?? new T();
                    }
            }
            catch (Exception ex)
            {
                // ex.DebugLog($"Unable to deserialize object of type {typeof(T).FullName} from {filePath}");
                ex.DebugLog();
            }

            return new T();
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Patterns
{  
    [Serializable]
    public static class PrototypeBase
    {       
        //Deep Clone
        public static T DeepClone<T>(this T obj) where T: class
        {         
            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    T copy = (T)formatter.Deserialize(stream);
                    stream.Close();
                    return copy;
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

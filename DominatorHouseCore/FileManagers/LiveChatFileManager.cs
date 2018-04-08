using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public static class LiveChatFileManager
    {
        private static readonly object _liveChatFileLocker = new object();
        public static bool SaveLiveChat(Dictionary<string, ObservableCollection<ChatDetails>> chat)
        {
            try
            {
                Stream stream = null;
                string filePath = ConstantVariable.GetLiveChatFile();
                try
                {
                    if (!File.Exists(filePath))
                        stream = File.Create(filePath);
                    else
                        stream = File.Open(filePath, FileMode.Truncate);

                    Serializer.Serialize(stream, chat);
                }
                catch (Exception )
                { }
                stream.Close();
          
                GlobusLogHelper.log.Debug($"Chat successfully saved");
                return true;
            }
            catch (Exception )
            {
                return false;
            }
            return false;
        }
        public static Dictionary<string, ObservableCollection<ChatDetails>> GetAllChatDetails()
        {
            Stream stream = null;
            string filePath = ConstantVariable.GetLiveChatFile();
            if (!File.Exists(filePath))
                stream = File.Create(filePath);
            else
                stream = File.OpenRead(filePath);
            Dictionary<string, ObservableCollection<ChatDetails>> dict = null;
            lock (_liveChatFileLocker)
                dict = Serializer.Deserialize<Dictionary<string, ObservableCollection<ChatDetails>>>(stream);
            stream.Close();
            return dict;
            
        }
    }
}

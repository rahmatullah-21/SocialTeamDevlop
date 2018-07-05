using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ManageMessagesModel : BindableBase
    {
        private int _serialNo;

        public int SerialNo
        {
            get
            {
                return _serialNo;
            }
            set
            {
                if (value == _serialNo)
                    return;
                SetProperty(ref _serialNo, value);
            }
        }
        private string _messagesText;

        public string MessagesText
        {
            get
            {
                return _messagesText;
            }
            set
            {
                if (value == _messagesText)
                    return;
                SetProperty(ref _messagesText, value);
            }
        }
        public string _MediaPath = string.Empty;
        public string MediaPath
        {
            get
            {
                return _MediaPath;
            }
            set
            {
                if (value == _MediaPath)
                    return;
                SetProperty(ref _MediaPath, value);
            }
        }

        public ObservableCollection<QueryContent> SelectedQuery { get; set; } = new ObservableCollection<QueryContent>();


        public ObservableCollection<QueryContent> LstQueries { get; set; } = new ObservableCollection<QueryContent>();


    }

  
}
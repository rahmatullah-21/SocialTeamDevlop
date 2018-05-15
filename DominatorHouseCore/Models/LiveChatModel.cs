using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using System.Windows;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class LiveChatModel:BindableBase
    {
       
        private List<SenderDetails> _lstSender=new List<SenderDetails>();
        [ProtoMember(1)]
        public List<SenderDetails> LstSender
        {
            get
            {
                return _lstSender;
            }
            set
            {
                if (value == _lstSender)
                    return;
                SetProperty(ref _lstSender, value);
            }
        }

        private Dictionary<string , ObservableCollection<ChatDetails>> _accountChatDetails=new Dictionary<string, ObservableCollection<ChatDetails>>();
        [ProtoMember(2)]
        public Dictionary<string , ObservableCollection<ChatDetails>> AccountChatDetails
        {
            get { return _accountChatDetails; }
            set
            {
                if (value == _accountChatDetails)
                    return;
                SetProperty(ref _accountChatDetails, value);
            }
        }


        private ObservableCollection<ChatDetails> _lstChat= new ObservableCollection<ChatDetails>();

        public ObservableCollection<ChatDetails> LstChat
        {
            get
            {
                return _lstChat;
            }
            set
            {
                if (value == _lstChat)
                    return;
                SetProperty(ref _lstChat, value);
            }
        }
      

        private SenderDetails _senderDetails = new SenderDetails();
        [ProtoMember(3)]
        public SenderDetails SenderDetails
        {
            get
            {
                return _senderDetails;
            }
            set
            {
                if (value == _senderDetails)
                    return;
                SetProperty(ref _senderDetails, value);
            }
        }

        [ProtoMember(4)]
        public string SenderDetailsCursorId { get; set; }

        public DominatorAccountModel dominatorAccountModel { get; set; }

    }


    [ProtoContract]
    public class SenderDetails : BindableBase
    {
        private string _senderName;
        [ProtoMember(1)]
        public string SenderName
        {
            get
            {
                return _senderName;
            }
            set
            {
                if (value == _senderName)
                    return;
                SetProperty(ref _senderName, value);
            }
        }
        private string _senderImage;
        [ProtoMember(2)]
        public string SenderImage
        {
            get
            {
                return _senderImage;
            }
            set
            {
                if (value == _senderImage)
                    return;
                SetProperty(ref _senderImage, value);
            }
        }
        private string _lastMessegedate;
        [ProtoMember(3)]
        public string LastMessegedate
        {
            get
            {
                return _lastMessegedate;
            }
            set
            {
                if (value == _lastMessegedate)
                    return;
                SetProperty(ref _lastMessegedate, value);
            }
        }

        private string _lastMesseges;
        [ProtoMember(4)]
        public string LastMesseges
        {
            get
            {
                return _lastMesseges;
            }
            set
            {
                if (value == _lastMesseges)
                    return;
                SetProperty(ref _lastMesseges, value);
            }
        }


        public string SenderId { get; set; }


        [ProtoMember(5)]
        public string ThreadId { get; set; }


        public bool MoreAvailableMax { get; set; }

        public bool MoreAvailableMin { get; set; }

        public string NextMaxId { get; set; }
        public string NextMinId { get; set; }
    }

    [ProtoContract]
    public class ChatDetails : BindableBase
    {
        private string _sender;
        [ProtoMember(1)]
        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                if (value == _sender)
                    return;
                SetProperty(ref _sender, value);
            }
        }
        private string _messeges;
        [ProtoMember(2)]
        public string Messeges
        {
            get
            {
                return _messeges;
            }
            set
            {
                if (value == _messeges)
                    return;
                SetProperty(ref _messeges, value);
            }
        }
        private string _type;
        [ProtoMember(3)]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value == _type)
                    return;
                SetProperty(ref _type, value);
            }
        }
        private string _time;

        [ProtoMember(4)]
        public string Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value == _time)
                    return;
                SetProperty(ref _time, value);
            }
        }

        [ProtoMember(5)]
        public string itemId { get; set; }

        [ProtoMember(6)]
        public string SenderId { get; set; }


        private bool _IsRecieved { get; set; }

        public bool IsRecieved
        {
            get { return _IsRecieved; }
            set
            {
                if (value == _IsRecieved) return;
                _IsRecieved = value;
                this.OnPropertyChanged("IsRecieved");
              
            }
        }

        public AllignmentConvertor allignmentConvertor = new AllignmentConvertor();

    }


    public class AllignmentConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return (bool)value == true ? HorizontalAlignment.Right: HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Right;
        }
    }
}

using DominatorHouseCore.Utility;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class LiveChatModel : BindableBase
    {

        private ObservableCollection<SenderDetails> _lstSender = new ObservableCollection<SenderDetails>();
        [ProtoMember(1)]
        public ObservableCollection<SenderDetails> LstSender
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


        private Dictionary<string, ObservableCollection<ChatDetails>> _accountChatDetails = new Dictionary<string, ObservableCollection<ChatDetails>>();
        [ProtoMember(2)]
        public Dictionary<string, ObservableCollection<ChatDetails>> AccountChatDetails
        {
            get { return _accountChatDetails; }
            set
            {
                if (value == _accountChatDetails)
                    return;
                SetProperty(ref _accountChatDetails, value);
            }
        }



        private ObservableCollection<ChatDetails> _lstChat = new ObservableCollection<ChatDetails>();

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

        private ObservableCollection<string> _lstImages = new ObservableCollection<string>();

        public ObservableCollection<string> LstImages
        {
            get
            {
                return _lstImages;
            }
            set
            {
                if (value == _lstImages)
                    return;
                SetProperty(ref _lstImages, value);
            }
        }

        private int _imageCount = 0;

        public int ImageCount
        {
            get
            {
                return _imageCount;
            }
            set
            {
                if (value == _imageCount)
                    return;
                SetProperty(ref _imageCount, value);
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

        public DominatorAccountModel DominatorAccountModel { get; set; } = new DominatorAccountModel();


        private ObservableCollection<string> _accountNames = new ObservableCollection<string>();
        [ProtoIgnore]
        public ObservableCollection<string> AccountNames
        {
            get
            {
                return _accountNames;
            }
            set
            {
                SetProperty(ref _accountNames, value);
            }
        }


        private string _selectedAccount = string.Empty;

        public string SelectedAccount
        {
            get
            {
                return _selectedAccount;
            }
            set
            {
                if (_selectedAccount == value)
                    return;
                SetProperty(ref _selectedAccount, value);
            }
        }


        private string _textMessage = string.Empty;

        public string TextMessage
        {
            get
            {
                return _textMessage;
            }
            set
            {
                if (_textMessage == value)
                    return;
                SetProperty(ref _textMessage, value);
            }
        }

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

        [ProtoMember(5)]
        public string ThreadId { get; set; }

        [ProtoMember(6)]
        public string LastMessageOwnerId { get; set; }

        [ProtoMember(7)]
        public string SenderId { get; set; }

        [ProtoMember(8)]
        public string AccountId { get; set; } = string.Empty;

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
                try
                {
                    if (Utilities.GetIntegerOnlyString(Time) != Time)
                        MessageTime = DateTime.Parse(Time);
                    else
                        MessageTime = DateTimeUtilities.EpochToDateTimeLocal(long.Parse(Time));
                }
                catch (Exception ex)
                {

                }
            }
        }

        [ProtoMember(5)]
        public string MessegesId { get; set; }

        [ProtoMember(6)]
        public string SenderId { get; set; }


        private bool _IsRecieved;

        public bool IsRecieved
        {
            get { return _IsRecieved; }
            set
            {
                if (value == _IsRecieved) return;
                _IsRecieved = value;
                OnPropertyChanged();

            }
        }
        private string _clientContext;

        public string ClientContext
        {
            get { return _clientContext; }
            set
            {
                if (value == _clientContext) return;
                _clientContext = value;
                OnPropertyChanged();

            }
        }

        private ChatMessageType _messegeType;
        [ProtoMember(7)]
        public ChatMessageType MessegeType
        {
            get
            {
                return _messegeType;
            }
            set
            {
                if (value == _messegeType)
                    return;
                SetProperty(ref _messegeType, value);
            }
        }


        private ObservableCollection<string> _listMediaUrls;

        [ProtoMember(8)]
        public ObservableCollection<string> ListMediaUrls
        {
            get
            {
                return _listMediaUrls;
            }
            set
            {
                if (value == _listMediaUrls)
                    return;
                SetProperty(ref _listMediaUrls, value);
            }
        }

        private DateTime _messageTime;

        [ProtoMember(9)]
        public DateTime MessageTime
        {
            get
            {
                return _messageTime;
            }
            set
            {
                SetProperty(ref _messageTime, value);
            }
        }
    }


    public class AllignmentConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (HorizontalAlignment)value == HorizontalAlignment.Right;
        }
    }
}

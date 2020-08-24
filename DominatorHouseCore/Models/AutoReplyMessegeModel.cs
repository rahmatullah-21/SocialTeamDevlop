#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class AutoReplyMessegeModel : BindableBase
    {
        private string _status;

        [ProtoMember(1)]
        public string Status
        {
            get => _status;
            set
            {
                if (_status == value)
                    return;
                SetProperty(ref _status, value);
            }
        }

        private string _messegeText;

        [ProtoMember(2)]
        public string MessegeText
        {
            get => _messegeText;
            set
            {
                if (_messegeText == value)
                    return;
                SetProperty(ref _messegeText, value);
            }
        }

        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;

        [ProtoMember(3)]
        public bool IsReplyToMessagesThatContainSpecificWord﻿Checked
        {
            get => _isReplyToMessagesThatContainSpecificWord﻿Checked;
            set
            {
                if (_isReplyToMessagesThatContainSpecificWord﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToMessagesThatContainSpecificWord﻿Checked, value);
            }
        }

        private bool _isReplyToPendingMessages﻿﻿Checked;

        [ProtoMember(4)]
        public bool IsReplyToPendingMessages﻿﻿Checked
        {
            get => _isReplyToPendingMessages﻿﻿Checked;
            set
            {
                if (_isReplyToPendingMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToPendingMessages﻿﻿Checked, value);
            }
        }

        private bool _isReplyToAllMessages﻿﻿Checked;

        [ProtoMember(5)]
        public bool IsReplyToAllMessagesChecked
        {
            get => _isReplyToAllMessages﻿﻿Checked;
            set
            {
                if (_isReplyToAllMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToAllMessages﻿﻿Checked, value);
            }
        }

        private bool _isPrioritizeMessages﻿Checked;

        [ProtoMember(6)]
        public bool IsPrioritizeMessages﻿Checked
        {
            get => _isPrioritizeMessages﻿Checked;
            set
            {
                if (_isPrioritizeMessages﻿Checked == value)
                    return;
                SetProperty(ref _isPrioritizeMessages﻿Checked, value);
            }
        }

        private bool _isSendSecondaryMessagesAfterChecked;

        [ProtoMember(6)]
        public bool IsSendSecondaryMessagesAfterChecked
        {
            get => _isSendSecondaryMessagesAfterChecked;
            set
            {
                if (_isSendSecondaryMessagesAfterChecked == value)
                    return;
                SetProperty(ref _isSendSecondaryMessagesAfterChecked, value);
            }
        }

        private RangeUtilities _sendSecondaryMessages = new RangeUtilities();

        [ProtoMember(7)]
        public RangeUtilities SendSecondaryMessages
        {
            get => _sendSecondaryMessages;
            set
            {
                if (_sendSecondaryMessages == value)
                    return;
                SetProperty(ref _sendSecondaryMessages, value);
            }
        }

        private bool _isSendIfUserHasReplied﻿﻿﻿﻿Checked;

        [ProtoMember(8)]
        public bool IsSendIfUserHasReplied﻿﻿﻿﻿Checked
        {
            get => _isSendIfUserHasReplied﻿﻿﻿﻿Checked;
            set
            {
                if (_isSendIfUserHasReplied﻿﻿﻿﻿Checked == value)
                    return;
                SetProperty(ref _isSendIfUserHasReplied﻿﻿﻿﻿Checked, value);
            }
        }

        private bool _isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked;

        [ProtoMember(9)]
        public bool IsSendIfUserHasRepliedNot﻿﻿﻿﻿Checked
        {
            get => _isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked;
            set
            {
                if (_isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked == value)
                    return;
                SetProperty(ref _isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked, value);
            }
        }

        private bool _isCheckForNewMessages﻿﻿﻿﻿Checked;

        [ProtoMember(10)]
        public bool IsCheckForNewMessages﻿﻿﻿﻿Checked
        {
            get => _isCheckForNewMessages﻿﻿﻿﻿Checked;
            set
            {
                if (_isCheckForNewMessages﻿﻿﻿﻿Checked == value)
                    return;
                SetProperty(ref _isCheckForNewMessages﻿﻿﻿﻿Checked, value);
            }
        }

        private int _checkMessagesMinutes;

        [ProtoMember(11)]
        public int CheckMessagesMinutes
        {
            get => _checkMessagesMinutes;
            set
            {
                if (_checkMessagesMinutes == value)
                    return;
                SetProperty(ref _checkMessagesMinutes, value);
            }
        }

        private string _specificWord;

        [ProtoMember(2)]
        public string SpecificWord
        {
            get => _specificWord;
            set
            {
                if (_specificWord == value)
                    return;
                SetProperty(ref _specificWord, value);
            }
        }
    }
}
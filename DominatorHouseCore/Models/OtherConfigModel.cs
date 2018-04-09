using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class OtherConfigModel : BindableBase
    {
        private bool _isSendSecondaryMessagesAfterChecked;
        [ProtoMember(1)]
        public bool IsSendSecondaryMessagesAfterChecked
        {
            get
            {
                return _isSendSecondaryMessagesAfterChecked;
            }
            set
            {
                if (_isSendSecondaryMessagesAfterChecked == value)
                    return;
                SetProperty(ref _isSendSecondaryMessagesAfterChecked, value);

            }
        }
        private RangeUtilities _sendSecondaryMessages = new RangeUtilities();
        [ProtoMember(2)]
        public RangeUtilities SendSecondaryMessages
        {
            get
            {
                return _sendSecondaryMessages;
            }
            set
            {
                if (_sendSecondaryMessages == value)
                    return;
                SetProperty(ref _sendSecondaryMessages, value);

            }
        }
        private bool _isSendIfUserHasReplied﻿﻿﻿﻿Checked;
        [ProtoMember(3)]
        public bool IsSendIfUserHasReplied﻿﻿﻿﻿Checked
        {
            get
            {
                return _isSendIfUserHasReplied﻿﻿﻿﻿Checked;
            }
            set
            {
                if (_isSendIfUserHasReplied﻿﻿﻿﻿Checked == value)
                    return;
                SetProperty(ref _isSendIfUserHasReplied﻿﻿﻿﻿Checked, value);

            }
        }
        private bool _isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked;
        [ProtoMember(4)]
        public bool IsSendIfUserHasRepliedNot﻿﻿﻿﻿Checked
        {
            get
            {
                return _isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked;
            }
            set
            {
                if (_isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked == value)
                    return;
                SetProperty(ref _isSendIfUserHasRepliedNot﻿﻿﻿﻿Checked, value);

            }
        }
        private bool _isCheckForNewMessages﻿﻿﻿﻿Checked;
        [ProtoMember(5)]
        public bool IsCheckForNewMessages﻿﻿﻿﻿Checked
        {
            get
            {
                return _isCheckForNewMessages﻿﻿﻿﻿Checked;
            }
            set
            {
                if (_isCheckForNewMessages﻿﻿﻿﻿Checked == value)
                    return;
                SetProperty(ref _isCheckForNewMessages﻿﻿﻿﻿Checked, value);

            }
        }
        private int _checkMessagesMinutes;
        [ProtoMember(6)]
        public int CheckMessagesMinutes
        {
            get
            {
                return _checkMessagesMinutes;
            }
            set
            {
                if (_checkMessagesMinutes == value)
                    return;
                SetProperty(ref _checkMessagesMinutes, value);

            }
        }
    }
}
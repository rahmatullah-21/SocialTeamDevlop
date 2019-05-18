using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class QuoraModel : BindableBase
    {
        private bool _isEnableFollowDifferentUserChecked;
        [ProtoMember(1)]
        public bool IsEnableFollowDifferentUserChecked
        {
            get
            {
                return _isEnableFollowDifferentUserChecked;
            }
            set
            {
                if (value == _isEnableFollowDifferentUserChecked)
                    return;
                SetProperty(ref _isEnableFollowDifferentUserChecked, value);
            }
        }
    }
    [ProtoContract]
    public class LinkedInModel : BindableBase
    {
        private bool _isEnableExportingHTMLOfDifferentConnections;
        [ProtoMember(1)]
        public bool IsEnableExportingHTMLOfDifferentConnections
        {
            get
            {
                return _isEnableExportingHTMLOfDifferentConnections;
            }
            set
            {
                if (value == _isEnableExportingHTMLOfDifferentConnections)
                    return;
                SetProperty(ref _isEnableExportingHTMLOfDifferentConnections, value);
            }
        }

        private bool _isFilterDuplicateMessageByCheckingConversationsHistory;
        [ProtoMember(2)]
        public bool IsFilterDuplicateMessageByCheckingConversationsHistory
        {
            get
            {
                return _isFilterDuplicateMessageByCheckingConversationsHistory;
            }
            set
            {
                if (value == _isFilterDuplicateMessageByCheckingConversationsHistory)
                    return;
                SetProperty(ref _isFilterDuplicateMessageByCheckingConversationsHistory, value);
            }
        }
        private bool _IsEnableSendConnectionRequestToDifferentUsers;
        [ProtoMember(3)]
        public bool IsEnableSendConnectionRequestToDifferentUsers
        {
            get { return _IsEnableSendConnectionRequestToDifferentUsers; }
            set
            {
                if (value == _IsEnableSendConnectionRequestToDifferentUsers)
                    return;
                SetProperty(ref _IsEnableSendConnectionRequestToDifferentUsers, value);
            }
        }

    }
    [ProtoContract]
    public class InstagramUserModel : BindableBase
    {
        private bool _isEnableScrapeDiffrentUserChecked;
        [ProtoMember(1)]
        public bool IsEnableScrapeDiffrentUserChecked
        {
            get
            {
                return _isEnableScrapeDiffrentUserChecked;
            }
            set
            {
                if (value == _isEnableScrapeDiffrentUserChecked)
                    return;
                SetProperty(ref _isEnableScrapeDiffrentUserChecked, value);
            }
        }
    }

}
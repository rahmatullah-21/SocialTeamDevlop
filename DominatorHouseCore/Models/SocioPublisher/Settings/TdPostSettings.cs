using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class TdPostSettings : BindableBase, ITdPostSettings
    {
        private bool _isDeletePostAfterHours;
        private int _deletePostAfterHours;
        private bool _isMentionUser;
        private string _mentionUserList;
        private List<string> _rssImageList;

        [ProtoMember(1)]
        public bool IsDeletePostAfterHours
        {
            get
            {
                return _isDeletePostAfterHours;
            }
            set
            {             
                if (_isDeletePostAfterHours == value)
                    return;

                SetProperty(ref _isDeletePostAfterHours, value);
            }
        }

        [ProtoMember(2)]
        public int DeletePostAfterHours
        {
            get
            {
                return _deletePostAfterHours;
            }
            set
            {              
                if (_deletePostAfterHours == value)
                    return;
              
                SetProperty(ref _deletePostAfterHours, value);
            }
        }

        [ProtoMember(3)]
        public bool IsMentionUser
        {
            get
            {
                return _isMentionUser;
            }
            set
            {              
                if (_isMentionUser == value)
                    return;
             
                SetProperty(ref _isMentionUser, value);
            }
        }

        [ProtoMember(4)]
        public string MentionUserList
        {
            get
            {
                return _mentionUserList;
            }
            set
            {           
                if (_mentionUserList == value)
                    return;
                
                SetProperty(ref _mentionUserList, value);
            }
        }

        [ProtoMember(5)]
        public List<string> RssImageList
        {
            get
            {
                return _rssImageList;
            }
            set
            {
                if (_rssImageList == value)
                    return;

                SetProperty(ref _rssImageList, value);
            }
        }
    }
}
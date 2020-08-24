#region

using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models.SocioPublisher.Settings
{
    [Serializable]
    [ProtoContract]
    public class GdPostSettings : BindableBase, IGdPostSettings
    {
        private string _postTitle;
        private bool _isPostAsStoryPost;
        private bool _isDeletePostAfterHours;
        private int _deletePostAfterHours;
        private bool _isGeoLocation;
        private string _geoLocationList;
        private bool _isTagUser;
        private string _tagUserList;
        private bool _isLocationName = true;
        private bool _isLocationId;

        [ProtoMember(1)]
        public string PostTitle
        {
            get => _postTitle;
            set
            {
                if (_postTitle == value)
                    return;
                SetProperty(ref _postTitle, value);
            }
        }

        [ProtoMember(2)]
        public bool IsPostAsStoryPost
        {
            get => _isPostAsStoryPost;
            set
            {
                if (_isPostAsStoryPost == value)
                    return;
                SetProperty(ref _isPostAsStoryPost, value);
            }
        }

        [ProtoMember(3)]
        public bool IsDeletePostAfterHours
        {
            get => _isDeletePostAfterHours;
            set
            {
                if (_isDeletePostAfterHours == value)
                    return;

                SetProperty(ref _isDeletePostAfterHours, value);
            }
        }

        [ProtoMember(4)]
        public int DeletePostAfterHours
        {
            get => _deletePostAfterHours;
            set
            {
                if (_deletePostAfterHours == value)
                    return;

                SetProperty(ref _deletePostAfterHours, value);
            }
        }

        [ProtoMember(5)]
        public bool IsGeoLocation
        {
            get => _isGeoLocation;
            set
            {
                if (_isGeoLocation == value)
                    return;

                SetProperty(ref _isGeoLocation, value);
            }
        }

        [ProtoMember(6)]
        public string GeoLocationList
        {
            get => _geoLocationList;
            set
            {
                if (_geoLocationList == value)
                    return;

                SetProperty(ref _geoLocationList, value);
            }
        }

        [ProtoMember(7)]
        public bool IsTagUser
        {
            get => _isTagUser;
            set
            {
                if (_isTagUser == value)
                    return;

                SetProperty(ref _isTagUser, value);
            }
        }

        [ProtoMember(8)]
        public string TagUserList
        {
            get => _tagUserList;
            set
            {
                if (_tagUserList == value)
                    return;

                SetProperty(ref _tagUserList, value);
            }
        }

        [ProtoMember(9)]
        public bool IsGeoLocationName
        {
            get => _isLocationName;
            set
            {
                if (value)
                    IsGeoLocationName = false;

                SetProperty(ref _isLocationName, value);
            }
        }

        [ProtoMember(10)]
        public bool IsGeoLocationId
        {
            get => _isLocationId;
            set
            {
                if (value)
                    IsGeoLocationName = false;

                SetProperty(ref _isLocationId, value);
            }
        }

        private bool _IsMentionUser;

        [ProtoMember(11)]
        public bool IsMentionUser
        {
            get => _IsMentionUser;
            set
            {
                if (value)
                    _IsMentionUser = false;

                SetProperty(ref _IsMentionUser, value);
            }
        }

        private string _MentionUserList;

        [ProtoMember(12)]
        public string MentionUserList
        {
            get => _MentionUserList;
            set
            {
                if (_MentionUserList == value)
                    return;

                SetProperty(ref _MentionUserList, value);
            }
        }
    }
}
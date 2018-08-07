using System;
using DominatorHouseCore.Interfaces.SocioPublisher;
using DominatorHouseCore.Utility;
using ProtoBuf;

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

        [ProtoMember(1)]
        public string PostTitle
        {
            get
            {
                return _postTitle;
            }
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
            get
            {
                return _isPostAsStoryPost;
            }
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

        [ProtoMember(4)]
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

        [ProtoMember(5)]
        public bool IsGeoLocation
        {
            get
            {
                return _isGeoLocation;
            }
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
            get
            {
                return _geoLocationList;
            }
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
            get
            {
                return _isTagUser;
            }
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
            get
            {
                return _tagUserList;
            }
            set
            {
                if (_tagUserList == value)
                    return;
              
                SetProperty(ref _tagUserList, value);
            }
        }
    }
}
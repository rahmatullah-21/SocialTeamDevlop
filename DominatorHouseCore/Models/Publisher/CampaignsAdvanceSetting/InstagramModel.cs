using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
    public class InstagramModel : BindableBase
    {
        private bool _isPostusingGeoLocation;

        [ProtoMember(1)]
        public bool IsPostusingGeoLocation
        {
            get
            {
                return _isPostusingGeoLocation;
            }
            set
            {
                if (_isPostusingGeoLocation == value)
                    return;
                SetProperty(ref _isPostusingGeoLocation, value);
            }
        }
        private bool _isEnableAutomaticHashTags;

        [ProtoMember(2)]
        public bool IsEnableAutomaticHashTags
        {
            get
            {
                return _isEnableAutomaticHashTags;
            }
            set
            {
                if (_isEnableAutomaticHashTags == value)
                    return;
                SetProperty(ref _isEnableAutomaticHashTags, value);
            }
        }
        private int _maxHashtagsPerPost;

        [ProtoMember(3)]
        public int MaxHashtagsPerPost
        {
            get
            {
                return _maxHashtagsPerPost;
            }
            set
            {
                if (_maxHashtagsPerPost == value)
                    return;
                SetProperty(ref _maxHashtagsPerPost, value);
            }
        }
        private string _hashWords;

        [ProtoMember(4)]
        public string HashWords
        {
            get
            {
                return _hashWords;
            }
            set
            {
                if (_hashWords == value)
                    return;
                SetProperty(ref _hashWords, value);
            }
        }
        private int _minimumWordLength;

        [ProtoMember(5)]
        public int MinimumWordLength
        {
            get
            {
                return _minimumWordLength;
            }
            set
            {
                if (_minimumWordLength == value)
                    return;
                SetProperty(ref _minimumWordLength, value);
            }
        }
        private int _replaceProbability;

        [ProtoMember(6)]
        public int ReplaceProbability
        {
            get
            {
                return _replaceProbability;
            }
            set
            {
                if (_replaceProbability == value)
                    return;
                SetProperty(ref _replaceProbability, value);
            }
        }
       
        private bool _isEnableDynamicHashTags;

        [ProtoMember(7)]
        public bool IsEnableDynamicHashTags
        {
            get
            {
                return _isEnableDynamicHashTags;
            }
            set
            {
                if (_isEnableDynamicHashTags == value)
                    return;
                SetProperty(ref _isEnableDynamicHashTags, value);
            }
        }
        private bool _isAddHashTagEvenIfAlreadyHastags;

        [ProtoMember(8)]
        public bool IsAddHashTagEvenIfAlreadyHastags
        {
            get
            {
                return _isAddHashTagEvenIfAlreadyHastags;
            }
            set
            {
                if (_isAddHashTagEvenIfAlreadyHastags == value)
                    return;
                SetProperty(ref _isAddHashTagEvenIfAlreadyHastags, value);
            }
        }
        private RangeUtilities _maxHashtagsPerPostRange = new RangeUtilities();

        [ProtoMember(9)]
        public RangeUtilities MaxHashtagsPerPostRange
        {
            get
            {
                return _maxHashtagsPerPostRange;
            }
            set
            {
                if (_maxHashtagsPerPostRange == value)
                    return;
                SetProperty(ref _maxHashtagsPerPostRange, value);
            }
        }
        private int _pickPercentHashTag;

        [ProtoMember(10)]
        public int PickPercentHashTag
        {
            get
            {
                return _pickPercentHashTag;
            }
            set
            {
                if (_pickPercentHashTag == value)
                    return;
                SetProperty(ref _pickPercentHashTag, value);
            }
        }
        private int _pickPercentFromList;

        [ProtoMember(11)]
        public int PickPercentFromList
        {
            get
            {
                return _pickPercentFromList;
            }
            set
            {
                if (_pickPercentFromList == value)
                    return;
                SetProperty(ref _pickPercentFromList, value);
            }
        }
        private bool _isEnableDynamicMentions;

        [ProtoMember(12)]
        public bool IsEnableDynamicMentions
        {
            get
            {
                return _isEnableDynamicMentions;
            }
            set
            {
                if (_isEnableDynamicMentions == value)
                    return;
                SetProperty(ref _isEnableDynamicMentions, value);
            }
        }
        private bool _isDeleteUsersFromList;

        [ProtoMember(13)]
        public bool IsDeleteUsersFromList
        {
            get
            {
                return _isDeleteUsersFromList;
            }
            set
            {
                if (_isDeleteUsersFromList == value)
                    return;
                SetProperty(ref _isDeleteUsersFromList, value);
            }
        }

        private int _numberOfUsersToMention;

        [ProtoMember(14)]
        public int NumberOfUsersToMention
        {
            get
            {
                return _numberOfUsersToMention;
            }
            set
            {
                if (_numberOfUsersToMention == value)
                    return;
                SetProperty(ref _numberOfUsersToMention, value);
            }
        }
        private string _userNamesSeparatedByComma;

        [ProtoMember(15)]
        public string UserNamesSeparatedByComma
        {
            get
            {
                return _userNamesSeparatedByComma;
            }
            set
            {
                if (_userNamesSeparatedByComma == value)
                    return;
                SetProperty(ref _userNamesSeparatedByComma, value);
            }
        }
        private bool _isRetryFailedVideoPost;

        [ProtoMember(16)]
        public bool IsRetryFailedVideoPost
        {
            get
            {
                return _isRetryFailedVideoPost;
            }
            set
            {
                if (_isRetryFailedVideoPost == value)
                    return;
                SetProperty(ref _isRetryFailedVideoPost, value);
            }
        }
        private bool _isDisableCommentsForNewPost;

        [ProtoMember(17)]
        public bool IsDisableCommentsForNewPost
        {
            get
            {
                return _isDisableCommentsForNewPost;
            }
            set
            {
                if (_isDisableCommentsForNewPost == value)
                    return;
                SetProperty(ref _isDisableCommentsForNewPost, value);
            }
        }
        private bool _isPostMultipleImagesVideoPostsAsAlbum;

        [ProtoMember(18)]
        public bool IsPostMultipleImagesVideoPostsAsAlbum
        {
            get
            {
                return _isPostMultipleImagesVideoPostsAsAlbum;
            }
            set
            {
                if (_isPostMultipleImagesVideoPostsAsAlbum == value)
                    return;
                SetProperty(ref _isPostMultipleImagesVideoPostsAsAlbum, value);
            }
        }
        private string _geoLocation=string.Empty;

        [ProtoMember(19)]
        public string GeoLocation
        {
            get
            {
                return _geoLocation;
            }
            set
            {
                if (_geoLocation == value)
                    return;
                SetProperty(ref _geoLocation, value);
            }
        }
        [ProtoMember(20)]
        public string CampaignId { get; set; }
        private string _hashtagsFromList1;
        [ProtoMember(21)]
        public string HashtagsFromList1
        {
            get
            {
                return _hashtagsFromList1;
            }
            set
            {
                if (_hashtagsFromList1 == value)
                    return;
                SetProperty(ref _hashtagsFromList1, value);
            }
        }

        private string _hashtagsFromList2;
        private bool _isLocationId;
        private bool _isLocationName;

        [ProtoMember(22)]
        public string HashtagsFromList2
        {
            get
            {
                return _hashtagsFromList2;
            }
            set
            {
                if (_hashtagsFromList2 == value)
                    return;
                SetProperty(ref _hashtagsFromList2, value);
            }
        }
        [ProtoMember(23)]
        public bool IsGeoLocationName
        {
            get
            {
                return _isLocationName;
            }
            set
            {
                if (_isLocationName == value)
                    return;

                SetProperty(ref _isLocationName, value);
            }
        }
        [ProtoMember(24)]
        public bool IsGeoLocationId
        {
            get
            {
                return _isLocationId;
            }
            set
            {
                if (_isLocationId == value)
                    return;

                SetProperty(ref _isLocationId, value);
            }
        }
        public InstagramModel Clone()
        {
            return (InstagramModel)MemberwiseClone();
        }
    }
}

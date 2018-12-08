using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
    public class FacebookModel : BindableBase
    {
        private bool _isNavigateFromFacebookWall;

        [ProtoMember(1)]
        public bool IsNavigateFromFacebookWall
        {
            get
            {
                return _isNavigateFromFacebookWall;
            }
            set
            {
                if (_isNavigateFromFacebookWall == value)
                    return;
                SetProperty(ref _isNavigateFromFacebookWall, value);
            }
        }
        private bool _isUseSimplifiedMethodForFacebook;

        [ProtoMember(2)]
        public bool IsUseSimplifiedMethodForFacebook
        {
            get
            {
                return _isUseSimplifiedMethodForFacebook;
            }
            set
            {
                if (_isUseSimplifiedMethodForFacebook == value)
                    return;
                SetProperty(ref _isUseSimplifiedMethodForFacebook, value);
            }
        }
        private bool _isSkipSimplifiedShare;

        [ProtoMember(3)]
        public bool IsSkipSimplifiedShare
        {
            get
            {
                return _isSkipSimplifiedShare;
            }
            set
            {
                if (_isSkipSimplifiedShare == value)
                    return;
                SetProperty(ref _isSkipSimplifiedShare, value);
            }
        }
        private bool _isUncheckIncludeOriginalPost;

        [ProtoMember(4)]
        public bool IsUncheckIncludeOriginalPost
        {
            get
            {
                return _isUncheckIncludeOriginalPost;
            }
            set
            {
                if (_isUncheckIncludeOriginalPost == value)
                    return;
                SetProperty(ref _isUncheckIncludeOriginalPost, value);
            }
        }
        private bool _isRemoveViaoption;

        [ProtoMember(5)]
        public bool IsRemoveViaoption
        {
            get
            {
                return _isRemoveViaoption;
            }
            set
            {
                if (_isRemoveViaoption == value)
                    return;
                SetProperty(ref _isRemoveViaoption, value);
            }
        }
        private bool _isHidePostInFacebook;

        [ProtoMember(6)]
        public bool IsHidePostInFacebook
        {
            get
            {
                return _isHidePostInFacebook;
            }
            set
            {
                if (_isHidePostInFacebook == value)
                    return;
                SetProperty(ref _isHidePostInFacebook, value);
            }
        }
        private bool _isDisableCommentsForNewPost;

        [ProtoMember(7)]
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
        private bool _isTurnOffNotificationsForNewPost;

        [ProtoMember(8)]
        public bool IsTurnOffNotificationsForNewPost
        {
            get
            {
                return _isTurnOffNotificationsForNewPost;
            }
            set
            {
                if (_isTurnOffNotificationsForNewPost == value)
                    return;
                SetProperty(ref _isTurnOffNotificationsForNewPost, value);
            }
        }
        private bool _isRemoveLocationForSellPosts;

        [ProtoMember(9)]
        public bool IsRemoveLocationForSellPosts
        {
            get
            {
                return _isRemoveLocationForSellPosts;
            }
            set
            {
                if (_isRemoveLocationForSellPosts == value)
                    return;
                SetProperty(ref _isRemoveLocationForSellPosts, value);
            }
        }
        private bool _isAllowMaximumOf;

        [ProtoMember(10)]
        public bool IsAllowMaximumOf
        {
            get
            {
                return _isAllowMaximumOf;
            }
            set
            {
                if (_isAllowMaximumOf == value)
                    return;
                SetProperty(ref _isAllowMaximumOf, value);
            }
        }
        private int _maximumOf;

        [ProtoMember(11)]
        public int MaximumOf
        {
            get
            {
                return _maximumOf;
            }
            set
            {
                if (_maximumOf == value)
                    return;
                SetProperty(ref _maximumOf, value);
            }
        }
        private bool _isRefreshPageAndRetryPost;

        [ProtoMember(12)]
        public bool IsRefreshPageAndRetryPost
        {
            get
            {
                return _isRefreshPageAndRetryPost;
            }
            set
            {
                if (_isRefreshPageAndRetryPost == value)
                    return;
                SetProperty(ref _isRefreshPageAndRetryPost, value);
            }
        }
        private bool _isRemoveLinkPreview;

        [ProtoMember(13)]
        public bool IsRemoveLinkPreview
        {
            get
            {
                return _isRemoveLinkPreview;
            }
            set
            {
                if (_isRemoveLinkPreview == value)
                    return;
                SetProperty(ref _isRemoveLinkPreview, value);
            }
        }
        private bool _isSkipImageUpload;

        [ProtoMember(14)]
        public bool IsSkipImageUpload
        {
            get
            {
                return _isSkipImageUpload;
            }
            set
            {
                if (_isSkipImageUpload == value)
                    return;
                SetProperty(ref _isSkipImageUpload, value);
            }
        }
        private bool _isEnableAutomaticHashTags;

        [ProtoMember(15)]
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

        [ProtoMember(16)]
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

        [ProtoMember(17)]
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

        [ProtoMember(18)]
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

        [ProtoMember(19)]
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
        private RangeUtilities _deletePostAfter = new RangeUtilities();

        [ProtoMember(20)]
        public RangeUtilities DeletePostAfter
        {
            get
            {
                return _deletePostAfter;
            }
            set
            {
                if (_deletePostAfter == value)
                    return;
                SetProperty(ref _deletePostAfter, value);
            }
        }
        private bool _isEnableDynamicHashTags;

        [ProtoMember(21)]
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

        [ProtoMember(22)]
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

        [ProtoMember(23)]
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

        [ProtoMember(24)]
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

        [ProtoMember(25)]
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
        private bool _isGlobalSellPostsZipcode;

        [ProtoMember(26)]
        public bool IsGlobalSellPostsZipcode
        {
            get
            {
                return _isGlobalSellPostsZipcode;
            }
            set
            {
                if (_isGlobalSellPostsZipcode == value)
                    return;
                SetProperty(ref _isGlobalSellPostsZipcode, value);
            }
        }
        private RangeUtilities _publishOnBetween = new RangeUtilities();

        [ProtoMember(27)]
        public RangeUtilities PublishOnBetween
        {
            get
            {
                return _publishOnBetween;
            }
            set
            {
                if (_publishOnBetween == value)
                    return;
                SetProperty(ref _publishOnBetween, value);
            }
        }
        private bool _isDeletePostAfter;

        [ProtoMember(28)]
        public bool IsDeletePostAfter
        {
            get
            {
                return _isDeletePostAfter;
            }
            set
            {
                if (_isDeletePostAfter == value)
                    return;
                SetProperty(ref _isDeletePostAfter, value);
            }
        }


        [ProtoMember(29)]
        public string CampaignId { get; set; }
        private string _hashtagsFromList1;
        [ProtoMember(30)]
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

        [ProtoMember(31)]
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
        private bool _isAutoTagFriends;

        [ProtoMember(32)]
        public bool IsAutoTagFriends
        {
            get
            {
                return _isAutoTagFriends;
            }
            set
            {
                if (_isAutoTagFriends == value)
                    return;
                if (value)
                    IsTagOptionChecked = true;
                SetProperty(ref _isAutoTagFriends, value);
            }
        }
        private RangeUtilities _usersForEachPost = new RangeUtilities();

        [ProtoMember(33)]
        public RangeUtilities UsersForEachPost
        {
            get
            {
                return _usersForEachPost;
            }
            set
            {
                if (_usersForEachPost == value)
                    return;
                SetProperty(ref _usersForEachPost, value);
            }
        }

        private bool _isTagUniqueFriends;

        [ProtoMember(34)]
        public bool IsTagUniqueFriends
        {
            get
            {
                return _isTagUniqueFriends;
            }
            set
            {
                if (_isTagUniqueFriends == value)
                    return;
                SetProperty(ref _isTagUniqueFriends, value);
            }
        }

        private List<string> _alreadyTaggedUsers = new List<string>();

        [ProtoMember(35)]
        public List<string> AlreadyTaggedUsers
        {
            get
            {
                return _alreadyTaggedUsers;
            }
            set
            {
                if (_alreadyTaggedUsers == value)
                    return;
                SetProperty(ref _alreadyTaggedUsers, value);
            }
        }

        private List<KeyValuePair<string, string>> _accountFriendsPair = new List<KeyValuePair<string, string>>();


        [ProtoMember(36)]
        public List<KeyValuePair<string, string>> AccountFriendsPair
        {
            get
            {
                return _accountFriendsPair;
            }
            set
            {
                if (_accountFriendsPair == value)
                    return;
                SetProperty(ref _accountFriendsPair, value);
            }
        }


        private bool _isTagSpecificFriends;

        [ProtoMember(37)]
        public bool IsTagSpecificFriends
        {
            get
            {
                return _isTagSpecificFriends;
            }
            set
            {
                if (_isTagSpecificFriends == value)
                    return;
                SetProperty(ref _isTagSpecificFriends, value);
            }
        }


        private string _CustomTaggedUser = string.Empty;

        [ProtoMember(38)]
        public string CustomTaggedUser
        {
            get
            {
                return _CustomTaggedUser;
            }
            set
            {
                if (_CustomTaggedUser == value)
                    return;
                SetProperty(ref _CustomTaggedUser, value);
            }
        }

        private List<string> _ListCustomTaggedUser = new List<string>();

        [ProtoMember(39)]
        public List<string> ListCustomTaggedUser
        {
            get
            {
                return _ListCustomTaggedUser;
            }
            set
            {
                if (_ListCustomTaggedUser == value)
                    return;
                SetProperty(ref _ListCustomTaggedUser, value);
            }
        }

        private SelectAccountDetailsModel _selectFriendsDetailsModel = new SelectAccountDetailsModel();

        [ProtoMember(40)]
        public SelectAccountDetailsModel SelectFriendsDetailsModel
        {
            get
            {
                return _selectFriendsDetailsModel;
            }
            set
            {
                if (_selectFriendsDetailsModel == value)
                    return;
                SetProperty(ref _selectFriendsDetailsModel, value);
            }
        }

        private bool _isPostAsPage;

        [ProtoMember(41)]
        public bool IsPostAsPage
        {
            get
            {
                return _isPostAsPage;
            }
            set
            {
                if (_isPostAsPage == value)
                    return;
                SetProperty(ref _isPostAsPage, value);
            }
        }

        private bool _isPostAsOwnAccount = true;

        [ProtoMember(42)]
        public bool IsPostAsOwnAccount
        {
            get
            {
                return _isPostAsOwnAccount;
            }
            set
            {
                if (_isPostAsOwnAccount == value)
                    return;
                SetProperty(ref _isPostAsOwnAccount, value);
            }
        }


        private bool _isReactAsPageOptionChecked;

        [ProtoMember(43)]
        public bool IsReactAsPageOptionChecked
        {
            get
            {
                return _isReactAsPageOptionChecked;
            }
            set
            {
                if (_isReactAsPageOptionChecked == value)
                    return;
                SetProperty(ref _isReactAsPageOptionChecked, value);
            }
        }


        private string _CustomPageUrl = string.Empty;

        [ProtoMember(44)]
        public string CustomPageUrl
        {
            get
            {
                return _CustomPageUrl;
            }
            set
            {
                if (_CustomPageUrl == value)
                    return;
                SetProperty(ref _CustomPageUrl, value);
            }
        }

        private List<string> _ListCustomPageUrl = new List<string>();

        [ProtoMember(45)]
        public List<string> ListCustomPageUrl
        {
            get
            {
                return _ListCustomPageUrl;
            }
            set
            {
                if (_ListCustomPageUrl == value)
                    return;
                SetProperty(ref _ListCustomPageUrl, value);
            }
        }

        private SelectAccountDetailsModel _selectPageDetailsModel = new SelectAccountDetailsModel();

        [ProtoMember(46)]
        public SelectAccountDetailsModel SelectPageDetailsModel
        {
            get
            {
                return _selectPageDetailsModel;
            }
            set
            {
                if (_selectPageDetailsModel == value)
                    return;
                SetProperty(ref _selectPageDetailsModel, value);
            }
        }


        private bool _IsTagOptionChecked;

        [ProtoMember(47)]
        public bool IsTagOptionChecked
        {
            get
            {
                return _IsTagOptionChecked;
            }
            set
            {
                if (_IsTagOptionChecked == value)
                    return;
                SetProperty(ref _IsTagOptionChecked, value);
            }
        }

        private List<KeyValuePair<string, string>> _AccountPagesBoardsPair = new List<KeyValuePair<string, string>>();

        [ProtoMember(48)]
        public List<KeyValuePair<string, string>> AccountMentionPair
        {
            get
            {
                return _AccountMentionPair;
            }
            set
            {
                if (_AccountMentionPair == value)
                    return;
                SetProperty(ref _AccountMentionPair, value);
            }
        }


        private string _CustomMentionUser = string.Empty;

        [ProtoMember(49)]
        public string CustomMentionUser
        {
            get
            {
                return _CustomMentionUser;
            }
            set
            {
                if (_CustomMentionUser == value)
                    return;
                SetProperty(ref _CustomMentionUser, value);
            }
        }


        private List<string> _ListCustomMentionUser = new List<string>();

        [ProtoMember(50)]
        public List<string> ListCustomMentionUser
        {
            get
            {
                return _ListCustomMentionUser;
            }
            set
            {
                if (_ListCustomMentionUser == value)
                    return;
                SetProperty(ref _ListCustomMentionUser, value);
            }
        }



        private bool _IsMentionSpecificFriends;

        [ProtoMember(51)]
        public bool IsMentionSpecificFriends
        {
            get
            {
                return _IsMentionSpecificFriends;
            }
            set
            {
                if (_IsMentionSpecificFriends == value)
                    return;
                SetProperty(ref _IsMentionSpecificFriends, value);
            }
        }

        private SelectAccountDetailsModel _selectFriendsDetailsModelForMention = new SelectAccountDetailsModel();

        [ProtoMember(52)]
        public SelectAccountDetailsModel SelectFriendsDetailsModelForMention
        {
            get
            {
                return _selectFriendsDetailsModelForMention;
            }
            set
            {
                if (_selectFriendsDetailsModelForMention == value)
                    return;
                SetProperty(ref _selectFriendsDetailsModelForMention, value);
            }
        }


        private List<KeyValuePair<string, string>> _AccountMentionPair = new List<KeyValuePair<string, string>>();

        [ProtoMember(53)]
        public List<KeyValuePair<string, string>> AccountPagesBoardsPair
        {
            get
            {
                return _AccountPagesBoardsPair;
            }
            set
            {
                if (_AccountPagesBoardsPair == value)
                    return;
                SetProperty(ref _AccountPagesBoardsPair, value);
            }
        }


        private RangeUtilities _MentionUsersForEachPost = new RangeUtilities(2, 5);

        [ProtoMember(54)]
        public RangeUtilities MentionUsersForEachPost
        {
            get
            {
                return _MentionUsersForEachPost;
            }
            set
            {
                if (_MentionUsersForEachPost == value)
                    return;
                SetProperty(ref _MentionUsersForEachPost, value);
            }
        }


        private bool _isPostAsSamePage;

        [ProtoMember(55)]
        public bool IsPostAsSamePage
        {
            get
            {
                return _isPostAsSamePage;
            }
            set
            {
                if (_isPostAsSamePage == value)
                    return;
                SetProperty(ref _isPostAsSamePage, value);
                if (value)
                    IsPostAsOwnAccount = false;
            }
        }


        public FacebookModel Clone()
        {
            return (FacebookModel)MemberwiseClone();
        }
    }

}

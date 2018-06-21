using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using ProtoBuf;

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
        private RangeUtilities _maxHashtagsPerPostRange=new RangeUtilities();

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


        public FacebookModel Clone()
        {
            return (FacebookModel)MemberwiseClone();
        }
    }

}

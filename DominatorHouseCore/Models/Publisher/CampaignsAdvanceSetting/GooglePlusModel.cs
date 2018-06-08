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
    public class GooglePlusModel : BindableBase
    {
        #region Common
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

        private bool _isAddOnePostAfterPublishingOnGooglePlus;

        [ProtoMember(7)]
        public bool IsAddOnePostAfterPublishingOnGooglePlus
        {
            get
            {
                return _isAddOnePostAfterPublishingOnGooglePlus;
            }
            set
            {
                if (_isAddOnePostAfterPublishingOnGooglePlus == value)
                    return;
                SetProperty(ref _isAddOnePostAfterPublishingOnGooglePlus, value);
            }
        }
        private bool _isSkipImageUpload;

        [ProtoMember(8)]
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
        private bool _isDeletePostAfter;

        [ProtoMember(9)]
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
        private RangeUtilities _deletePostAfter = new RangeUtilities();

        [ProtoMember(10)]
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

        #endregion
    }
}

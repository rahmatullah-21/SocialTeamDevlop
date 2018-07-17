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
    public class TumblrModel : BindableBase
    {
        private bool _isRemoveTagsFromPostText;

        [ProtoMember(1)]
        public bool IsRemoveTagsFromPostText
        {
            get
            {
                return _isRemoveTagsFromPostText;
            }
            set
            {
                if (_isRemoveTagsFromPostText == value)
                    return;
                SetProperty(ref _isRemoveTagsFromPostText, value);
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
       
        [ProtoMember(12)]
        public string CampaignId { get; set; }
        private string _hashtagsFromList1;
        [ProtoMember(13)]
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

        [ProtoMember(14)]
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
        public TumblrModel Clone()
        {
            return (TumblrModel) MemberwiseClone();
        }
    }

}

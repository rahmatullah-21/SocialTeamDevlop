using System.Collections.Generic;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher
{
    [ProtoContract]
    public class OtherConfigurationModel : BindableBase
    {
        /// <summary>
        /// To specify the whether to use the signature
        /// </summary>
        private bool _isEnableSignatureChecked;
        [ProtoMember(2)]
        public bool IsEnableSignatureChecked
        {
            get
            {
                return _isEnableSignatureChecked;
            }
            set
            {
                if (value == _isEnableSignatureChecked)
                    return;
                SetProperty(ref _isEnableSignatureChecked, value);
            }
        }

        /// <summary>
        /// To specify the shorten url for link
        /// </summary>
        private bool _isShortenURLsChecked;
        [ProtoMember(3)]
        public bool IsShortenURLsChecked
        {
            get
            {
                return _isShortenURLsChecked;
            }
            set
            {
                if (value == _isShortenURLsChecked)
                    return;
                SetProperty(ref _isShortenURLsChecked, value);
            }
        }


        /// <summary>
        /// To specify the publishing post with texts and link.Its only for Pinterest
        /// </summary>
        private bool _isAllowPublishingPinterestChecked;
        [ProtoMember(5)]
        public bool IsAllowPublishingPinterestChecked
        {
            get
            {
                return _isAllowPublishingPinterestChecked;
            }
            set
            {
                if (value == _isAllowPublishingPinterestChecked)
                    return;
                SetProperty(ref _isAllowPublishingPinterestChecked, value);
            }
        }

        /// <summary>
        /// To publish as story instead of own wall. Its only for instagram
        /// </summary>
        private bool _isPostAsStoryChecked;
        [ProtoMember(6)]
        public bool IsPostAsStoryChecked
        {
            get
            {
                return _isPostAsStoryChecked;
            }
            set
            {
                if (value == _isPostAsStoryChecked)
                    return;
                SetProperty(ref _isPostAsStoryChecked, value);
            }
        }
       
        /// <summary>
        /// Adding signature to your post descriptions
        /// </summary>
        private string _signatureText;
        [ProtoMember(10)]
        public string SignatureText
        {
            get { return _signatureText; }
            set
            {
                if (value == _signatureText)
                    return;
                SetProperty(ref _signatureText, value);
            }
        }


        #region Not Used

        private string _campaigntTag;
        [ProtoMember(1)]
        public string CampaignTag
        {
            get
            {
                return _campaigntTag;
            }
            set
            {
                if (value == _campaigntTag)
                    return;
                SetProperty(ref _campaigntTag, value);
            }
        }



        private bool _isPostTextChecked;
        [ProtoMember(4)]
        public bool IsPostTextChecked
        {
            get
            {
                return _isPostTextChecked;
            }
            set
            {
                if (value == _isPostTextChecked)
                    return;
                SetProperty(ref _isPostTextChecked, value);
            }
        }



        private List<string> _makeImagesUniqueStatus = new List<string>();

        public List<string> MakeImagesUniqueStatus
        {
            get { return _makeImagesUniqueStatus; }
            set
            {
                if (value == _makeImagesUniqueStatus)
                    return;
                SetProperty(ref _makeImagesUniqueStatus, value);
            }
        }


        private bool _isEnableCustomTokensChecked;
        [ProtoMember(9)]
        public bool IsEnableCustomTokensChecked
        {
            get
            {
                return _isEnableCustomTokensChecked;
            }
            set
            {
                if (value == _isEnableCustomTokensChecked)
                    return;
                SetProperty(ref _isEnableCustomTokensChecked, value);
            }
        }


        private bool _isEnableWatermarkChecked;
        [ProtoMember(8)]
        public bool IsEnableWatermarkChecked
        {
            get
            {
                return _isEnableWatermarkChecked;
            }
            set
            {
                if (value == _isEnableWatermarkChecked)
                    return;
                SetProperty(ref _isEnableWatermarkChecked, value);
            }
        }


        private bool _isMakeImagesUniqueChecked;
        [ProtoMember(7)]
        public bool IsMakeImagesUniqueChecked
        {
            get
            {
                return _isMakeImagesUniqueChecked;
            }
            set
            {
                if (value == _isMakeImagesUniqueChecked)
                    return;
                SetProperty(ref _isMakeImagesUniqueChecked, value);
            }
        }



        #endregion
    }
}

using System;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Enums.SocioPublisher;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PostDetailsModel : BindableBase
    {
        /// <summary>
        /// To specify the Post Description
        /// </summary>
        private string _postDescription =string.Empty;
        [ProtoMember(1)]
        public string PostDescription
        {
            get
            {
                return _postDescription;
            }
            set
            {
                if (_postDescription == value)
                    return;
                _postDescription = value;
                OnPropertyChanged(nameof(PostDescription));
            }
        }

        /// <summary>
        /// To specify whether multiple post
        /// </summary>
        private bool _isMultiPost;
        [ProtoMember(2)]
        public bool IsMultiPost
        {
            get
            {
                return _isMultiPost;
            }
            set
            {
                if (value == _isMultiPost)
                    return;
                SetProperty(ref _isMultiPost, value);
            }
        }


        /// <summary>
        /// To specify whether multiple image posts
        /// </summary>
        private bool _isMultipleImagePost;
        [ProtoMember(3)]
        public bool IsMultipleImagePost
        {
            get
            {
                return _isMultipleImagePost;
            }
            set
            {
                if (value == _isMultipleImagePost)
                    return;
                SetProperty(ref _isMultipleImagePost, value);
            }
        }

        /// <summary>
        /// Is need to use file name as a description for multiple image posts
        /// </summary>
        private bool _isUseFileNameAsDescription;
        [ProtoMember(4)]
        public bool IsUseFileNameAsDescription
        {
            get
            {
                return _isUseFileNameAsDescription;
            }
            set
            {
                if (value == _isUseFileNameAsDescription)
                    return;
                SetProperty(ref _isUseFileNameAsDescription, value);
            }
        }

        /// <summary>
        /// Is need to add only unique post for multiple images
        /// </summary>
        private bool _isUniquePost;
        [ProtoMember(5)]
        public bool IsUniquePost
        {
            get
            {
                return _isUniquePost;
            }
            set
            {
                if (value == _isUniquePost)
                    return;
                SetProperty(ref _isUniquePost, value);
            }
        }

        /// <summary>
        /// Image url for multiple image post
        /// </summary>
        private string _imagesUrl=string.Empty;
        [ProtoMember(6)]
        public string ImagesUrl
        {
            get
            {
                return _imagesUrl;
            }
            set
            {
                if (value == _imagesUrl)
                    return;
                SetProperty(ref _imagesUrl, value);
            }
        }


        /// <summary>
        /// Title of the post
        /// </summary>
        private string _publisherInstagramTitle =string.Empty;
        [ProtoMember(7)]
        public string PublisherInstagramTitle
        {
            get
            {
                return _publisherInstagramTitle;
            }
            set
            {
                if (_publisherInstagramTitle == value)
                    return;
                _publisherInstagramTitle = value;
                OnPropertyChanged(nameof(_publisherInstagramTitle));
            }
        }

        /// <summary>
        /// Facebook Sell post Title
        /// </summary>
        private string _fdSellProductTitle;
        [ProtoMember(8)]
        public string FdSellProductTitle
        {
            get
            {
                return _fdSellProductTitle;
            }
            set
            {
                if (value == _fdSellProductTitle)
                    return;
                SetProperty(ref _fdSellProductTitle, value);
            }
        }

        /// <summary>
        /// Facebook Sell price for the post 
        /// </summary>

        private double _fdSellPrice;
        [ProtoMember(9)]
        public double FdSellPrice
        {
            get
            {
                return _fdSellPrice;
            }
            set
            {
                if (value == _fdSellPrice)
                    return;
                SetProperty(ref _fdSellPrice, value);
            }
        }

        /// <summary>
        /// Facebook sell post available locaion
        /// </summary>
        private string _fdSellLocation;
        [ProtoMember(10)]
        public string FdSellLocation
        {
            get
            {
                return _fdSellLocation;
            }
            set
            {
                if (value == _fdSellLocation)
                    return;
                SetProperty(ref _fdSellLocation, value);
            }
        }
       
        /// <summary>
        /// Media Viewer Details
        /// </summary>
        private PublisherMediaViewerModel _mediaViewer =new PublisherMediaViewerModel();
        [ProtoMember(11)]
        public PublisherMediaViewerModel MediaViewer
        {
            get
            {
                return _mediaViewer;
            }
            set
            {            
                if (_mediaViewer == value)
                    return;
                _mediaViewer = value;
                OnPropertyChanged(nameof(MediaViewer));
            }
        }


        /// <summary>
        /// To Specify the settings for the posts
        /// </summary>
        private PublisherPostSettings _publisherPostSettings = new PublisherPostSettings();
        [ProtoMember(12)]
        public PublisherPostSettings PublisherPostSettings
        {
            get
            {
                return _publisherPostSettings;
            }
            set
            {
                if (_publisherPostSettings == value)
                    return;
                _publisherPostSettings = value;
                OnPropertyChanged(nameof(PublisherPostSettings));
            }
        }

        /// <summary>
        /// Is a single or direct post
        /// </summary>
        private bool _isSinglePost=true;
        [ProtoMember(13)]
        public bool IsSinglePost
        {
            get
            {
                return _isSinglePost;
            }
            set
            {
                if (value == _isSinglePost)
                    return;
                SetProperty(ref _isSinglePost, value);
            }
        }

        /// <summary>
        /// Is Facebook sell post
        /// </summary>
        private bool _isFdSellPost;
        [ProtoMember(14)]
        public bool IsFdSellPost
        {
            get
            {
                return _isFdSellPost;
            }
            set
            {
                if (value == _isFdSellPost)
                    return;
                SetProperty(ref _isFdSellPost, value);
            }
        }

        /// <summary>
        /// Soruce url
        /// </summary>

        private string _pdSourceUrl=string.Empty;
        [ProtoMember(15)]
        public string PdSourceUrl
        {
            get
            {
                return _pdSourceUrl;
            }
            set
            {
                if (value == _pdSourceUrl)
                    return;
                SetProperty(ref _pdSourceUrl, value);
            }
        }


        /// <summary>
        /// To Specify the Media lists
        /// </summary>
        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();
       
        [ProtoMember(16)]
        public ObservableCollection<string> MediaList
        {
            get
            {
                return _mediaList;
            }
            set
            {
                if (value == _mediaList)
                    return;
                SetProperty(ref _mediaList, value);

            }
        }

        /// <summary>
        /// To Specify the created date time 
        /// </summary>
        private DateTime _createdDateTime;
        [ProtoMember(17)]
        public DateTime CreatedDateTime
        {
            get
            {
                return _createdDateTime;
            }
            set
            {              
                if (value == _createdDateTime)
                    return;
                SetProperty(ref _createdDateTime, value);
            }
        }

        /// <summary>
        /// To specify the post ID
        /// </summary>
        private string _postDetailsId;
        [ProtoMember(18)]
        public string PostDetailsId
        {
            get
            {
                return _postDetailsId;
            }
            set
            {
                if (value == _postDetailsId)
                    return;
                SetProperty(ref _postDetailsId, value);
            }
        }

        /// <summary>
        /// To Specify the post queued status
        /// </summary>
        private PostQueuedStatus _postQueuedStatus = PostQueuedStatus.Pending;
        [ProtoMember(19)]
        public PostQueuedStatus PostQueuedStatus
        {
            get
            {
                return _postQueuedStatus;
            }
            set
            {
                if (value == _postQueuedStatus)
                    return;
                SetProperty(ref _postQueuedStatus, value);
            }
        }

    }
}
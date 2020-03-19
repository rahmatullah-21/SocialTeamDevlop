using System;
using System.Collections.ObjectModel;
using System.Threading;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublisherPostlistModel : BindableBase
    {
        #region Properties

        private string _postDescription = string.Empty;
        /// <summary>
        /// To describe the post data
        /// </summary>
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


        private DateTime _createdTime;

        /// <summary>
        /// To specify the post created date time
        /// </summary>
        [ProtoMember(2)]
        public DateTime CreatedTime
        {
            get
            {
                return _createdTime;
            }
            set
            {
                if (_createdTime == value)
                    return;
                _createdTime = value;
                OnPropertyChanged(nameof(CreatedTime));
            }
        }


        private PostSource _postSource = PostSource.NormalPost;

        /// <summary>
        /// To specify from where the post comes
        /// </summary>
        [ProtoMember(3)]
        public PostSource PostSource
        {
            get
            {
                return _postSource;
            }
            set
            {
                if (_postSource == value)
                    return;
                _postSource = value;
                OnPropertyChanged(nameof(PostSource));
            }
        }


        private PostCategory _postCategory;

        /// <summary>
        /// To specify the post category
        /// </summary>
        [ProtoMember(4)]
        public PostCategory PostCategory
        {
            get
            {
                return _postCategory;
            }
            set
            {
                if (_postCategory == value)
                    return;
                _postCategory = value;
                OnPropertyChanged(nameof(PostCategory));
            }
        }

        private string _campaignId = string.Empty;

        /// <summary>
        /// To specify the post category
        /// </summary>
        [ProtoMember(5)]
        public string CampaignId
        {
            get
            {
                return _campaignId;
            }
            set
            {
                if (_campaignId == value)
                    return;
                _campaignId = value;
                OnPropertyChanged(nameof(CampaignId));
            }
        }


        private PostRunningStatus _postRunningStatus = PostRunningStatus.Active;

        /// <summary>
        /// To specify the post running status whether active or completed
        /// </summary>
        [ProtoMember(6)]
        public PostRunningStatus PostRunningStatus
        {
            get { return _postRunningStatus; }
            set
            {
                if (_postRunningStatus == value)
                    return;
                _postRunningStatus = value;
                OnPropertyChanged(nameof(PostRunningStatus));
            }
        }

        private PostQueuedStatus _postQueuedStatus;
        /// <summary>
        /// To specify the post queued status whether pending, draft or published
        /// </summary>
        [ProtoMember(7)]
        public PostQueuedStatus PostQueuedStatus
        {
            get
            {
                return _postQueuedStatus;
            }
            set
            {
                if (_postQueuedStatus == value)
                    return;
                _postQueuedStatus = value;
                OnPropertyChanged(nameof(PostQueuedStatus));
            }
        }


        private DateTime? _expiredTime;
        /// <summary>
        /// To specify the post expired time
        /// </summary>
        [ProtoMember(8)]
        public DateTime? ExpiredTime
        {
            get
            {
                return _expiredTime;
            }
            set
            {
                if (_expiredTime == value)
                    return;
                _expiredTime = value;
                OnPropertyChanged(nameof(ExpiredTime));
            }
        }


        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();
        /// <summary>
        /// To hold the image or video file 
        /// </summary>
        [ProtoMember(9)]
        public ObservableCollection<string> MediaList
        {
            get
            {
                return _mediaList;
            }
            set
            {
                SetProperty(ref _mediaList, value);
            }
        }

        private string _postId = Utilities.GetGuid();

        /// <summary>
        /// To specify the post id
        /// </summary>
        [ProtoMember(10)]
        public string PostId
        {
            get
            {
                return _postId;
            }
            set
            {
                if (_postId == value)
                    return;
                _postId = value;
                OnPropertyChanged(nameof(PostId));
            }
        }


        private bool _isPostlistSelected;
        /// <summary>
        /// To specify the post list is selected or not
        /// </summary>
        [ProtoIgnore]
        public bool IsPostlistSelected
        {
            get
            {
                return _isPostlistSelected;
            }
            set
            {
                if (_isPostlistSelected == value)
                    return;
                _isPostlistSelected = value;
                OnPropertyChanged(nameof(IsPostlistSelected));
            }
        }

        private string _shareUrl = string.Empty;
        [ProtoMember(21)]
        public string ShareUrl
        {
            get
            {
                return _shareUrl;
            }
            set
            {
                _shareUrl = value;
                if (_shareUrl == value)
                    return;
                _shareUrl = value;
                OnPropertyChanged(nameof(ShareUrl));
            }
        }


        private bool _isFdSellPost;
        [ProtoMember(22)]
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
        private ObservableCollection<PublishedPostDetailsModel> _lstPublishedPostDetailsModels = new ObservableCollection<PublishedPostDetailsModel>();
        [ProtoMember(23)]
        public ObservableCollection<PublishedPostDetailsModel> LstPublishedPostDetailsModels
        {
            get
            {
                return _lstPublishedPostDetailsModels;
            }
            set
            {
                if (value == _lstPublishedPostDetailsModels)
                    return;
                SetProperty(ref _lstPublishedPostDetailsModels, value);
            }
        }


        private string _fetchedPostId = string.Empty;
        [ProtoMember(25)]
        public string FetchedPostIdOrUrl
        {
            get
            {
                return _fetchedPostId;
            }
            set
            {
                if (_fetchedPostId == value)
                    return;
                SetProperty(ref _fetchedPostId, value);
            }
        }


        private string _publishedTriedAndSuccessStatus = "00/00";
        [ProtoMember(26)]
        public string PublishedTriedAndSuccessStatus
        {
            get
            {
                return _publishedTriedAndSuccessStatus;
            }
            set
            {
                if (_publishedTriedAndSuccessStatus == value)
                    return;
                SetProperty(ref _publishedTriedAndSuccessStatus, value);
            }
        }

        private string _monitorFilePath = string.Empty;
        [ProtoMember(28)]
        public string MonitorFilePath
        {
            get
            {
                return _monitorFilePath;
            }
            set
            {

                if (_monitorFilePath == value)
                    return;
                SetProperty(ref _monitorFilePath, value);
            }
        }

        private bool _isSpinTax;
        [ProtoMember(30)]
        public bool IsSpinTax
        {
            get
            {
                return _isSpinTax;
            }
            set
            {
                if (value == _isSpinTax)
                    return;
                SetProperty(ref _isSpinTax, value);
            }
        }
        #region Postlist




        private string _currentMediaUrl = string.Empty;
        public string CurrentMediaUrl
        {
            get
            {
                return _currentMediaUrl;
            }
            set
            {
                if (_currentMediaUrl == value)
                    return;
                _currentMediaUrl = value;
                OnPropertyChanged(nameof(CurrentMediaUrl));
            }
        }


        private bool _nextImageEnable;
        public bool NextImageEnable
        {
            get
            {
                return _nextImageEnable;
            }
            set
            {
                if (_nextImageEnable == value)
                    return;
                _nextImageEnable = value;
                OnPropertyChanged(nameof(NextImageEnable));
            }
        }


        private bool _previousImageEnable;
        public bool PreviousImageEnable
        {
            get
            {
                return _previousImageEnable;
            }
            set
            {
                if (_previousImageEnable == value)
                    return;
                _previousImageEnable = value;
                OnPropertyChanged(nameof(PreviousImageEnable));
            }
        }


        private int _mediaCurrentPointer;
        public int MediaCurrentPointer
        {
            get
            {
                return _mediaCurrentPointer;
            }
            set
            {
                if (_mediaCurrentPointer == value)
                    return;
                _mediaCurrentPointer = value;
                OnPropertyChanged(nameof(MediaCurrentPointer));
            }
        }

        private int _imagePointer;
        public int ImagePointer
        {
            get
            {
                return _imagePointer;
            }
            set
            {
                if (_imagePointer == value)
                    return;
                _imagePointer = value;
                OnPropertyChanged(nameof(ImagePointer));
            }
        }

        private int _totalMediaCount;
        public int TotalMediaCount
        {
            get
            {
                return _totalMediaCount;
            }
            set
            {
                if (_totalMediaCount == value)
                    return;
                _totalMediaCount = value;
                OnPropertyChanged(nameof(TotalMediaCount));
            }
        }

        private bool _isPostListPresent = true;
        public bool IsPostListPresent
        {
            get
            {
                return _isPostListPresent;
            }
            set
            {
                if (_isPostListPresent == value)
                    return;
                _isPostListPresent = value;
                OnPropertyChanged(nameof(IsPostListPresent));
            }
        }

        public bool CanPostForNetwork { get; set; } = true;

        #endregion

        #region Settings

        private GeneralPostSettings _generalPostSettings;
        [ProtoMember(27)]
        public GeneralPostSettings GeneralPostSettings
        {
            get
            {
                return _generalPostSettings;
            }
            set
            {
                if (_generalPostSettings == value)
                    return;
                SetProperty(ref _generalPostSettings, value);
            }
        }



        private FdPostSettings _fdPostSettings = new FdPostSettings();

        [ProtoMember(11)]
        public FdPostSettings FdPostSettings
        {
            get
            {
                return _fdPostSettings;
            }
            set
            {
                if (_fdPostSettings == value)
                    return;
                SetProperty(ref _fdPostSettings, value);
            }
        }


        private GdPostSettings _gdPostSettings = new GdPostSettings();
        [ProtoMember(12)]
        public GdPostSettings GdPostSettings
        {
            get
            {
                return _gdPostSettings;
            }
            set
            {
                if (_gdPostSettings == value)
                    return;

                SetProperty(ref _gdPostSettings, value);
            }
        }


        private TdPostSettings _tdPostSettings = new TdPostSettings();
        [ProtoMember(13)]
        public TdPostSettings TdPostSettings
        {
            get
            {
                return _tdPostSettings;
            }
            set
            {
                if (_tdPostSettings == value)
                    return;

                SetProperty(ref _tdPostSettings, value);
            }
        }

        private LdPostSettings _ldPostSettings = new LdPostSettings();
        [ProtoMember(14)]
        public LdPostSettings LdPostSettings
        {
            get
            {
                return _ldPostSettings;
            }
            set
            {
                if (_ldPostSettings == value)
                    return;

                SetProperty(ref _ldPostSettings, value);
            }
        }

        private RedditPostSetting _redditPostSetting = new RedditPostSetting();
        [ProtoMember(24)]
        public RedditPostSetting RedditPostSetting
        {
            get
            {
                return _redditPostSetting;
            }
            set
            {

                if (_redditPostSetting == value)
                    return;
                SetProperty(ref _redditPostSetting, value);
            }
        }

        private TumberPostSettings _tumberPostSettings = new TumberPostSettings();
        [ProtoMember(15)]
        public TumberPostSettings TumberPostSettings
        {
            get
            {
                return _tumberPostSettings;
            }
            set
            {
                if (_tumberPostSettings == value)
                    return;

                SetProperty(ref _tumberPostSettings, value);
            }
        }
        private string _fdSellProductTitle;
        [ProtoMember(16)]
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

        private double _fdSellPrice;
        [ProtoMember(17)]
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
        private string _fdSellLocation;
        [ProtoMember(18)]
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
        private string _publisherInstagramTitle;
        [ProtoMember(19)]
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
                OnPropertyChanged(nameof(PublisherInstagramTitle));
            }
        }
        private string _pdSourceUrl;
        private PublisherPostSettings _publisherPostSettings = new PublisherPostSettings();

        [ProtoMember(20)]
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

        [ProtoMember(29)]
        public PublisherPostSettings PublisherPostSettings
        {
            get
            {
                return _publisherPostSettings;
            }
            set
            {
                if (value == _publisherPostSettings)
                    return;
                SetProperty(ref _publisherPostSettings, value);
            }
        }
        private bool _isChangeHashOfMedia;
        [ProtoMember(35)]
        public bool IsChangeHashOfMedia
        {
            get { return _isChangeHashOfMedia; }
            set { SetProperty(ref _isChangeHashOfMedia, value); }
        }

        private string _redditScrapedMediaType;
        [ProtoMember(36)]
        public string RedditScrapedMediaType
        {
            get { return _redditScrapedMediaType; }
            set
            {
                if (_redditScrapedMediaType == value)
                    return;
                _redditScrapedMediaType = value;
                OnPropertyChanged(nameof(RedditScrapedMediaType));
            }
        }

        private string _redditScrapedVideoUrl;
        [ProtoMember(37)]
        public string RedditScrapedVideoUrl
        {
            get { return _redditScrapedVideoUrl; }
            set
            {
                if (_redditScrapedVideoUrl == value)
                    return;
                _redditScrapedVideoUrl = value;
                OnPropertyChanged(nameof(RedditScrapedVideoUrl));
            }
        }
        #endregion

        #endregion

        #region Methods

        public void GenerateClonePostId()
        {
            PostId = Utilities.GetGuid();
            CreatedTime = DateTime.Now;
        }

        public void GenerateNewPostId()
        {
            PostId = Utilities.GetGuid();
        }

        /// <summary>
        /// Update the posts navigation details based on media list
        /// </summary>
        public void InitializePostData()
        {
            try
            {
                // Check whether media list contains items or not
                IsPostListPresent = MediaList.Count > 0;

                // Update the process
                if (IsPostListPresent)
                {
                    ImagePointer = 0;
                    MediaCurrentPointer = 1;
                    var mediaUtilites = new MediaUtilites();

                    CurrentMediaUrl = mediaUtilites.GetThumbnail(MediaList[ImagePointer]);
                    TotalMediaCount = MediaList.Count;
                    NextImageEnable = (TotalMediaCount - ImagePointer) > -1;
                    PreviousImageEnable = ImagePointer > 0;
                }
                else
                {
                    ImagePointer = 0;
                    MediaCurrentPointer = 0;
                    CurrentMediaUrl = string.Empty;
                    TotalMediaCount = MediaList.Count;
                    NextImageEnable = false;
                    PreviousImageEnable = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateNavigationPointer()
        {
            NextImageEnable = (TotalMediaCount - MediaCurrentPointer) > 0;
            PreviousImageEnable = ImagePointer > 0;
        }


        #endregion
    }
}
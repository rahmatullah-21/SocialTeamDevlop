using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.SocioPublisher;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherPostlistModel : BindableBase
    {
        private string _postDescription;
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

        private string _title;
        /// <summary>
        /// To describe the post title
        /// </summary>
        [ProtoMember(2)]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }


        private ObservableCollection<string> _mediaList = new ObservableCollection<string>();

        /// <summary>
        /// To hold the image or video file 
        /// </summary>
        [ProtoMember(3)]
        public ObservableCollection<string> MediaList
        {
            get
            {
                return _mediaList;
            }
            set
            {
                if (_mediaList == value)
                    return;
                _mediaList = value;
                OnPropertyChanged(nameof(MediaList));              
            }
        }

        private DateTime _createdTime;

        /// <summary>
        /// To specify the post created date time
        /// </summary>
        [ProtoMember(4)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
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



        private string _campaignId;

        /// <summary>
        /// To specify the post category
        /// </summary>
        [ProtoMember(7)]
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

        private int _sellPostPrice;
        /// <summary>
        /// To specify the sell post price, its only for facebook network
        /// </summary>
        [ProtoMember(8)]
        public int SellPostPrice
        {
            get
            {
                return _sellPostPrice;
            }
            set
            {
                if (_sellPostPrice == value)
                    return;
                _sellPostPrice = value;
                OnPropertyChanged(nameof(SellPostPrice));
            }
        }

        private string _sellPostAvailableLocation;

        /// <summary>
        /// To specify the sell post location, its only for facebook network
        /// </summary>
        [ProtoMember(9)]
        public string SellPostAvailableLocation
        {
            get
            {
                return _sellPostAvailableLocation;
            }
            set
            {
                if (_sellPostAvailableLocation == value)
                    return;
                _sellPostAvailableLocation = value;
                OnPropertyChanged(nameof(SellPostAvailableLocation));
            }
        }

        private Dictionary<string, string> _postMacros = new Dictionary<string, string>();
        /// <summary>
        /// To specify the post macros
        /// </summary>
        [ProtoMember(10)]
        public Dictionary<string, string> PostMacro
        {
            get
            {
                return _postMacros;
            }
            set
            {
                if (_postMacros == value)
                    return;
                _postMacros = value;
                OnPropertyChanged(nameof(PostMacro));
            }
        }

        private PostRunningStatus _postRunningStatus = PostRunningStatus.Active;

        /// <summary>
        /// To specify the post running status whether active or completed
        /// </summary>
        [ProtoMember(11)]
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
        [ProtoMember(12)]
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


        private DateTime _expiredTime;
        /// <summary>
        /// To specify the post expired time
        /// </summary>
        [ProtoMember(13)]
        public DateTime ExpiredTime
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

        private PublisherMediaViewerViewModel _publisherMediaViewerViewModel = new PublisherMediaViewerViewModel();
        [ProtoIgnore]
        public PublisherMediaViewerViewModel PublisherMediaViewerViewModel
        {
            get
            {
                return _publisherMediaViewerViewModel;
            }
            set
            {
                if (_publisherMediaViewerViewModel == value)
                    return;
                _publisherMediaViewerViewModel = value;
                OnPropertyChanged(nameof(PublisherMediaViewerModel));
            }
        }
    }
}
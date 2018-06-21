using System.Collections.ObjectModel;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using  System.Windows;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PostDetailsModel : BindableBase
    {

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
        private string _publisherInstagramTitle ;
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
        private bool _isSinglePost;
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

        private string _pdSourceUrl;
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
    }
}
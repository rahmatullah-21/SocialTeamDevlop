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
        private string _imagesUrl;
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

        private PublisherMediaViewerModel _mediaViewer =new PublisherMediaViewerModel();
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
    }
}
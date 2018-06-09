using System.Collections.ObjectModel;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using  System.Windows;

namespace DominatorHouseCore.Models.SocioPublisher
{
    public class PostDetailsModel : BindableBase
    {

        private string _postDescription =string.Empty;

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


        private PublisherMediaViewerModel _mediaViewer =new PublisherMediaViewerModel();

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
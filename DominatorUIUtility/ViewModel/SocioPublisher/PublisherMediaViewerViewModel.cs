using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherMediaViewerViewModel : BindableBase
    {
        public PublisherMediaViewerViewModel()
        {
            GoPreviousCommand = new BaseCommand<object>(GoPreviousCanExecute, GoPreviousExecute);
            GoNextCommand = new BaseCommand<object>(GoNextCanExecute, GoNextExecute);
        }

        #region Properties

        public ICommand GoPreviousCommand { get; set; }

        public ICommand GoNextCommand { get; set; }

        private PublisherMediaViewerModel _publisherMediaViewerModel = new PublisherMediaViewerModel();

        public PublisherMediaViewerModel PublisherMediaViewerModel
        {
            get
            {
                return _publisherMediaViewerModel;
            }
            set
            {
                if (_publisherMediaViewerModel == value)
                    return;
                _publisherMediaViewerModel = value;
                OnPropertyChanged(nameof(PublisherMediaViewerModel));
            }
        }

        #endregion

        private bool GoPreviousCanExecute(object sender) => true;

        private void GoPreviousExecute(object sender)
        {
            PublisherMediaViewerModel.MediaUrl =  PublisherMediaViewerModel.MediaList[PublisherMediaViewerModel.CurrentItem - 1];
            PublisherMediaViewerModel.CurrentItem = PublisherMediaViewerModel.CurrentItem - 1;
        }

        private bool GoNextCanExecute(object sender) => true;

        private void GoNextExecute(object sender)
        {
            PublisherMediaViewerModel.MediaUrl = PublisherMediaViewerModel.MediaList[PublisherMediaViewerModel.CurrentItem + 1];
            PublisherMediaViewerModel.CurrentItem = PublisherMediaViewerModel.CurrentItem + 1;
        }
    }
}
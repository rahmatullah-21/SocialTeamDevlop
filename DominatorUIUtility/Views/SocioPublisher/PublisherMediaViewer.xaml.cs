using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherMediaViewer.xaml
    /// </summary>
    public partial class PublisherMediaViewer : UserControl , INotifyPropertyChanged
    {
        private PublisherMediaViewerViewModel _publisherMediaViewerViewModel = new PublisherMediaViewerViewModel();

        private PublisherMediaViewerViewModel PublisherMediaViewerViewModel
        {
            get
            {
                return _publisherMediaViewerViewModel;
            }
            set
            {
                if(_publisherMediaViewerViewModel == value)
                    return;
                _publisherMediaViewerViewModel = value;
                OnPropertyChanged(nameof(PublisherMediaViewerViewModel));

            }
        }

        public PublisherMediaViewer()
        {
            InitializeComponent();
            MediaViewer.DataContext = PublisherMediaViewerViewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

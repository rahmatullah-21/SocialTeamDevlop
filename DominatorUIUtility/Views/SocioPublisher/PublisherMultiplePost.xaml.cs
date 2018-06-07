using System.ComponentModel;
using System.Runtime.CompilerServices;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;
using MahApps.Metro.Controls;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherMultiplePost.xaml
    /// </summary>
    public partial class PublisherMultiplePost : MetroWindow, INotifyPropertyChanged
    {
        public PublisherMultiplePost()
        {
            InitializeComponent();
            PublisherMultiplePostViewModel = new PublisherMultiplePostViewModel();
            PublisherMultiplePostViewModel.PublisherPostlistModels.Add(new PublisherPostlistModel());
            PublisherMultiplePostViewModel.PublisherPostlistModels.Add(new PublisherPostlistModel());
            MultiplePost.DataContext = PublisherMultiplePostViewModel;
        }

        private PublisherMultiplePostViewModel _publisherMultiplePostViewModel;
        public PublisherMultiplePostViewModel PublisherMultiplePostViewModel
        {
            get
            {
                return _publisherMultiplePostViewModel;
            }
            set
            {
                if (_publisherMultiplePostViewModel == value)
                    return;
                _publisherMultiplePostViewModel = value;
                OnPropertyChanged(nameof(PublisherMultiplePostViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

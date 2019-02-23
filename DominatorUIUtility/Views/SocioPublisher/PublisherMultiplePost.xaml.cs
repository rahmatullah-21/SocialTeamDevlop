using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherMultiplePost.xaml
    /// </summary>

    public partial class PublisherMultiplePost : UserControl, INotifyPropertyChanged, IDisposable
    {
        public PublisherMultiplePost()
        {
            InitializeComponent();
            PublisherMultiplePostViewModel = new PublisherMultiplePostViewModel();
            MultiplePost.DataContext = PublisherMultiplePostViewModel;
            _instance = this;
        }

        private static PublisherMultiplePost _instance;

        public static PublisherMultiplePost GetMultiplePost(ObservableCollection<PostDetailsModel> postDetails)
        {
            return _instance ?? (_instance = new PublisherMultiplePost(postDetails));
        }
        public PublisherMultiplePost(ObservableCollection<PostDetailsModel> postDetails) : this()
        {
            PublisherMultiplePostViewModel.LstPostDetailsModel = postDetails;
        }
        public ObservableCollection<PostDetailsModel> GetFinalPostDetails()
        {
            return PublisherMultiplePostViewModel.LstPostDetailsModel;
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


        private void PublisherMultiplePost_OnLoaded(object sender, RoutedEventArgs e)
        {
            var lstPostDetails = PublisherMultiplePostViewModel.LstPostDetailsModel = PublisherCreateCampaigns
                 .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                 .PublisherCreateCampaignModel.LstPostDetailsModels;
            PublisherMultiplePostViewModel.LstPostDetailsModel = lstPostDetails;

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    PublisherMultiplePostViewModel.PostListsCollectionView =
                        CollectionViewSource.GetDefaultView(PublisherMultiplePostViewModel.LstPostDetailsModel);
                });
            }
            else
                PublisherMultiplePostViewModel.PostListsCollectionView =
                    CollectionViewSource.GetDefaultView(PublisherMultiplePostViewModel.LstPostDetailsModel);

        }
        public void Dispose()
        {
            _instance = null;
        }
    }
}

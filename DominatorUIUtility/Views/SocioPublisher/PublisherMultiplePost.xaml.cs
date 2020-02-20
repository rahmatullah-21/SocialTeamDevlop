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
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
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
            return _instance = _instance ?? (_instance = new PublisherMultiplePost(postDetails));
        }
        public PublisherMultiplePost(ObservableCollection<PostDetailsModel> postDetails) : this()
        {
            PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels = postDetails;
        }
        public ObservableCollection<PostDetailsModel> GetFinalPostDetails()
        {
            return PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels;
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
            //var lstPostDetails = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
            //        .PublisherCreateCampaignModel.LstPostDetailsModels;
            //PublisherMultiplePostViewModel.LstPostDetailsModel = lstPostDetails;

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                    PublisherMultiplePostViewModel.PostListsCollectionView =
                        CollectionViewSource.GetDefaultView(PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels);
                });
            }
            else
                PublisherMultiplePostViewModel.PostListsCollectionView =
                    CollectionViewSource.GetDefaultView(PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels);

        }
        public void Dispose()
        {
            _instance = null;
        }
    }
}

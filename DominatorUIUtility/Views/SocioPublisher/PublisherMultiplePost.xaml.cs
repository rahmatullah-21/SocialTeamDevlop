using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;
using MahApps.Metro.Controls;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherMultiplePost.xaml
    /// </summary>
    public partial class PublisherMultiplePost : UserControl, INotifyPropertyChanged
    {
        public PublisherMultiplePost()
        {
            InitializeComponent();
            PublisherMultiplePostViewModel = new PublisherMultiplePostViewModel();
            MultiplePost.DataContext = PublisherMultiplePostViewModel;
        }


        public PublisherMultiplePost(ObservableCollection<PostDetailsModel> postDetails) : this()
        {
            PublisherMultiplePostViewModel.LstPostDetailsModel = postDetails;
        }

        public Action<PublisherMultiplePost> UpdatePostDetails { get; set; }
        public PublisherMultiplePost(Action<PublisherMultiplePost> updatePostDetails) : this()
        {
            UpdatePostDetails = updatePostDetails;
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

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublisherMultiplePostViewModel.LstPostDetailsModel = PublisherCreateCampaigns
                        .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                        .PublisherCreateCampaignModel.LstPostDetailsModels;
                    PublisherMultiplePostViewModel.PostListsCollectionView =
                        CollectionViewSource.GetDefaultView(PublisherMultiplePostViewModel.LstPostDetailsModel);
                });
            }
            else
            {
                PublisherMultiplePostViewModel.LstPostDetailsModel = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels;
                PublisherMultiplePostViewModel.PostListsCollectionView = CollectionViewSource.GetDefaultView(PublisherMultiplePostViewModel.LstPostDetailsModel);

            }

        }
    }
}

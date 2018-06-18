using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorHouseCore.FileManagers;

using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateCampaignViewModel : INotifyPropertyChanged
    {
        private TabItemsControl tabItemsControl { get; set; } = new TabItemsControl();

        public PublisherCreateCampaignViewModel()
        {
            #region Command initilization

            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            SelectDestinationCommand = new BaseCommand<object>(SelectDestinationCanExecute, SelectDestinationExecute);
            CampaignChangedCommand = new BaseCommand<object>(CampaignChangedCanExecute, CampaignChangedExecute);

            #endregion
         
            PostTabItems = InitializeTabs();
            BindTabItemsControlProperties();
            CampaignList = new ObservableCollection<string>(GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile()).Select(x => x.CampaignName));
            PublisherCreateCampaignModel.JobConfigurations.Weekday.Clear();

            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                PublisherCreateCampaignModel.JobConfigurations.Weekday.Add(new ContentSelectGroup
                {
                    Content = day.ToString()
                });
            }
        }


        #region Properties

        private PublisherCreateCampaignModel _publisherCreateCampaignModel = new PublisherCreateCampaignModel();

        public PublisherCreateCampaignModel PublisherCreateCampaignModel
        {
            get
            {
                return _publisherCreateCampaignModel;
            }
            set
            {
                if (_publisherCreateCampaignModel == value)
                    return;
                _publisherCreateCampaignModel = value;
                OnPropertyChanged(nameof(PublisherCreateCampaignModel));
            }
        }

        private ObservableCollection<string> _campaignList = new ObservableCollection<string>();
        // To hold all available the campaign name       
        public ObservableCollection<string> CampaignList
        {
            get
            {
                return _campaignList;
            }
            set
            {
                if (_campaignList == value)
                    return;
                _campaignList = value;
                OnPropertyChanged(nameof(CampaignList));
            }
        }

        private string _selectedItem;

        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value)
                    return;
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        #region Command

        public ICommand NavigationCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectDestinationCommand { get; set; }
        public ICommand CampaignChangedCommand { get; set; }

        #endregion

        public List<TabItemTemplates> PostTabItems { get; set; }

        #endregion

        private List<TabItemTemplates> InitializeTabs()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title= Application.Current.FindResource("DHlangCreatePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=> new PublisherDirectPosts(tabItemsControl))
                },
                new TabItemTemplates
                {
                    Title=Application.Current.FindResource("DHlangScrapePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new PublisherScrapePost(PublisherCreateCampaignModel.ScrapePostModel))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangSharePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new PublisherSharePost(PublisherCreateCampaignModel.SharePostModel))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangRssFeed")?.ToString(),
                    Content = new Lazy<UserControl>(()=> PublisherRssFeed.GetPublisherRssFeed(tabItemsControl))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangMonitorFolder")?.ToString(),
                    Content = new Lazy<UserControl>(()=>new PublisherMonitorFolder())
                },

            };
            return tabItems;
        }

        #region Command Methods

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance;
                    break;
            }
        }

        private bool SaveCanExecute(object sender) => true;

        private void SaveExecute(object sender)
        {           
            if (_publisherCreateCampaignModel.LstDestinationId.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please select atleast one Destination.");
                return;
            }

            if (!string.IsNullOrEmpty(SelectedItem))
            {
                var LstCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(
                    ConstantVariable.GetPublisherCampaignFile());
                LstCampaign.ForEach(x =>
                {
                    if (x.CampaignName == SelectedItem)
                        x = PublisherCreateCampaignModel;
                });
                GenericFileManager.UpdateModuleDetails<PublisherCreateCampaignModel>(LstCampaign,
                    ConstantVariable.GetPublisherCampaignFile());
            }
            else
            {
                if (GenericFileManager.AddModule<PublisherCreateCampaignModel>(PublisherCreateCampaignModel,
                    ConstantVariable.GetPublisherCampaignFile()))
                    Dialog.ShowDialog("Success", "Campaign successfully saved.");
            }
            CampaignList.Add(PublisherCreateCampaignModel.CampaignName);

            var publisherCampaignStatusModel = new PublisherCampaignStatusModel
            {
                CampaignName = PublisherCreateCampaignModel.CampaignName,
                CampaignId = PublisherCreateCampaignModel.CampaignId,
                StartDate = PublisherCreateCampaignModel.JobConfigurations.CampaignStartDate,
                EndDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                CreatedDate = PublisherCreateCampaignModel.CreatedDate,
                Status = PublisherCreateCampaignModel.CampaignStatus,
                DestinationCount = PublisherCreateCampaignModel.LstDestinationId.Count,
            };

            PublisherDefaultPage.Instance.PublisherDefaultViewModel.AddCampaignDetails(publisherCampaignStatusModel);
            
        }


        private bool SelectDestinationCanExecute(object sender) => true;

        private void SelectDestinationExecute(object sender)
        {
            SelectDestinations selectDestinations = new SelectDestinations(_publisherCreateCampaignModel.LstDestinationId);
            Dialog dialog = new Dialog();
            var metroWindow = dialog.GetMetroWindow(selectDestinations, "Select Destination");
            var IsCanceled = false;
            selectDestinations.btnCancel.Click += (CancelEventArgs, eventarg) =>
            {
                selectDestinations.PublisherManageDestinationViewModel.ListPublisherManageDestinationModels.Select(x =>
                {
                    x.IsSelected = false;
                    return x;
                });
                IsCanceled = true;
                metroWindow.Close();
            };
            metroWindow.ShowDialog();

            if (!IsCanceled)
            {
                var destinationId = selectDestinations.PublisherManageDestinationViewModel
                     .ListPublisherManageDestinationModels
                     .Where(x => x.IsSelected).Select(x => x.DestinationId).ToList();
                _publisherCreateCampaignModel.LstDestinationId = new ObservableCollection<string>(destinationId);
            }
        }

        private bool CampaignChangedCanExecute(object sender) => true;

        private void CampaignChangedExecute(object sender)
        {
            var publisherDirectPosts = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl);
            var publisherRssFeed = PublisherRssFeed.GetPublisherRssFeed(tabItemsControl);
            try
            {
                PublisherCreateCampaignModel = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>
                      (ConstantVariable.GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignName == (string)sender);
                BindTabItemsControlProperties();
                SetPostContectData(publisherDirectPosts);
                SetPublisherRssFeedData(publisherRssFeed);
            }
            catch (Exception ex)
            {
                PublisherCreateCampaignModel = new PublisherCreateCampaignModel();
                SetPostContectData(publisherDirectPosts);
                SetPublisherRssFeedData(publisherRssFeed);

                ex.DebugLog();

            }
        }
        private void SetPublisherRssFeedData(PublisherRssFeed publisherRssFeed)
        {
            publisherRssFeed.PublisherRssFeedViewModel.LstFeedUrl.Clear();
            PublisherCreateCampaignModel.LstFeedUrl.ForEach(x=> publisherRssFeed.PublisherRssFeedViewModel.LstFeedUrl.Add(x));
        }

        private void SetPostContectData(PublisherDirectPosts publisherDirectPosts)
        {
            publisherDirectPosts.PublisherDirectPostsViewModel.PostDetailsModel =
                PublisherCreateCampaignModel.PostDetailsModel;
            publisherDirectPosts.PostContentControl.SetMedia();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BindTabItemsControlProperties()
        {
            tabItemsControl.PostDetailsModel = PublisherCreateCampaignModel.PostDetailsModel;
            tabItemsControl.LstFeedUrl = PublisherCreateCampaignModel.LstFeedUrl;
            tabItemsControl.PublisherDirectPostsViewModel = new PublisherDirectPostsViewModel(tabItemsControl);
            tabItemsControl.PublisherRssFeedViewModel = new PublisherRssFeedViewModel(tabItemsControl);
        }

        public class TabItemsControl
        {
            public PostDetailsModel PostDetailsModel { get; set; }
            public PublisherDirectPostsViewModel PublisherDirectPostsViewModel { get; set; }
            public ObservableCollection<PublisherRssFeedModel> LstFeedUrl { get; set; }
            public PublisherRssFeedViewModel PublisherRssFeedViewModel { get; set; }
        }
    }
}
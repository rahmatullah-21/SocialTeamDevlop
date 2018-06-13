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

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateCampaignViewModel : INotifyPropertyChanged
    {
        public PublisherCreateCampaignViewModel()
        {
            #region Command initilization

            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            SelectDestinationCommand = new BaseCommand<object>(SelectDestinationCanExecute, SelectDestinationExecute);
            CampaignChangedCommand = new BaseCommand<object>(CampaignChangedCanExecute, CampaignChangedExecute);

            #endregion

            PostTabItems = InitializeTabs();
            CampaignList = new ObservableCollection<string>(
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetOtherDir() +
                                                                                  "\\Campaign.bin").Select(x => x.CampaignName));


            PublisherCreateCampaignModel.JobConfigurations.Weekday.Clear();

            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                PublisherCreateCampaignModel.JobConfigurations.Weekday.Add(new ContentSelectGroup
                {
                    Content = day.ToString(),
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
        //[ProtoMember(4)]
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
        private string _selectedItem ;

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
                    Content=new Lazy<UserControl>(()=> new PublisherDirectPosts(PublisherCreateCampaignModel.PostDetailsModel))
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

                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangMonitorFolder")?.ToString(),

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
                var LstCampaign=GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(
                    ConstantVariable.GetOtherDir() + "\\Campaign.bin");
                LstCampaign.ForEach(x =>
                {
                    if (x.CampaignName == SelectedItem)
                        x = PublisherCreateCampaignModel;
                });
                GenericFileManager.UpdateModuleDetails<PublisherCreateCampaignModel>(LstCampaign,
                    ConstantVariable.GetOtherDir() + "\\Campaign.bin");
            }
            else
            {
                if (GenericFileManager.AddModule<PublisherCreateCampaignModel>(PublisherCreateCampaignModel,
                    ConstantVariable.GetOtherDir() + "\\Campaign.bin"))
                    Dialog.ShowDialog("Success", "Campaign successfully saved.");
            }
            CampaignList.Add(PublisherCreateCampaignModel.CampaignName);
        }


        private bool SelectDestinationCanExecute(object sender) => true;

        private void SelectDestinationExecute(object sender)
        {
            PublisherManageDestinations publisherManageDestinations = new PublisherManageDestinations(Visibility.Collapsed);
            Dialog dialog = new Dialog();
            var metroWindow = dialog.GetMetroWindow(publisherManageDestinations, "Select Destination");
            metroWindow.ShowDialog();
            var destinationId = publisherManageDestinations.PublisherManageDestinationViewModel
                .ListPublisherManageDestinationModels
                .Where(x => x.IsSelected).Select(x => x.DestinationId).ToList();
            _publisherCreateCampaignModel.LstDestinationId = new ObservableCollection<string>(destinationId);
        }

        private bool CampaignChangedCanExecute(object sender) => true;

        private void CampaignChangedExecute(object sender)
        {

            try
            {
                PublisherCreateCampaigns.
                    GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel.PublisherCreateCampaignModel = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>
                    (ConstantVariable.GetOtherDir() + "\\Campaign.bin").FirstOrDefault(x => x.CampaignName == (string)sender);


            }
            catch (Exception ex)
            {
                ex.DebugLog();

            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateCampaignViewModel : INotifyPropertyChanged
    {     
        public PublisherCreateCampaignViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
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
                if(_publisherCreateCampaignModel == value)
                    return;
                _publisherCreateCampaignModel = value;
               OnPropertyChanged(nameof(PublisherCreateCampaignModel));
            }
        }


        public ICommand NavigationCommand { get; set; }

        public List<TabItemTemplates> PostTabItems { get; set; } = InitializeTabs();

        #endregion

        private static List<TabItemTemplates> InitializeTabs()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title= Application.Current.FindResource("DHlangCreatePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=> new PublisherDirectPosts())
                },
                new TabItemTemplates
                {
                    Title=Application.Current.FindResource("DHlangScrapePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new PublisherScrapePost())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangSharePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new PublisherSharePost())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangRssFeed")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new PublisherDirectPosts())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangMonitorFolder")?.ToString(),
                    Content=new Lazy<UserControl>(()=>new PublisherDirectPosts())
                },
               
            };
            return tabItems;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
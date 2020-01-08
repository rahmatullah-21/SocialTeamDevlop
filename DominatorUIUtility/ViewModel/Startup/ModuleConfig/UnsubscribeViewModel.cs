using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System;
using System.Linq;
using CommonServiceLocator;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IUnsubscribeViewModel : IYoutubeModel, IRedditModel
    {
        #region Youtube Model
        bool UniqueSubscribe { get; set; }
        bool IsChkSkipBlackListedUser { get; set; }
        bool IsChkPrivateBlackList { get; set; }
        bool IsChkGroupBlackList { get; set; }
        bool IsChkChannelSubscribedBySoftwareChecked { get; set; }
        bool IsChkChannelSubscribedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomChannelsListChecked { get; set; }
        string CustomChannelsList { get; set; }
        List<string> ListCustomChannels { get; set; }
        List<string> ListOfChannels { get; set; }
        #endregion

        #region Reddit Model
        bool IsChkCommunitySubscribedBySoftwareChecked { get; set; }
        bool IsChkCommunitySubscribedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomCommunityListChecked { get; set; }
        string CustomCommunityList { get; set; }
        int SubscribedBeforeDay { get; set; }
        int SubscribedBeforeHour { get; set; }
        bool IsCommunitySubscribedBeforeChecked { get; set; }
        #endregion
    }

    public class UnsubscribeViewModel : StartupBaseViewModel, IUnsubscribeViewModel
    {
        public UnsubscribeViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.UnSubscribe });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            ElementsVisibility.NetworkElementsVisibilty(this);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyUnsubscribesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyUnsubscribesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyUnsubscribesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyUnsubscribesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumUnsubscribesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        private void ValidateAndNevigate()
        {
            var network = ServiceLocator.Current.TryResolve<ISelectActivityViewModel>().SelectAccount.AccountBaseModel.AccountNetwork;
            //if (network == SocialNetworks.Youtube)
            //{
            //    if (ValidateYoutube())
            //        NavigateNext();
            //}
            //else if (ValidateReddit())
            NavigateNext();
        }

        public Visibility YoutubeElementsVisibility { get; set; } = Visibility.Collapsed;

        public Visibility RedditElementsVisibility { get; set; } = Visibility.Collapsed;
        bool ValidateYoutube()
        {
            if ((!IsChkChannelSubscribedBySoftwareChecked && !IsChkChannelSubscribedOutsideSoftwareChecked)
                && (!IsChkCustomChannelsListChecked))
            {
                Dialog.ShowDialog("Error", "Please select atleast one unsubscribe source");
                return false;
            }
            if (IsChkCustomChannelsListChecked && string.IsNullOrEmpty(CustomChannelsList))
            {
                Dialog.ShowDialog("Error", "Custom user list is empty");
                return false;
            }
            return true;
        }
        bool ValidateReddit()
        {
            if (!IsChkCommunitySubscribedBySoftwareChecked && !IsChkCommunitySubscribedOutsideSoftwareChecked
                && !IsChkCustomCommunityListChecked)
            {
                Dialog.ShowDialog("Input Error", "Please check atleast one UnSubscribe source option...");
                return false;
            }

            if (IsChkCustomCommunityListChecked && string.IsNullOrEmpty(CustomCommunityList))
            {
                Dialog.ShowDialog("Error", "Custom community list is empty");
                return false;
            }
            return true;
        }
        #region Youtube Model
        private bool _uniqueSubscribe;

        public bool UniqueSubscribe
        {
            get
            {
                return _uniqueSubscribe;
            }
            set
            {
                if (value == _uniqueSubscribe)
                    return;
                SetProperty(ref _uniqueSubscribe, value);
            }
        }

        private bool _isChkSkipBlackListedUser;

        public bool IsChkSkipBlackListedUser
        {
            get
            {
                return _isChkSkipBlackListedUser;
            }
            set
            {
                if (_isChkSkipBlackListedUser == value)
                {
                    return;
                }
                SetProperty(ref _isChkSkipBlackListedUser, value);
            }
        }

        private bool _isChkPrivateBlackList;

        public bool IsChkPrivateBlackList
        {
            get
            {
                return _isChkPrivateBlackList;
            }
            set
            {
                if (_isChkPrivateBlackList == value)
                {
                    return;
                }
                SetProperty(ref _isChkPrivateBlackList, value);
            }
        }

        private bool _isChkGroupBlackList;

        public bool IsChkGroupBlackList
        {
            get
            {
                return _isChkGroupBlackList;
            }
            set
            {
                if (_isChkGroupBlackList == value)
                {
                    return;
                }
                SetProperty(ref _isChkGroupBlackList, value);
            }
        }

        private bool _isChkChannelSubscribedBySoftwareChecked;

        public bool IsChkChannelSubscribedBySoftwareChecked
        {
            get
            {
                return _isChkChannelSubscribedBySoftwareChecked;
            }
            set
            {
                if (value == _isChkChannelSubscribedBySoftwareChecked)
                {
                    return;
                }
                SetProperty(ref _isChkChannelSubscribedBySoftwareChecked, value);
            }

        }

        private bool _isChkChannelSubscribedOutsideSoftwareChecked;

        public bool IsChkChannelSubscribedOutsideSoftwareChecked
        {
            get
            {
                return _isChkChannelSubscribedOutsideSoftwareChecked;
            }
            set
            {
                if (value == _isChkChannelSubscribedOutsideSoftwareChecked)
                {
                    return;
                }
                SetProperty(ref _isChkChannelSubscribedOutsideSoftwareChecked, value);
            }
        }

        private bool _isChkCustomChannelsListChecked;

        public bool IsChkCustomChannelsListChecked
        {
            get
            {
                return _isChkCustomChannelsListChecked;
            }
            set
            {
                if (_isChkCustomChannelsListChecked == value)
                {
                    return;
                }
                SetProperty(ref _isChkCustomChannelsListChecked, value);
            }
        }

        private string _customChannelsList;


        public string CustomChannelsList
        {
            get
            {
                return _customChannelsList;
            }
            set
            {
                if (value == _customChannelsList)
                    return;
                SetProperty(ref _customChannelsList, value);
            }
        }

        private List<string> _listOfCustomChannels = new List<string>();

        public List<string> ListCustomChannels
        {
            get
            {
                return _listOfCustomChannels;
            }
            set
            {
                if (value == _listOfCustomChannels)
                    return;
                SetProperty(ref _listOfCustomChannels, value);
            }
        }

        private List<string> _listOfChannels = new List<string>();


        public List<string> ListOfChannels
        {
            get
            {
                return _listOfChannels;
            }
            set
            {
                if (_listOfChannels == value)
                {
                    return;
                }
                SetProperty(ref _listOfChannels, value);
            }
        }
        #endregion

        #region Reddit Model

        private bool _isChkCommunitySubscribedBySoftwareChecked;

        public bool IsChkCommunitySubscribedBySoftwareChecked
        {
            get
            {
                return _isChkCommunitySubscribedBySoftwareChecked;
            }
            set
            {
                SetProperty(ref _isChkCommunitySubscribedBySoftwareChecked, value);
            }
        }

        private bool _isChkCommunitySubscribedOutsideSoftwareChecked;

        public bool IsChkCommunitySubscribedOutsideSoftwareChecked
        {
            get
            {
                return _isChkCommunitySubscribedOutsideSoftwareChecked;
            }
            set
            {
                SetProperty(ref _isChkCommunitySubscribedOutsideSoftwareChecked, value);
            }
        }

        private bool _isChkCustomCommunityListChecked;

        public bool IsChkCustomCommunityListChecked
        {
            get
            {
                return _isChkCustomCommunityListChecked;
            }
            set
            {
                SetProperty(ref _isChkCustomCommunityListChecked, value);
            }
        }

        private string _customCommunityList;

        public string CustomCommunityList
        {
            get
            {
                return _customCommunityList;
            }
            set
            {
                SetProperty(ref _customCommunityList, value);
            }
        }

        private int _subscribedBeforeDay;

        public int SubscribedBeforeDay
        {
            get
            {
                return _subscribedBeforeDay;
            }
            set
            {
                SetProperty(ref _subscribedBeforeDay, value);
            }
        }

        private int _subscribedBeforeHour;

        public int SubscribedBeforeHour
        {
            get
            {
                return _subscribedBeforeHour;
            }
            set
            {
                SetProperty(ref _subscribedBeforeHour, value);
            }
        }

        private bool _isCommunitySubscribedBeforeChecked;

        public bool IsCommunitySubscribedBeforeChecked
        {
            get
            {
                return _isCommunitySubscribedBeforeChecked;
            }
            set
            {
                SetProperty(ref _isCommunitySubscribedBeforeChecked, value);
            }
        }

        #endregion
    }
}

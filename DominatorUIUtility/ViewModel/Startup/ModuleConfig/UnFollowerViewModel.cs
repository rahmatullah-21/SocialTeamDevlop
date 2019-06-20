using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;
using ProtoBuf;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    [ProtoContract]
    public class UnFollower : BindableBase
    {
        private string _customUsers;
        private int _followedBeforeDay;
        private int _followedBeforeHour;
        private bool _isChkCustomUsersListChecked;
        private bool _isChkPeopleFollowedBySoftwareCheecked;
        private bool _isChkPeopleFollowedOutsideSoftwareChecked;
        private bool _isUserFollowedBeforeChecked;
        private bool _isWhoDoNotFollowBackChecked;
        private bool _isWhoFollowBackChecked;
        private List<string> _listOfCustomUsers = new List<string>();

        [ProtoMember(3)]
        public bool IsChkPeopleFollowedBySoftwareCheecked
        {
            get { return _isChkPeopleFollowedBySoftwareCheecked; }
            set
            {
                SetProperty(ref _isChkPeopleFollowedBySoftwareCheecked, value);
            }
        }


        [ProtoMember(4)]
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get { return _isChkPeopleFollowedOutsideSoftwareChecked; }
            set
            {
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
            }
        }


        [ProtoMember(5)]
        public bool IsChkCustomUsersListChecked
        {
            get { return _isChkCustomUsersListChecked; }
            set
            {
                SetProperty(ref _isChkCustomUsersListChecked, value);
            }
        }


        [ProtoMember(6)]
        public bool IsWhoDoNotFollowBackChecked
        {
            get { return _isWhoDoNotFollowBackChecked; }
            set
            {

                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }


        [ProtoMember(7)]
        public bool IsWhoFollowBackChecked
        {
            get { return _isWhoFollowBackChecked; }
            set
            {

                SetProperty(ref _isWhoFollowBackChecked, value);
            }
        }

        [ProtoMember(8)]
        public bool IsUserFollowedBeforeChecked
        {
            get { return _isUserFollowedBeforeChecked; }
            set
            {
                SetProperty(ref _isUserFollowedBeforeChecked, value);
            }
        }


        [ProtoMember(9)]
        public int FollowedBeforeDay
        {
            get { return _followedBeforeDay; }
            set
            {
                SetProperty(ref _followedBeforeDay, value);
            }
        }


        [ProtoMember(10)]
        public int FollowedBeforeHour
        {
            get { return _followedBeforeHour; }
            set
            {
                SetProperty(ref _followedBeforeHour, value);
            }
        }


        [ProtoMember(11)]
        public string CustomUsers
        {
            get { return _customUsers; }
            set { SetProperty(ref _customUsers, value); }
        }


        [ProtoMember(12)]
        public List<string> ListCustomUsers
        {
            get { return _listOfCustomUsers; }
            set
            {
                SetProperty(ref _listOfCustomUsers, value);
            }
        }

        [ProtoMember(13)]
        public bool IsRdWhoDoNotFollowBackChecked
        {
            get { return _isWhoDoNotFollowBackChecked; }
            set
            {
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
            }
        }
        [ProtoMember(14)]
        public bool IsRdWhoFollowBackChecked
        {
            get { return _isWhoFollowBackChecked; }
            set
            {
                SetProperty(ref _isWhoFollowBackChecked, value);
            }
        }
    }
    public interface IUnFollowerViewModel : ITwitterVisibilityModel
    {
        bool IsChkPeopleFollowedBySoftwareChecked { get; set; }
        bool IsChkPeopleFollowedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomUsersListChecked { get; set; }
        string CustomUsersList { get; set; }
        bool IsWhoDoNotFollowBackChecked { get; set; }
        bool IsWhoFollowBackChecked { get; set; }
        bool IsUserFollowedBeforeChecked { get; set; }
        int FollowedBeforeDay { get; set; }
        int FollowedBeforeHour { get; set; }

        UnFollower UnFollower { get; set; }

    }

    public class UnFollowerViewModel : StartupBaseViewModel, IUnFollowerViewModel
    {
        private UnFollower _UnFollower = new UnFollower();
        public Visibility TwitterElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility InstagramElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility AllVisibility { get; set; } = Visibility.Visible;
        public UnFollowerViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Unfollow });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(NextValidation);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            ElementsVisibility.NetworkElementsVisibilty(this);

            if (InstagramElementsVisibility == Visibility.Visible)
                AllVisibility = Visibility.Collapsed;

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfollowPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfollowPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfollowPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfollowPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfollowsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        private void NextValidation()
        {
            if ((_UnFollower.IsChkPeopleFollowedOutsideSoftwareChecked || _UnFollower.IsChkPeopleFollowedBySoftwareCheecked || _UnFollower.IsChkCustomUsersListChecked) &&
                (_UnFollower.IsWhoDoNotFollowBackChecked || _UnFollower.IsWhoFollowBackChecked))
                NavigateNext();
            else
            {
                Dialog.ShowDialog("Error", "Please select Unfollow source and source type");
                return;
            }
        }

        public UnFollower UnFollower
        {
            get { return _UnFollower; }
            set { SetProperty(ref _UnFollower, value); }
        }

        private string _customUsersList;
        public string CustomUsersList
        {
            get
            {
                return _customUsersList;
            }
            set
            {
                SetProperty(ref _customUsersList, value);
                UnFollower.CustomUsers = value;
            }
        }
        int _followedBeforeDay;
        public int FollowedBeforeDay
        {
            get
            {
                return _followedBeforeDay;
            }
            set
            {
                SetProperty(ref _followedBeforeDay, value);
                UnFollower.FollowedBeforeDay = value;
            }
        }

        private int _followedBeforeHour;
        public int FollowedBeforeHour
        {
            get
            {
                return _followedBeforeHour;
            }
            set
            {
                SetProperty(ref _followedBeforeHour, value);
                UnFollower.FollowedBeforeHour = value;
            }
        }
        private bool _isChkCustomUsersListChecked;
        public bool IsChkCustomUsersListChecked
        {
            get
            {
                return _isChkCustomUsersListChecked;
            }
            set
            {
                SetProperty(ref _isChkCustomUsersListChecked, value);
                UnFollower.IsChkCustomUsersListChecked = value;
            }
        }
        private bool _isChkPeopleFollowedBySoftwareChecked;
        public bool IsChkPeopleFollowedBySoftwareChecked
        {
            get
            {
                return _isChkPeopleFollowedBySoftwareChecked;
            }
            set
            {
                SetProperty(ref _isChkPeopleFollowedBySoftwareChecked, value);
                UnFollower.IsChkPeopleFollowedBySoftwareCheecked = value;
            }
        }
        private bool _isChkPeopleFollowedOutsideSoftwareChecked;
        public bool IsChkPeopleFollowedOutsideSoftwareChecked
        {
            get
            {
                return _isChkPeopleFollowedOutsideSoftwareChecked;
            }
            set
            {
                SetProperty(ref _isChkPeopleFollowedOutsideSoftwareChecked, value);
                UnFollower.IsChkPeopleFollowedOutsideSoftwareChecked = value;
            }
        }

        private bool _isUserFollowedBeforeChecked;
        public bool IsUserFollowedBeforeChecked
        {
            get
            {
                return _isUserFollowedBeforeChecked;
            }
            set
            {
                SetProperty(ref _isUserFollowedBeforeChecked, value);
                UnFollower.IsUserFollowedBeforeChecked = value;
            }
        }
        private bool _isWhoDoNotFollowBackChecked;
        public bool IsWhoDoNotFollowBackChecked
        {
            get
            {
                return _isWhoDoNotFollowBackChecked;
            }
            set
            {
                SetProperty(ref _isWhoDoNotFollowBackChecked, value);
                UnFollower.IsWhoDoNotFollowBackChecked = value;
            }
        }
        private bool _isWhoFollowBackChecked;
        public bool IsWhoFollowBackChecked
        {
            get
            {
                return _isWhoFollowBackChecked;
            }
            set
            {
                SetProperty(ref _isWhoFollowBackChecked, value);
                UnFollower.IsWhoFollowBackChecked = value;
            }
        }
    }
}

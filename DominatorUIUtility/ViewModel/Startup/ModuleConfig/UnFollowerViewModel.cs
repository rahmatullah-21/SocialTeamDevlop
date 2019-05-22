using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IUnFollowerViewModel
    {
        JobConfiguration JobConfiguration { get; set; }
        bool IsChkPeopleFollowedBySoftwareChecked { get; set; }
        bool IsChkPeopleFollowedOutsideSoftwareChecked { get; set; }
        bool IsChkCustomUsersListChecked { get; set; }
        string CustomUsersList { get; set; }
        bool IsWhoDoNotFollowBackChecked { get; set; }
        bool IsWhoFollowBackChecked { get; set; }
        int IsUserFollowedBeforeChecked { get; set; }
        int FollowedBeforeDay { get; set; }
        int FollowedBeforeHour { get; set; }

    }

    public class UnFollowerViewModel : StartupBaseViewModel, IUnFollowerViewModel
    {
        public UnFollowerViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfollowPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfollowPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfollowPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfollowPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfollowsPerDay".FromResourceDictionary(),
            };
        }
        private JobConfiguration _jobConfiguration;

        public JobConfiguration JobConfiguration
        {
            get
            {
                return _jobConfiguration;
            }
            set
            {
                if (value == _jobConfiguration)
                    return;
                SetProperty(ref _jobConfiguration, value);
            }
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
            }
        }

        private int _isUserFollowedBeforeChecked;
        public int IsUserFollowedBeforeChecked
        {
            get
            {
                return _isUserFollowedBeforeChecked;
            }
            set
            {
                SetProperty(ref _isUserFollowedBeforeChecked, value);
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
            }
        }
    }
}

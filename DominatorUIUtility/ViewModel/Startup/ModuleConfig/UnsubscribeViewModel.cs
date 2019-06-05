using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class UnSubscribeModel : BindableBase
    {
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

        private bool _isChkEnableAutoUnSubscribeUnUnSubscribeChecked;

        public bool IsChkEnableAutoSubscribeUnSubscribeChecked
        {
            get
            {//IsChkEnableAutoSubscribeUnSubscribeChecked
                return _isChkEnableAutoUnSubscribeUnUnSubscribeChecked;
            }
            set
            {
                if (_isChkEnableAutoUnSubscribeUnUnSubscribeChecked == value)
                {
                    return;
                }
                SetProperty(ref _isChkEnableAutoUnSubscribeUnUnSubscribeChecked, value);
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

        //public bool IsAccountGrowthActive { get; set; }
        private bool _isAccountGrowthActive;

        public bool IsAccountGrowthActive
        {
            get
            {
                return _isAccountGrowthActive;
            }
            set
            {
                if (_isAccountGrowthActive == value)
                {
                    return;
                }
                SetProperty(ref _isAccountGrowthActive, value);
            }
        }
        public bool IsChkUnSubscribeUnique { get; set; }
        public bool IsChkUnUnSubscribeUnSubscribeedbackChecked { get; set; }

        public bool IsChkStartUnUnSubscribeWhenReached { get; set; }

        public bool IsChkUnUnSubscribenotUnSubscribeedbackChecked { get; set; }

        public int UnSubscribeerUnSubscribeingsMaxValue { get; set; }

        public bool IsChkStartUnUnSubscribe { get; set; }

        private RangeUtilities _startUnSubscribeToolWhenReach = new RangeUtilities();
        private RangeUtilities _stopUnSubscribeToolWhenReach = new RangeUtilities();
        private bool _isChkStopUnSubscribeToolWhenReachChecked;
        private bool _isChkStartUnUnSubscribeToolBetweenChecked;
        private bool _isChkUnUnSubscribeUsersChecked;
        private bool _startUnUnSubscribe;

        public RangeUtilities StopUnSubscribeToolWhenReach
        {
            get
            {
                return _stopUnSubscribeToolWhenReach;
            }

            set
            {
                if (value == _stopUnSubscribeToolWhenReach)
                    return;
                SetProperty(ref _stopUnSubscribeToolWhenReach, value);
            }
        }

        public RangeUtilities StartUnUnSubscribeToolWhenReach
        {
            get
            {
                return _startUnSubscribeToolWhenReach;
            }
            set
            {
                if (value == _startUnSubscribeToolWhenReach)
                    return;
                SetProperty(ref _startUnSubscribeToolWhenReach, value);
            }
        }

        public bool IsChkUnSubscribeToolGetsTemporaryBlockedChecked { get; set; }

        public bool IsChkWhenUnSubscribeerUnSubscribeingsIsSmallerThanChecked { get; set; }

        public bool IsChkStopUnSubscribe { get; set; }

        public bool IsChkStopUnSubscribeToolWhenReachChecked
        {
            get
            {
                return _isChkStopUnSubscribeToolWhenReachChecked;
            }
            set
            {
                if (value == _isChkStopUnSubscribeToolWhenReachChecked)
                    return;
                SetProperty(ref _isChkStopUnSubscribeToolWhenReachChecked, value);
            }
        }

        public bool IsChkStartUnUnSubscribeToolBetweenChecked
        {
            get
            {
                return _isChkStartUnUnSubscribeToolBetweenChecked;
            }
            set
            {
                if (value == _isChkStartUnUnSubscribeToolBetweenChecked)
                    return;
                SetProperty(ref _isChkStartUnUnSubscribeToolBetweenChecked, value);
            }
        }

        public bool IsChkUnUnSubscribeUsersChecked
        {
            get
            {
                return _isChkUnUnSubscribeUsersChecked;
            }
            set
            {
                if (value == _isChkUnUnSubscribeUsersChecked)
                    return;
                SetProperty(ref _isChkUnUnSubscribeUsersChecked, value);
            }
        }

        public bool StartUnUnSubscribe
        {
            get
            {
                return _startUnUnSubscribe;
            }
            set
            {
                if (value == _startUnUnSubscribe)
                    return;
                SetProperty(ref _startUnUnSubscribe, value);
            }
        }

      

    }
    public interface IUnsubscribeViewModel
    {
    }
    public class UnsubscribeViewModel : StartupBaseViewModel, IUnsubscribeViewModel
    {
        public UnsubscribeViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.UnSubscribe });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyUnsubscribesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyUnsubscribesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyUnsubscribesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyUnsubscribesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumUnsubscribesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            
        }
        private UnSubscribeModel _unSubscribeModel = new UnSubscribeModel();

        public UnSubscribeModel UnSubscribeModel
        {
            get
            {
                return _unSubscribeModel;
            }
            set
            {
                if (_unSubscribeModel == null & _unSubscribeModel == value)
                    return;
                SetProperty(ref _unSubscribeModel, value);
            }
        }
    }
}

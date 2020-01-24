using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Linq;

namespace LegionUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IIncommingFriendRequestViewModel
    {
        int Count { get; set; }
        bool IsCancelReceivedRequest { get; set; }
        bool IsAcceptRequest { get; set; }
    }
    public class IncommingFriendRequestViewModel : StartupBaseViewModel, IIncommingFriendRequestViewModel
    {
        public IncommingFriendRequestViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.IncommingFriendRequest });
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfRequestPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfRequestPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfRequestPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfRequestPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxRequestPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }
        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (value == _count)
                    return;
                SetProperty(ref _count, value);
            }
        }
        private bool _isAcceptRequest;
        public bool IsAcceptRequest
        {
            get
            {
                return _isAcceptRequest;
            }
            set
            {
                if (value == _isAcceptRequest)
                    return;
                SetProperty(ref _isAcceptRequest, value);
            }
        }

        private bool _isCancelReceivedRequest;

        public bool IsCancelReceivedRequest
        {
            get
            {
                return _isCancelReceivedRequest;
            }
            set
            {
                if (value == _isCancelReceivedRequest)
                    return;
                SetProperty(ref _isCancelReceivedRequest, value);
            }
        }
    }
}

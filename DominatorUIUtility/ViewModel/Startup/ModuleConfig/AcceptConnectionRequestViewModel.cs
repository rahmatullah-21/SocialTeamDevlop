using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IAcceptConnectionRequestViewModel
    {
        bool IsChkAllInvitations { get; set; }
        bool IsChkIgnoreAllInvitations { get; set; }
    }
    public class AcceptConnectionRequestViewModel : StartupBaseViewModel, IAcceptConnectionRequestViewModel
    {
        public AcceptConnectionRequestViewModel(IRegionManager region) : base(region)
        {
            IsNonQuery = true;
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.AcceptConnectionRequest });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfConnectionRequestsToAcceptPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfConnectionRequestsToAcceptPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfConnectionRequestsToAcceptPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfConnectionRequestsToAcceptPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxConnectionRequestsToAcceptPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }

        private bool _isChkIgnoreAllInvitations;
        private bool _IsChkIgnoreAllInvitations;
        public bool IsChkAllInvitations
        {
            get { return _isChkIgnoreAllInvitations; }
            set { SetProperty(ref _isChkIgnoreAllInvitations, value); }
        }

        public bool IsChkIgnoreAllInvitations
        {
            get { return _IsChkIgnoreAllInvitations; }
            set { SetProperty(ref _IsChkIgnoreAllInvitations, value); }
        }
    }
}

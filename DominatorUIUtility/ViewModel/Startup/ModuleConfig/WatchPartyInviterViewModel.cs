using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IWatchPartyInviterViewModel
    {
    }
    public class WatchPartyInviterViewModel : StartupBaseViewModel, IWatchPartyInviterViewModel
    {
        public WatchPartyInviterViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.WatchPartyInviter });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            IsNonQuery = true;

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyInviteNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyInviteNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyInviteNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyInviteToNumberOfProfilesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyInviteMaxProfilesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds=Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
        }

        private InviterDetails _inviterDetails = new InviterDetails();
        public InviterDetails InviterDetailsModel
        {
            get { return _inviterDetails; }
            set
            {
                if (_inviterDetails == null & _inviterDetails == value)
                    return;
                SetProperty(ref _inviterDetails, value);
            }
        }

        private InviterOptions _inviterOptions=new InviterOptions();
        public InviterOptions InviterOptionsModel
        {
            get { return _inviterOptions; }
            set
            {
                if (_inviterOptions == value & _inviterOptions == null)
                    return;
                SetProperty(ref _inviterOptions, value);
            }
        }

    }
}

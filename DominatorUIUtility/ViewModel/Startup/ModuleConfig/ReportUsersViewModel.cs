using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
   
    public interface IReportUsersViewModel
    {
    }
    public class ReportUsersViewModel : StartupBaseViewModel, IReportUsersViewModel
    {
        public ReportUsersViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.ReportUsers });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUsersToReportPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUsersToReportPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUsersToReportPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUsersToReportPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUsersToReportPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}

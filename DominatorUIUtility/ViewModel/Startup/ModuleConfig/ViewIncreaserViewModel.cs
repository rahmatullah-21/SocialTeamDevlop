using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    
    public interface IViewIncreaserViewModel
    {
    }
    public class ViewIncreaserViewModel : StartupBaseViewModel, IViewIncreaserViewModel
    {
        public ViewIncreaserViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.ViewIncreaser });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyViewIncreasePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyViewIncreasePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyViewIncreasePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyViewIncreasePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaximumViewIncreasePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}

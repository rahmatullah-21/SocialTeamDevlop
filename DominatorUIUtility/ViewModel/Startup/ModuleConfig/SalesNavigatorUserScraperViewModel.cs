using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface ISalesNavigatorUserScraperViewModel
    {
    }
    public class SalesNavigatorUserScraperViewModel : StartupBaseViewModel, ISalesNavigatorUserScraperViewModel
    {
        public SalesNavigatorUserScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.SalesNavigatorUserScraper });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfScrapingPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfScrapingPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfScrapingPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfScrapingPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxScrape".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }
    }
}

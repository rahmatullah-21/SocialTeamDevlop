using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    
    public interface IPostScraperViewModel
    {
    }
    public class PostScraperViewModel : StartupBaseViewModel, IPostScraperViewModel
    {
        public PostScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.PostScraper });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName ="LangKeyScrapNumberOfPostsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName ="LangKeyScrapNumberOfPostsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName ="LangKeyScrapNumberOfPostsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName ="LangKeyScrapNumberOfPostsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName ="LangKeyScrapMaxPostsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}

using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
   
    public interface IHashtagsScraperViewModel
    {
        string Keyword { get; set; }
    }
    public class HashtagsScraperViewModel : StartupBaseViewModel, IHashtagsScraperViewModel
    {
        public HashtagsScraperViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.HashtagsScraper });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfHashtagsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfHashtagsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfHashtagsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfHashtagsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxHashtagsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }

        private string _Keyword;
        public string Keyword
        {
            get
            {
                return _Keyword;
            }

            set
            {
                if (_Keyword == value)
                    return ;
                SetProperty(ref _Keyword,value);
            }
        }
    }
}

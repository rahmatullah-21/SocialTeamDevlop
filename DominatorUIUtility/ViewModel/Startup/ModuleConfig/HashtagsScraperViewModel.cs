using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Linq;

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
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.HashtagsNiching });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNevigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfHashtagsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfHashtagsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfHashtagsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfHashtagsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxHashtagsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        private void ValidateAndNevigate()
        {
            if (string.IsNullOrEmpty(Keyword))
            {
                Dialog.ShowDialog("Input Error", "Please enter atleast one keyword");
                return;
            }
           NavigateNext();
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

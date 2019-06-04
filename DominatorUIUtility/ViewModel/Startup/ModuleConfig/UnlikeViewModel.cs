using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class UnLikeModel : BindableBase
    {
        private string _CustomTweets = string.Empty;
        private bool _IsCustomTweets;
        private bool _IsLikedTweets;

        public List<string> ListQueryType { get; set; } = new List<string>();
        public bool IsLikedTweets
        {
            get { return _IsLikedTweets; }
            set { SetProperty(ref _IsLikedTweets, value); }
        }

        public bool IsCustomTweets
        {
            get { return _IsCustomTweets; }
            set { SetProperty(ref _IsCustomTweets, value); }
        }


        public string CustomTweets
        {
            get { return _CustomTweets; }
            set { SetProperty(ref _CustomTweets, value); }
        }

    }
    public interface IUnlikeViewModel
    {
    }
    public class UnlikeViewModel : StartupBaseViewModel, IUnlikeViewModel
    {
        public UnlikeViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Unlike });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
    }
}

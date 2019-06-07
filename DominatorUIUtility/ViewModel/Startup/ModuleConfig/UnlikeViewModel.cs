using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig;
using Prism.Commands;
using Prism.Regions;
using ProtoBuf;
using System;
using System.Windows;


namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    [ProtoContract]
    public class UnLike : BindableBase
    {
        private string _CustomTweets = string.Empty;
        private bool _IsCustomTweets;
        private bool _IsLikedTweets;

        [ProtoMember(1)]
        public bool IsLikedTweets
        {
            get { return _IsLikedTweets; }
            set { SetProperty(ref _IsLikedTweets, value); }
        }

        [ProtoMember(2)]
        public bool IsCustomTweets
        {
            get { return _IsCustomTweets; }
            set { SetProperty(ref _IsCustomTweets, value); }
        }

        [ProtoMember(3)]
        public string CustomTweets
        {
            get { return _CustomTweets; }
            set { SetProperty(ref _CustomTweets, value); }
        }
    }
    public interface IUnlikeViewModel : ITwitterVisibilityModel, IInstagramVisibilityModel
    {
        bool IsCheckedUnlikeMedia { get; set; }
        UnLike UnLike { get; set; }
    }
    public class UnlikeViewModel : StartupBaseViewModel, IUnlikeViewModel
    {
        private bool _IsCheckedUnlikeMedia = true;
        public Visibility InstagramElementsVisibility { get; set; } = Visibility.Collapsed;
        public Visibility TwitterElementsVisibility { get; set; } = Visibility.Collapsed;
        public bool IsCheckedUnlikeMedia
        {
            get { return _IsCheckedUnlikeMedia; }
            set { SetProperty(ref _IsCheckedUnlikeMedia, value); }
        }

        private UnLike _UnLike = new UnLike();

        public UnLike UnLike
        {
            get { return _UnLike; }
            set { SetProperty(ref _UnLike, value); }
        }

        public UnlikeViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Unlike });
            ElementsVisibility.NetworkElementsVisibilty(this);
            IsNonQuery = true;
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

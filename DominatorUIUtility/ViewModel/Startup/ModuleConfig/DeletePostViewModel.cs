using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IDeletePostViewModel
    {
        bool IsCheckedDeleteAllPosts { get; set; }
        bool ChkDeletePostWhichIsPostedBySoftware { get; set; }
        bool ChkDeletePostWhichIsPostedByOutsideSoftware { get; set; }
    }
    public class DeletePostViewModel : StartupBaseViewModel, IDeletePostViewModel
    {
        public DeletePostViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.DeletePost });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(ValidateAndNavigate);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfPostsDeletePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfPostsDeletePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfPostsDeletePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfPostsDeletePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxPostsDeletePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        private void ValidateAndNavigate()
        {
            if (!ChkDeletePostWhichIsPostedBySoftware && !ChkDeletePostWhichIsPostedByOutsideSoftware)
            {
                Dialog.ShowDialog("Input Error", "Please select atleast one option");
                return;
            }
            NavigateNext();
        }

        private bool _IsCheckedDeleteAllPosts = true;
        public bool IsCheckedDeleteAllPosts
        {
            get
            {
                return _IsCheckedDeleteAllPosts;
            }

            set
            {
                if (_IsCheckedDeleteAllPosts == value)
                    return;
                SetProperty(ref _IsCheckedDeleteAllPosts, value);
            }
        }

        private bool _ChkDeletePostWhichIsPostedBySoftware = false;
        public bool ChkDeletePostWhichIsPostedBySoftware
        {
            get
            {
                return _ChkDeletePostWhichIsPostedBySoftware;
            }

            set
            {
                if (_ChkDeletePostWhichIsPostedBySoftware == value)
                    return;
                SetProperty(ref _ChkDeletePostWhichIsPostedBySoftware, value);
            }
        }

        private bool _ChkDeletePostWhichIsPostedByOutsideSoftware = false;
        public bool ChkDeletePostWhichIsPostedByOutsideSoftware
        {
            get
            {
                return _ChkDeletePostWhichIsPostedByOutsideSoftware;
            }

            set
            {
                if (_ChkDeletePostWhichIsPostedByOutsideSoftware == value)
                    return;
                SetProperty(ref _ChkDeletePostWhichIsPostedByOutsideSoftware, value);
            }
        }
    }
}

using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class CommentModel
    {
        public ObservableCollection<ManageCommentModel> LstDisplayManageCommentModel { get; set; } = new ObservableCollection<ManageCommentModel>();

        public ManageCommentModel ManageCommentModel { get; set; } = new ManageCommentModel();
    }
    public interface ICommentViewModel
    {
    }
    public class CommentViewModel : StartupBaseViewModel, ICommentViewModel
    {
        public CommentViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Comment });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfCommentsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfCommentsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfCommentsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfCommentsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxCommentPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }

        private CommentModel _commentModel=new CommentModel();

        public CommentModel CommentModel
        {
            get { return _commentModel; }
            set {SetProperty(ref _commentModel , value); }
        }


    }
}

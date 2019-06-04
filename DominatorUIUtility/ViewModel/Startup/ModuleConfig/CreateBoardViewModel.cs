using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class BoardInfo : BindableBase
    {
        private string _boardName;

        public string BoardName
        {
            get
            {
                return _boardName;
            }
            set
            {
                if (_boardName != null && _boardName == value)
                    return;
                SetProperty(ref _boardName, value);
            }
        }

        private string _boardDescription;

        public string BoardDescription
        {
            get
            {
                return _boardDescription;
            }
            set
            {
                if (_boardDescription != null && _boardDescription == value)
                    return;
                SetProperty(ref _boardDescription, value);
            }
        }


        private string _category;

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (_category != null && _category == value)
                    return;
                SetProperty(ref _category, value);
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                if (_selectedIndex != 0 && _selectedIndex == value)
                    return;
                SetProperty(ref _selectedIndex, value);
            }
        }


        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    public interface ICreateBoardViewModel
    {
    }
    public class CreateBoardViewModel : StartupBaseViewModel, ICreateBoardViewModel
    {
        public CreateBoardViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.CreateBoard });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfBoardsCreatePerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfBoardsCreatePerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfBoardsCreatePerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfBoardsCreatePerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxBoardsCreatePerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }
        public BoardInfo CurrentBoard = new BoardInfo();
        public ObservableCollectionBase<BoardInfo> ListBoardInfo = new ObservableCollectionBase<BoardInfo>();
    }
}

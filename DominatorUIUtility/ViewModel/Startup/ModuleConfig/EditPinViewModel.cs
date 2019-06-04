using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public class PinInfo : BindableBase
    {
        private string _board;

        public string Board
        {
            get
            {
                return _board;
            }
            set
            {
                if (_board != null && _board == value)
                    return;
                SetProperty(ref _board, value);
            }
        }

        private string _pinDescription;

        public string PinDescription
        {
            get
            {
                return _pinDescription;
            }
            set
            {
                if (_pinDescription != null && _pinDescription == value)
                    return;
                SetProperty(ref _pinDescription, value);
            }
        }


        private string _section;

        public string Section
        {
            get
            {
                return _section;
            }
            set
            {
                if (_section != null && _section == value)
                    return;
                SetProperty(ref _section, value);
            }
        }

        private string _websiteUrl;

        public string WebsiteUrl
        {
            get
            {
                return _websiteUrl;
            }
            set
            {
                if (_websiteUrl != null && _websiteUrl == value)
                    return;
                SetProperty(ref _websiteUrl, value);
            }
        }

        private string _pinToBeEdit;

        public string PinToBeEdit
        {
            get
            {
                return _pinToBeEdit;
            }
            set
            {
                if (_pinToBeEdit != null && _pinToBeEdit == value)
                    return;
                SetProperty(ref _pinToBeEdit, value);
            }
        }

        private string _account;

        public string Account
        {
            get
            {
                return _account;
            }
            set
            {
                if (_account != null && _account == value)
                    return;
                SetProperty(ref _account, value);
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
    }
    public interface IEditPinViewModel
    {
    }
    public class EditPinViewModel : StartupBaseViewModel, IEditPinViewModel
    {
        public EditPinViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.EditPin });
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);

            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfEditPinsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfEditPinsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfEditPinsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfEditPinsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyEditPinsPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
        }
        public PinInfo CurrentPin = new PinInfo();
        ObservableCollectionBase<PinInfo> ListPinInfo = new ObservableCollectionBase<PinInfo>();
    }
}

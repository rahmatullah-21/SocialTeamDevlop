using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IUnfriendViewModel
    {
        int Count { get; set; }
        int TypeCount { get; set; }
        List<string> LstFilterText { get; set; }
        string FilterText { get; set; }
    }
    public class UnfriendViewModel : StartupBaseViewModel, IUnfriendViewModel
    {
        public UnfriendViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.Unfriend });

            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            SaveCommandBinding = new BaseCommand<object>(sender => true, UserInputOnSaveExecute);
            SelectOptionCommandBinding = new BaseCommand<object>(sender => true, SelectOptionCommandExecute);
            LoadTextBoxes();
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfUnfriendPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfUnfriendPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfUnfriendPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfUnfriendPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxUnfriendPerDay".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes
            };
            ListQueryType.Clear();
        }

        public ICommand SaveCommandBinding { get; set; }

        public ICommand SelectOptionCommandBinding { get; set; }

        private void LoadTextBoxes()
        {
            if (LstSelectedInput != null)
            {
                foreach (var str in LstSelectedInput)
                {
                    InputText = InputText + str + "\r\n";
                }
            }
        }
        private void SelectOptionCommandExecute(object obj)
        {
          
        }
        private void UserInputOnSaveExecute(object sender)
        {

            if (!string.IsNullOrEmpty(InputText))
            {
                LstSelectedInput = Regex.Split(InputText, "\r\n").ToList();
            }
            else
            {
                Dialog.ShowDialog("Error", "There is no data to save.");
            }

        }
        private string _inputText;

        public string InputText
        {
            get { return _inputText; }
            set { SetProperty(ref _inputText, value); }
        }
        private IEnumerable<string> _lstSelectedInput;

        public IEnumerable<string> LstSelectedInput
        {
            get { return _lstSelectedInput; }
            set { SetProperty(ref _lstSelectedInput, value); }
        }



        private bool _isAddedThroughSoftware;
        public bool IsAddedThroughSoftware
        {
            get
            {
                return _isAddedThroughSoftware;
            }
            set
            {
                SetProperty(ref _isAddedThroughSoftware, value);
            }
        }

        private bool _isAddedOutsideSoftware;
        public bool IsAddedOutsideSoftware
        {
            get
            {
                return _isAddedOutsideSoftware;
            }
            set
            {
                SetProperty(ref _isAddedOutsideSoftware, value);
            }
        }


        private bool _isFilterApplied;
        public bool IsFilterApplied
        {
            get
            {
                return _isFilterApplied;
            }

            set
            {
                SetProperty(ref _isFilterApplied, value);
            }
        }

        private int _daysBefore;
        public int DaysBefore
        {
            get
            {
                return _daysBefore;
            }
            set
            {
                SetProperty(ref _daysBefore, value);
            }
        }


        private int _hoursBefore;
        public int HoursBefore
        {
            get
            {
                return _hoursBefore;
            }

            set
            {
                SetProperty(ref _hoursBefore, value);
            }
        }

        private int _count;
        public int Count
        {
            get
            {
                return _count;
            }

            set
            {
                SetProperty(ref _count, value);
            }
        }

        int _typeCount;
        public int TypeCount
        {
            get
            {
                return _typeCount;
            }

            set
            {
                SetProperty(ref _typeCount, value);
            }
        }


        private string _bySoftwareDisplayName = string.Empty;
        public string BySoftwareDisplayName
        {
            get
            {
                return _bySoftwareDisplayName;
            }

            set
            {
                SetProperty(ref _bySoftwareDisplayName, value);
            }
        }

        string _outsideSoftwareDisplayName = string.Empty;
        public string OutsideSoftwareDisplayName
        {
            get
            {
                return _outsideSoftwareDisplayName;
            }

            set
            {
                SetProperty(ref _outsideSoftwareDisplayName, value);

            }
        }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get
            {
                return _filterText;
            }

            set
            {
                SetProperty(ref _filterText, value);
            }
        }
        private List<string> _lstFilterText = new List<string>();
        public List<string> LstFilterText
        {
            get
            {
                return _lstFilterText;
            }

            set
            {
                SetProperty(ref _lstFilterText, value);
            }
        }
        private string _sourceDisplayName;
        public string SourceDisplayName
        {
            get
            {
                return _sourceDisplayName;
            }

            set
            {
                SetProperty(ref _sourceDisplayName, value);

            }
        }
        private string _customUserText;
        public string CustomUserText
        {
            get
            {
                return _customUserText;
            }

            set
            {
                SetProperty(ref _customUserText, value);
            }
        }

        private List<string> _lstCustomUsers;
        public List<string> LstCustomUsers
        {
            get
            {
                return _lstCustomUsers;
            }

            set
            {
                SetProperty(ref _lstCustomUsers, value);
            }
        }

        private bool _isCustomUserList;
        public bool IsCustomUserList
        {
            get
            {
                return _isCustomUserList;
            }

            set
            {
                SetProperty(ref _isCustomUserList, value);
            }
        }

        private bool _isMutualFriends;
        public bool IsMutualFriends
        {
            get
            {
                return _isMutualFriends;
            }

            set
            {
                SetProperty(ref _isMutualFriends, value);
            }
        }


    }
}

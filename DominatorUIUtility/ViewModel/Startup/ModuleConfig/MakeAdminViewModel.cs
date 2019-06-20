using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{

    public interface IMakeAdminViewModel
    {
        SelectAccountDetailsModel SelectAccountDetailsModel { get; set; }
        bool IsSelctDetails { get; set; }
    }
    public class MakeAdminViewModel : StartupBaseViewModel, IMakeAdminViewModel
    {
        public MakeAdminViewModel(IRegionManager region) : base(region)
        {
            ViewModelToSave.Add(new ActivityConfig { Model = this, ActivityType = ActivityType.MakeAdmin });
            IsNonQuery = true;
            NextCommand = new DelegateCommand(NavigateNext);
            PreviousCommand = new DelegateCommand(NavigatePrevious);
            LoadedCommand = new DelegateCommand<string>(OnLoad);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyMakeAdminToMaxProfilesPerDay".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMakeAdminToNumberOfProfilesPerWeek".FromResourceDictionary(),
                RunningTime = RunningTimes.DayWiseRunningTimes,
                Speeds = Enum.GetNames(typeof(ActivitySpeed)).ToList()
            };
            ListQueryType.Clear();
        }

        private SelectAccountDetailsModel _selectAccountDetailsModel = new SelectAccountDetailsModel();
        public SelectAccountDetailsModel SelectAccountDetailsModel
        {
            get { return _selectAccountDetailsModel; }
            set
            {
                if (_selectAccountDetailsModel == value & _selectAccountDetailsModel == null)
                    return;
                SetProperty(ref _selectAccountDetailsModel, value);
            }

        }

        private bool _isSelctDetails;
        public bool IsSelctDetails
        {
            get { return _isSelctDetails; }
            set
            {
                if (_isSelctDetails == value)
                    return;
                SetProperty(ref _isSelctDetails, value);
            }
        }

    }
}

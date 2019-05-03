using DominatorHouseCore.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;

namespace DominatorHouse.ViewModels.Startup
{
    public interface IJobConfigViewModel
    {

    }
    public class JobConfigViewModel : StartupBaseViewModel, IJobConfigViewModel
    {
        public JobConfigViewModel(IRegionManager region):base(region)
        {
            NextCommand = new DelegateCommand<string>(OnNextClick);
            PreviousCommand = new DelegateCommand<string>(OnPreviousClick);
            JobConfiguration.RunningTime = RunningTimes.DayWiseRunningTimes;
            LstJobConfiguration.Add(new ActivityTypeWithJobConfiguration
            {
                ActivityType = "Follow",
                ActivityJobConfiguration=new JobConfiguration
                {
                    RunningTime = RunningTimes.DayWiseRunningTimes
                }
            });
            LstJobConfiguration.Add(new ActivityTypeWithJobConfiguration
            {
                ActivityType = "Follow",
                ActivityJobConfiguration = new JobConfiguration
                {
                    RunningTime = RunningTimes.DayWiseRunningTimes
                }
            });
            LstJobConfiguration.Add(new ActivityTypeWithJobConfiguration
            {
                ActivityType = "Follow",
                ActivityJobConfiguration = new JobConfiguration
                {
                    RunningTime = RunningTimes.DayWiseRunningTimes
                }
            });
            LstJobConfiguration.Add(new ActivityTypeWithJobConfiguration
            {
                ActivityType = "Unlike",
                ActivityJobConfiguration = new JobConfiguration
                {
                    RunningTime = RunningTimes.DayWiseRunningTimes
                }
            });
            LstJobConfiguration.Add(new ActivityTypeWithJobConfiguration
            {
                ActivityType = "Like",
                ActivityJobConfiguration = new JobConfiguration
                {
                    RunningTime = RunningTimes.DayWiseRunningTimes
                }
            });
            LstJobConfiguration.Add(new ActivityTypeWithJobConfiguration
            {
                ActivityType = "UnFollow",
                ActivityJobConfiguration = new JobConfiguration
                {
                    RunningTime = RunningTimes.DayWiseRunningTimes
                }

            });
        }
        private bool _isIndivisualSelected;

        public bool IsIndivisualSelected
        {
            get { return _isIndivisualSelected; }
            set { SetProperty(ref _isIndivisualSelected, value); }
        }

        private JobConfiguration _jobConfiguration = new JobConfiguration();
        
        public JobConfiguration JobConfiguration
        {
            get { return _jobConfiguration; }
            set { SetProperty(ref _jobConfiguration, value); }
        }

        private ObservableCollection<ActivityTypeWithJobConfiguration> _lstJobConfiguration = new ObservableCollection<ActivityTypeWithJobConfiguration>();

        public ObservableCollection<ActivityTypeWithJobConfiguration> LstJobConfiguration
        {
            get { return _lstJobConfiguration; }
            set { SetProperty(ref _lstJobConfiguration, value); }
        }
    }
    public class ActivityTypeWithJobConfiguration:BindableBase
    {
        private JobConfiguration _activityJobConfiguration = new JobConfiguration();

        public JobConfiguration ActivityJobConfiguration
        {
            get { return _activityJobConfiguration; }
            set { SetProperty(ref _activityJobConfiguration, value); }
        }
        private string _activityType;

        public string ActivityType
        {
            get { return _activityType; }
            set { SetProperty(ref _activityType , value); }
        }

    }
}

using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ILikeViewModel : IStartupJobConfiguration, IStartUpSearchQuery
    {
    }
    public class LikeViewModel : StartupBaseViewModel, ILikeViewModel
    {
        public LikeViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfLikesPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfLikesPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfLikesPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfLikesPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxLikesPerDay".FromResourceDictionary(),
            };
        }

        private JobConfiguration _jobConfiguration;

        public JobConfiguration JobConfiguration
        {
            get
            {
                return _jobConfiguration;
            }
            set
            {
                if (value == _jobConfiguration)
                    return;
                SetProperty(ref _jobConfiguration, value);
            }
        }

        private ObservableCollection<QueryInfo> _savedQueries = new ObservableCollection<QueryInfo>();
      
        public  ObservableCollection<QueryInfo> SavedQueries
        {
            get
            {
                return _savedQueries;
            }
            set
            {
                if (_savedQueries != null && _savedQueries == value)
                    return;
                SetProperty(ref _savedQueries, value);
            }
        }

    }
}

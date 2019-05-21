using System;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using ProtoBuf;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface IFollowViewModel
    {
        JobConfiguration JobConfiguration { get; set; }
    }
    public class FollowViewModel : StartupBaseViewModel, IFollowViewModel
    {
        public FollowViewModel(IRegionManager region) : base(region)
        {
            NextCommand = new DelegateCommand(NevigateNext);
            PreviousCommand = new DelegateCommand(NevigatePrevious);
            JobConfiguration = new JobConfiguration
            {
                ActivitiesPerJobDisplayName = "LangKeyNumberOfFollowsPerJob".FromResourceDictionary(),
                ActivitiesPerHourDisplayName = "LangKeyNumberOfFollowsPerHour".FromResourceDictionary(),
                ActivitiesPerDayDisplayName = "LangKeyNumberOfFollowsPerDay".FromResourceDictionary(),
                ActivitiesPerWeekDisplayName = "LangKeyNumberOfFollowsPerWeek".FromResourceDictionary(),
                IncreaseActivityDisplayName = "LangKeyMaxFollowsPerDay".FromResourceDictionary(),

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
    }
    
}

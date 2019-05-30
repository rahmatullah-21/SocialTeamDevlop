using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Commands;
using Prism.Regions;
using DominatorHouseCore.Interfaces.StartUp;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommonServiceLocator;
using System.Windows.Input;
using System;
using DominatorHouseCore;
using DominatorUIUtility.Views.AccountSetting.CustomControl;
using System.Linq;

namespace DominatorUIUtility.ViewModel.Startup.ModuleConfig
{
    public interface ICommentViewModel
    {
    }
    public class CommentViewModel : StartupBaseViewModel, ICommentViewModel
    {
        public CommentViewModel(IRegionManager region) : base(region)
        {
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
            };
        }
     
        public void OnLoad(string activityType)
        {
            ListQueryType.Clear();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            ListQueryType = NetworkFactory.GetNetworkfactory(viewModel.SelectedNetwork).GetActivity(activityType).GetQueryType();
        }

    }
}

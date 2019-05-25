using Prism.Mvvm;
using System.Windows.Input;
using Prism.Regions;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using System;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore.Interfaces.StartUp;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.Startup
{
    public class StartupBaseViewModel : BindableBase
    {
        public IRegionManager regionManager;

        public static int selectedIndex = 0;
        public static List<string> NavigationList { get; set; }
        public StartupBaseViewModel(IRegionManager region)
        {
            regionManager = region;
            //LoadedCommand = new DelegateCommand<string>(OnLoad);
        }
        #region Commands
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        #endregion

        protected void NevigateNext()
        {
            if (selectedIndex >= NavigationList.Count - 1)
                return;
            selectedIndex++;
            var next = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", next);
        }
        protected void NevigatePrevious()
        {
            if (selectedIndex <= 0)
                return;
            selectedIndex--;
            var previous = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", previous);
        }
       

        //private List<string> _listQueryType = new List<string>();
        //public List<string> ListQueryType
        //{
        //    get
        //    {
        //        return _listQueryType;
        //    }
        //    set
        //    {
        //        SetProperty(ref _listQueryType, value);
        //    }
        //}
        //public virtual void OnLoad(string activityType)
        //{
        //    ListQueryType.Clear();
        //    var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
        //    //Enum.GetValues(typeof(QueryType)).Cast<QueryType>().ToList().ForEach(
        //    //         query =>
        //    //         {
        //    //             switch (viewModel.SelectedNetwork)
        //    //             {
        //    //                 case "Facebook":
        //    //                     if (query.IsFacebookActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Instagram":
        //    //                     if (query.IsInstagramActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Twitter":
        //    //                     if (query.IsTwitterActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Pinterest":
        //    //                     if (query.IsPinterestActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "LinkedIn":
        //    //                     if (query.IsLinkedInActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Reddit":
        //    //                     if (query.IsRedditActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Quora":
        //    //                     if (query.IsQuoraActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Youtube":
        //    //                     if (query.IsYoutubeActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //                 case "Tumblr":
        //    //                     if (query.IsTumblrActivity(activityType))
        //    //                         ListQueryType.Add(query.ToString());
        //    //                     break;
        //    //             }
                        
        //    //         });
        //}
    }
}

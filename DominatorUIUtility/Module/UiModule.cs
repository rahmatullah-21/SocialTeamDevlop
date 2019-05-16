using DominatorUIUtility.Views.AccountSetting;
using DominatorUIUtility.Views.AccountSetting.Activity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.Module
{
    public class UiModule:IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SelectActivity>();
            containerRegistry.RegisterForNavigation<Follow>();
            containerRegistry.RegisterForNavigation<Unfollow>();
            containerRegistry.RegisterForNavigation<JobConfig>();
            containerRegistry.RegisterForNavigation<QueryControl>();

        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
           
            regionManager.RegisterViewWithRegion("StartupRegion", typeof(SelectActivity));

        }
    }
}

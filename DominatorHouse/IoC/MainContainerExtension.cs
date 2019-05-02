using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouse.ViewModels;
using DominatorHouse.ViewModels.Startup;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel.Startup;
using Unity;
using Unity.Extension;

namespace DominatorHouse.IoC
{
    public class MainContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // views

            // view models
            Container.RegisterSingleton<IMainViewModel, MainViewModel>();
            Container.RegisterSingleton<IPerfCounterViewModel, PerfCounterViewModel>();
            Container.RegisterSingleton<IDominatorAutoActivityViewModel, DominatorAutoActivityViewModel>();

            Container.RegisterSingleton<IApplicationResourceProvider, ApplicationResourceProvider>();
            Container.RegisterSingleton<ISelectUserTypeViewModel, SelectUserTypeViewModel>();
            Container.RegisterSingleton<ISelectNetworkViewModel, SelectNetworkViewModel>();
            //Container.RegisterSingleton<ISelectActivityViewModel, SelectActivityViewModel>();
            Container.RegisterSingleton<ISaveSettingViewModel, SaveSettingViewModel>();

        }
    }
}

using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouse.ViewModels;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.ViewModel;
using Socinator.Social.AutoActivity.Views;
using Unity;
using Unity.Extension;

namespace DominatorHouse.IoC
{
    public class MainContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // views
            Container.RegisterSingleton<IDominatorAutoActivity, DominatorAutoActivity>();


            // view models
            Container.RegisterSingleton<IMainViewModel, MainViewModel>();
            Container.RegisterSingleton<IPerfCounterViewModel, PerfCounterViewModel>();
            Container.RegisterSingleton<IDominatorAutoActivityViewModel, DominatorAutoActivityViewModel>();

            Container.RegisterSingleton<IApplicationResourceProvider, ApplicationResourceProvider>();
        }
    }
}

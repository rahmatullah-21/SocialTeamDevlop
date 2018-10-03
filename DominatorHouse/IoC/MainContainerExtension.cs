using DominatorHouse.ViewModels;
using DominatorHouseCore.AppResources;
using Unity;
using Unity.Extension;

namespace DominatorHouse.IoC
{
    public class MainContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // specify name of the module (CurrentyNetwork.ToString()) when you think it will be many registration upon your interface
            Container.RegisterType<IMainViewModel, MainViewModel>();
            Container.RegisterType<IApplicationResourceProvider, ApplicationResourceProvider>();
        }
    }
}

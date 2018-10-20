using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.ProxyServerManagment;
using DominatorHouseCore.ViewModel;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore
{
    public class CoreUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IGlobalDatabaseConnection, GlobalDatabaseConnection>();

            Container.RegisterSingleton<ILogViewModel, LogViewModel>();

            Container.AddNewExtension<ViewModelUnityExtension>();
            Container.AddNewExtension<ProxyManagmentUnityExtension>();
        }
    }
}

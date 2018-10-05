using DominatorHouseCore.ViewModel;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore
{
    public class CoreUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<ILogViewModel, LogViewModel>();
            Container.AddNewExtension<ViewModelUnityExtension>();
        }
    }
}

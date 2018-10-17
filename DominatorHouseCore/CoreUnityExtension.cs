using DominatorHouseCore.FileManagers;
using DominatorHouseCore.ViewModel;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore
{
    public class CoreUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IAccountsCacheService, AccountsCacheService>();
            Container.RegisterSingleton<ITemplatesCacheService, TemplatesCacheService>();
            Container.AddNewExtension<ViewModelUnityExtension>();
        }
    }
}

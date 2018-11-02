using DominatorHouseCore.Dal;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
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
            Container.RegisterSingleton<IAccountsCacheService, AccountsCacheService>();
            Container.RegisterSingleton<ITemplatesCacheService, TemplatesCacheService>();

            Container.RegisterSingleton<IAccountGrowthPropertiesProvider, AccountGrowthPropertiesProvider>();


            Container.AddNewExtension<ViewModelUnityExtension>();
            Container.AddNewExtension<DbMigrationUnityExtension>();
        }
    }
}

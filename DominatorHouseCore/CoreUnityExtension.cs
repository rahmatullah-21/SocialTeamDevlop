using DominatorHouseCore.Dal;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.ProxyServerManagment;
using DominatorHouseCore.Settings;
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
            Container.RegisterSingleton<ITemplatesFileManager, TemplatesFileManager>();
            Container.RegisterSingleton<IGenericFileManager, GenericFileManager>();
            Container.RegisterSingleton<IAccountsFileManager, AccountsFileManager>();

            Container.RegisterSingleton<IAccountGrowthPropertiesProvider, AccountGrowthPropertiesProvider>();

            Container.RegisterSingleton<ISoftwareSettings, SoftwareSettings>();

            Container.RegisterSingleton<IWebService, WebService>();
            Container.RegisterSingleton<IDateProvider, DateProvider>();
            Container.RegisterSingleton<IFileSystemProvider, FileSystemProvider>();
            Container.RegisterSingleton<IJobActivityConfigurationManager, JobActivityConfigurationManager>();
            Container.RegisterSingleton<ICampaignsFileManager, CampaignsFileManager>();
            Container.RegisterSingleton<IBinFileHelper, BinFileHelper>();
            Container.RegisterSingleton<ILockFileConfigProvider, LockFileConfigProvider>();
            Container.RegisterSingleton<IProtoBuffBase, ProtoBuffBase>();
            Container.RegisterSingleton<IRunningJobsHolder, RunningJobsHolder>();
            Container.RegisterSingleton<ICampaignInteractionDetails, CampaignInteractionDetails>();
            Container.RegisterSingleton<IGlobalInteractionDetails, GlobalInteractionDetails>();
            Container.RegisterSingleton<ISoftwareSettingsFileManager, SoftwareSettingsFileManager>();

            Container.AddNewExtension<ViewModelUnityExtension>();
            Container.AddNewExtension<DbMigrationUnityExtension>();
            Container.AddNewExtension<ProxyManagmentUnityExtension>();
        }
    }
}

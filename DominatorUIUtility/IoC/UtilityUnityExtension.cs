using LegionUIUtility.CustomControl;
using LegionUIUtility.ViewModel;
using LegionUIUtility.ViewModel.OtherConfigurations;
using LegionUIUtility.ViewModel.OtherConfigurations.ThridPartyServices;
using LegionUIUtility.ViewModel.OtherTools;
using Unity;
using Unity.Extension;

namespace LegionUIUtility.IoC
{
    public class UtilityUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // Views
            Container.RegisterSingleton<AccountGrowthControl>();

            // View Models
            Container.RegisterSingleton<IOtherConfigurationViewModel, SoftwareSettingsViewModel>("SoftwareSettingsViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, SocinatorMacrosViewModel>("SocinatorMacrosViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, EmbeddedBrowserViewModel>("EmbeddedBrowserViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, QuoraViewModel>("QuoraViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, YoutubeViewModel>("YoutubeViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, InstagramUserViewModel>("InstagramUserViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, PinterestOtherConfigViewModel>("PinterestOtherConfigViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, LinkedInViewModel>("LinkedInViewModel");
            //Container.RegisterSingleton<IOtherConfigurationViewModel, ThirdPartyViewModel>("ThirdPartyViewModel");
            Container.RegisterSingleton<IThridPartyServicesViewModel, CaptchaServicesViewModel>("CaptchaServicesViewModel");
            Container.RegisterSingleton<IThridPartyServicesViewModel, UrlShortnerServicesViewModel>("UrlShortnerServicesViewModel");
            Container.RegisterSingleton<IThridPartyServicesViewModel, ImageCaptchaServicesViewModel>("ImageCaptchaServicesViewModel");
           
            Container.RegisterSingleton<IOtherToolsViewModel, MediaGeneratorViewModel>("MediaGeneratorViewModel");

            Container.RegisterSingleton<IProxyManagerViewModel, ProxyManagerViewModel>();
            Container.RegisterSingleton<IVerifyProxiesViewModel, VerifyProxiesViewModel>();

            Container.RegisterSingleton<IDominatorAccountViewModel, DominatorAccountViewModel>();

            Container.RegisterSingleton<IAccountGrowthControlViewModel, AccountGrowthControlViewModel>();
            Container.RegisterSingleton<IAccountCollectionViewModel, AccountCollectionViewModel>();
        }
    }
}

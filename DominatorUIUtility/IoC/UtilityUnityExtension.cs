using DominatorUIUtility.ViewModel.OtherConfigurations;
using DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices;
using Unity;
using Unity.Extension;

namespace DominatorUIUtility.IoC
{
    public class UtilityUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterSingleton<IOtherConfigurationViewModel, SoftwareSettingsViewModel>("SoftwareSettingsViewModel");
            Container.RegisterSingleton<IOtherConfigurationViewModel, SocinatorMacrosViewModel>("SocinatorMacrosViewModel");
            Container.RegisterSingleton<IOtherConfigurationViewModel, QuoraViewModel>("QuoraViewModel");
            Container.RegisterSingleton<IOtherConfigurationViewModel, LinkedInViewModel>("LinkedInViewModel");
            Container.RegisterSingleton<IOtherConfigurationViewModel, ThirdPartyViewModel>("ThirdPartyViewModel");

            Container.RegisterSingleton<IThridPartyServicesViewModel, CaptchaServicesViewModel>("CaptchaServicesViewModel");
            Container.RegisterSingleton<IThridPartyServicesViewModel, UrlShortnerServicesViewModel>("UrlShortnerServicesViewModel");
        }
    }
}

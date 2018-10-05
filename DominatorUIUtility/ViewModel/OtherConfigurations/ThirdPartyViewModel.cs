using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices;
using System.Windows;
using Unity;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class ThirdPartyViewModel : TablifiedContentControlViewModel<IThridPartyServicesViewModel>, IOtherConfigurationViewModel
    {
        public string Title { get; }
        public string TemplateName { get; }

        public ThirdPartyViewModel(IUnityContainer container) : base(container)
        {
            Title = Application.Current.FindResource("LangKeyThirdPartyServices")?.ToString();
            TemplateName = "TablifiedContentControlControlTemplate";
        }
    }
}

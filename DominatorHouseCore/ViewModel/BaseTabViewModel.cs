using CommonServiceLocator;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.Utility;
using System.Windows;

namespace DominatorHouseCore.ViewModel
{
    public abstract class BaseTabViewModel : BindableBase, ITabViewModel
    {
        public string Title { get; }
        public string TemplateName { get; }

        protected BaseTabViewModel(string titleResourceName, string templateName)
        {
            var serviceProvider = ServiceLocator.Current.TryResolve<IApplicationResourceProvider>();
            Title = serviceProvider.GetStringResource(titleResourceName);
            TemplateName = templateName;
        }
    }
}

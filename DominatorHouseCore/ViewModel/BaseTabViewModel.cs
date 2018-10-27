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
            Title = Application.Current.FindResource(titleResourceName)?.ToString();
            TemplateName = templateName;
        }
    }
}

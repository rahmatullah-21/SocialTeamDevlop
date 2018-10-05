using System.ComponentModel;

namespace DominatorHouseCore.ViewModel
{
    public interface ITabViewModel : INotifyPropertyChanged
    {
        string Title { get; }
        string TemplateName { get; }
    }
}

using System.Collections.ObjectModel;

namespace DominatorHouseCore.Models
{
    public interface IModuleSetting
    {
        JobConfiguration JobConfiguration { get; }
        ObservableCollection<QueryInfo> SavedQueries { get; }
    }
}

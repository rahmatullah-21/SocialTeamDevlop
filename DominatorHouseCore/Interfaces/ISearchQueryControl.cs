using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public interface ISearchQueryControl
    {        
        ObservableCollection<QueryInfo> SavedQueries { get; set; }
    }
}
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface ISearchQueryControl
    {        
        ObservableCollection<QueryInfo> SavedQueries { get; set; }
    }
}
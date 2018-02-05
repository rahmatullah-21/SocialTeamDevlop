using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public interface ISearchQueryControl
    {        
        ObservableCollectionBase<QueryInfo> SavedQueries { get; set; }
    }
}
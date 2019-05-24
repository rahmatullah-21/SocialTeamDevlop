using DominatorHouseCore.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DominatorHouseCore.Interfaces.StartUp
{
    public interface IStartUpSearchQuery
    {
        //List<string> ListQueryType { get; set; }
        ObservableCollection<QueryInfo> SavedQueries { get; set; }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Interfaces
{
    public interface IAccountToolsFactory
    {
        UserControl GetStartupToolsView();

        IEnumerable<ActivityType> GetImportantActivityTypes();

        string RecentlySelectedAccount { get; set; }
    }
}
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DominatorHouseCore.Models
{
    public class AccountWiseReportModel
    {
        public ObservableCollection<string> AccountList { get; set; } = new ObservableCollection<string>();

        public ICollectionView ReportCollection { get; set; }

        public string ModuleType { get; set; } = "";
    }
}

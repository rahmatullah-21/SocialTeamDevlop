using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models
{
    public class AccountWiseReportModel
    {
        public ObservableCollection<string> AccountList { get; set; } = new ObservableCollection<string>();

        public ICollectionView ReportCollection { get; set; }

        public string ModuleType { get; set; } = "";
    }
}

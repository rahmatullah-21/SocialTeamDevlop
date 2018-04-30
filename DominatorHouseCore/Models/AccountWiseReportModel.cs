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
        public ObservableCollection<ContentSelectGroup> AccountList { get; set; } = new ObservableCollection<ContentSelectGroup>();

        public ICollectionView ReportCollection { get; set; }

        public string ModuleType { get; set; } = "";
    }
}

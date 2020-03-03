using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models.FacebookModels
{
    public class InactiveUserFilterModel : BindableBase
    {

        private ObservableCollection<UnfriendReportModel> _lstReports;

        public ObservableCollection<UnfriendReportModel> LstReports
        {
            get { return _lstReports; }
            set { SetProperty(ref _lstReports, value); }
        }

        private ICollectionView _reportCollection;
        public ICollectionView ReportCollection
        {
            get { return _reportCollection; }
            set { SetProperty(ref _reportCollection, value); }
        }
        
    }
}

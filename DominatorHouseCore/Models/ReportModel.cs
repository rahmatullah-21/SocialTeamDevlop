using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using DominatorHouseCore.Utility;
using System.ComponentModel.DataAnnotations;

namespace DominatorHouseCore.Models
{
    public class ReportModel : BindableBase
    {
 
        public ObservableCollection<ContentSelectGroup> AccountList { get; set; } =new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> QueryList { get; set; } = new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> StatusList { get; set; } = new ObservableCollection<ContentSelectGroup>();

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public ICollectionView ReportCollection { get; set; } 

        public ObservableCollection<GridViewColumnDescriptor> GridViewColumn { get; set; }  = new ObservableCollection<GridViewColumnDescriptor>();

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Models
{
    public class ReportModel : BindableBase
    {

        public ObservableCollection<ContentSelectGroup> AccountList { get; set; } = new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> QueryList { get; set; } = new ObservableCollection<ContentSelectGroup>();

        public ObservableCollection<ContentSelectGroup> StatusList { get; set; } = new ObservableCollection<ContentSelectGroup>();

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        private ICollectionView _reportCollection;
        public ICollectionView ReportCollection
        {
            get { return _reportCollection; }
            set { SetProperty(ref _reportCollection, value); }
        }

        private ObservableCollection<object> _lstReports;

        public ObservableCollection<object> LstReports
        {
            get { return _lstReports; }
            set { SetProperty(ref _lstReports, value); }
        }
        public ActivityType ActivityType { get; set; }
        public ObservableCollection<GridViewColumnDescriptor> GridViewColumn { get; set; } = new ObservableCollection<GridViewColumnDescriptor>();
        public List<KeyValuePair<string, string>> LstCurrentQueries = new List<KeyValuePair<string, string>>();

        public string CampaignId { get; set; } = string.Empty;
        private bool _FollowRate;
        public bool FollowRate
        {
            get
            {
                return _FollowRate;
            }
            set
            {
                SetProperty(ref _FollowRate, value);
            }
        }
    }
}

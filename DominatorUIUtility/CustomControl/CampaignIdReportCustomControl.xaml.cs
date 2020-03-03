using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Extensions;
using DominatorHouseCore.Models.FacebookModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CampaignInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedUsers;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    /// Interaction logic for CampaignIdReportCustomControl.xaml
    /// </summary>
    public partial class CampaignIdReportCustomControl : UserControl, INotifyPropertyChanged
    {
        public CampaignIdReportCustomControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        private string CampaignId;
        public CampaignIdReportCustomControl(string campaignId)
        {
            InitializeComponent();
            CampaignId = campaignId;
            IntializeUsers(campaignId);
            MainGrid.DataContext = this;
        }

        private void IntializeUsers(string campaignId)
        {
            try
            {
                string activityType = ActivityType.ProfileScraper.ToString();
                var _dbOperations = ServiceLocator.Current.ResolveCampaignDbOperations(CampaignId ?? string.Empty, SocialNetworks.Facebook);
                var reportDetails = _dbOperations.Get<CampaignInteractedUsres>(x => x.ActivityType == activityType);
                InactiveUserFilterModel.LstReports = new ObservableCollection<UnfriendReportModel>();
                InactiveUserFilterModel.ReportCollection = CollectionViewSource.GetDefaultView(InactiveUserFilterModel.LstReports);
                Task.Factory.StartNew(() =>
                {
                    var currentId = 1;
                    reportDetails.ForEach(item =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                           InactiveUserFilterModel.LstReports.Add(new UnfriendReportModel()
                           {
                               Id = currentId++,
                               AccountEmail = item.AccountEmail,
                               QueryType = item.QueryType,
                               QueryValue = item.QueryValue,
                               UserId = item.UserId,
                               UserProfilePicUrl = $"https://graph.facebook.com/{item.UserId}/picture?type=large&redirect=true&width=400&height=400",
                               UserName = item.Username
                           }));
                        Thread.Sleep(10);
                    });
                });
            }
            catch (Exception ex) { }
        }

        private InactiveUserFilterModel _inactiveUserFilterModel = new InactiveUserFilterModel();
        public InactiveUserFilterModel InactiveUserFilterModel
        {
            get { return _inactiveUserFilterModel; }
            set
            {
                if (InactiveUserFilterModel == null || value == InactiveUserFilterModel)
                    return;
                OnPropertyChanged(nameof(InactiveUserFilterModel));
            }
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RefreshReport(object sender, RoutedEventArgs e)
        {
            try
            {
                IntializeUsers(CampaignId);
            }
            catch (Exception) { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : INotifyPropertyChanged
    {
        private ReportModel _reportModel = new ReportModel();

        public Reports()
        {
            InitializeComponent();
        }

        public Reports(CampaignDetails campaign) : this()
        {
            Campaign = campaign;
            ReportModel.CampaignId = campaign.CampaignId;
            ReportModel.ActivityType = (ActivityType) Enum.Parse(typeof(ActivityType), campaign.SubModule);
            MainGrid.DataContext = this;
        }

        public CampaignDetails Campaign { get; set; }

        public ReportModel ReportModel
        {
            get => _reportModel;
            set
            {
                if (_reportModel == value)
                    return;
                _reportModel = value;
                OnPropertyChanged(nameof(ReportModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = Regex.Replace(
                    $"{Campaign.CampaignName}-Reports[{DateTimeUtilities.GetEpochTime()}]",
                    "[\\/:*?<>|\"]",
                    "-");

                filename = $"{exportPath}\\{filename}.csv";
                var activityType = (ActivityType) Enum.Parse(typeof(ActivityType), Campaign.SubModule);

                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory().ReportFactory
                    .ExportReports(activityType, filename, ReportType.Campaign);
            }
            catch (Exception ex)
            {
                Dialog.ShowDialog("Fail", "Export failed !!");
                ex.DebugLog();
            }
        }

        private void ClkFollowRate(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory =
                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory();
            var reportDetails =
                networkCoreFactory.ReportFactory.GetReportDetail(ReportModel, ReportModel.LstCurrentQueries, Campaign);
            var reportControl = ServiceLocator.Current.GetInstance<IQueryFollowedControl>();
            reportControl.AssignReportDetailsToModel(reportDetails, Campaign);
            var objDialog = new Dialog();
            var win = objDialog.GetMetroWindow(reportControl, "Get Follow Rate");
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        private void RefreshReport(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory =
                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory();
            var reportDetails =
                networkCoreFactory.ReportFactory.GetReportDetail(ReportModel, ReportModel.LstCurrentQueries, Campaign);

            ReportModel.LstReports = new ObservableCollection<object>();
            ReportModel.ReportCollection =
                CollectionViewSource.GetDefaultView(ReportModel.LstReports);

            Task.Factory.StartNew(() =>
            {
                reportDetails.ForEach(item =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        ReportModel.LstReports.Add(item));
                    Thread.Sleep(10);
                });
            });
        }
    }
}
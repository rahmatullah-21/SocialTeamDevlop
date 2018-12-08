using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using System.Text.RegularExpressions;
using DominatorHouseCore.Enums;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl, INotifyPropertyChanged
    {
        public Reports()
        {
            InitializeComponent();
        }

        public Reports(CampaignDetails campaign) : this()
        {
            Campaign = campaign;
            ReportModel.CampaignId = campaign.CampaignId;
            ReportModel.ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaign.SubModule);
            MainGrid.DataContext = this;
        }
        public CampaignDetails Campaign { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ReportModel _reportModel = new ReportModel();
        public ReportModel ReportModel
        {
            get
            {
                return _reportModel;
            }
            set
            {
                if (_reportModel == value)
                    return;
                _reportModel = value;
                OnPropertyChanged(nameof(ReportModel));
            }
        }
        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = Regex.Replace(
                    input: $"{ Campaign.CampaignName }-Reports[{DateTimeUtilities.GetEpochTime()}]",
                    pattern: "[\\/:*?<>|\"]",
                    replacement: "-");

                filename = $"{exportPath}\\{filename}.csv";
                var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), Campaign.SubModule);

                SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory().ReportFactory.ExportReports(activityType, filename, ReportType.Campaign);
                Dialog.ShowDialog("Sucess", "Sucessfully Exported to " + filename);
                GlobusLogHelper.log.Info(Log.CustomMessage, Campaign.SocialNetworks, activityType, Campaign.CampaignName, "Sucessfully Exported to " + filename);
            }
            catch (Exception ex)
            {
                Dialog.ShowDialog("Fail", "Export failed !!");
                ex.DebugLog();

            }
        }

        private void RefreshReport(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory();
            networkCoreFactory.ReportFactory.GetReportDetail(ReportModel, ReportModel.LstCurrentQueries, Campaign);
        }
    }
}

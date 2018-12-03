using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using System.Text.RegularExpressions;
using System.Windows.Data;
using DominatorHouseCore.Enums;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Reports.xaml
    /// </summary>
    public partial class Reports : UserControl
    {
        public Reports()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public Reports(CampaignDetails campaign) : this()
        {
            Campaign = campaign;
           // ReportModel = new ReportModel();
            ReportModel.CampaignId = campaign.CampaignId;
            ReportModel.ActivityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaign.SubModule);
            MainGrid.DataContext = this;
        }
        public CampaignDetails Campaign { get; set; }
        public Reports(ReportModel ReportModel, CampaignDetails campaign) : this()
        {
            this.ReportModel = ReportModel;
            Campaign = campaign;
            MainGrid.DataContext = this;
        }
        private ReportModel _reportModel=new ReportModel();

        public ReportModel ReportModel
        {
            get { return _reportModel; }
            set { _reportModel = value; }
        }

        //public ReportModel ReportModel
        //{
        //    get { return (ReportModel)GetValue(ReportModelProperty); }
        //    set { SetValue(ReportModelProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for ReportModel.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ReportModelProperty =
        //    DependencyProperty.Register("ReportModel", typeof(ReportModel), typeof(Reports), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
        //    {
        //        BindsTwoWayByDefault = true
        //    });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
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

            var result=SocinatorInitialize.GetSocialLibrary(Campaign.SocialNetworks).GetNetworkCoreFactory().ReportFactory
                .GetReportDetail(ReportModel , ReportModel.LstCurrentQueries, Campaign);
           
        }
    }
}

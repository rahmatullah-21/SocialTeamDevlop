using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for CampaignAccountWiseReport.xaml
    /// </summary>
    public partial class CampaignAccountWiseReport : UserControl
    {
        public CampaignDetails currentCampaign { get; set; }
        public AccountWiseReportModel AccountWiseReport { get; set; }
        public CampaignAccountWiseReport()
        {
            InitializeComponent();
        }
        public CampaignAccountWiseReport(CampaignDetails currentCampaign) : this()
        {

            try
            {
                this.currentCampaign = currentCampaign;
                AccountWiseReport = new AccountWiseReportModel();
                AccountWiseReport.ModuleType = currentCampaign.SubModule;

                #region Update AccountList

                currentCampaign.SelectedAccountList.ToList().ForEach(acc =>
                {
                    DominatorAccountModel objDominatorAccountModel = AccountsFileManager.GetAccount(acc);

                    if (!AccountWiseReport.AccountList.Any(account =>
                        account == objDominatorAccountModel.AccountBaseModel.UserName))
                    {
                        AccountWiseReport.AccountList.Add(objDominatorAccountModel.AccountBaseModel.UserName);
                    }

                });

                #endregion

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

            MainGrid.DataContext = this;

        }

        private void CmbAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var accountId = AccountsFileManager.GetAll().FirstOrDefault(x =>
                    x.AccountBaseModel.AccountNetwork == currentCampaign.SocialNetworks &&
                    x.UserName == CmbAccounts.SelectedItem.ToString()).AccountId;

                var dbAccountoperation = new DbOperations(accountId, currentCampaign.SocialNetworks, ConstantVariable.GetAccountDb);
                if (ReportManager.GetAccountWiseReportDetail(this, dbAccountoperation) == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Account Wise Report", "Reports for "
                                                                                                                               + CmbAccounts.SelectedItem + " Campaign not available");
                    return;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            var filename = Regex.Replace(
                input: $"{ currentCampaign.CampaignName }-Reports[{ ConstantVariable.DateasFileName}]",
                pattern: "[\\/:*?<>|\"]",
                replacement: "-");

            filename = $"{exportPath}\\{filename}.csv";

            //Header for csv file columns
            string header = ReportManager.GetHeader();

            if (!File.Exists(filename))
            {
                using (var streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(header);
                }
            }

            //Export Reports to csv File
            ReportManager.ExportReports(currentCampaign.SubModule, filename);

        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = Regex.Replace(e.Column.Header.ToString(), "(\\B[A-Z])", " $1");
        }
    }
}

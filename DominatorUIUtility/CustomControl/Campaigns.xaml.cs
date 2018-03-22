
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using FluentScheduler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.BusinessLogic.Scheduler;
using System.Collections.ObjectModel;
using DominatorHouseCore.DatabaseHandler;
using System.IO;
using DominatorHouseCore.Converters;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Process;



namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl
    {
        private CampaignDetails objCampaignDetails;

        public static Action<TemplateModel, CampaignDetails, bool, Visibility, string, string> EditOrDuplicateCampaign { get; set; } =
            (t, c, b, v, s, s2) => GlobusLogHelper.log.Error($"Campaigns.EditOrDuplicateCampaign action handler wasn't set");

        public SocialNetworks SocialNetworks { get; set; }


        public Campaigns(SocialNetworks socialNetworks)
        {
            InitializeComponent();
            this.SocialNetworks = socialNetworks;
            objCampaignDetails = new CampaignDetails();

        }

        private void SetDataContext()
        {
            var data = CampaignsFileManager.GetCampaignByNetwork(DominatorHouseInitializer.ActiveSocialNetwork);

            SetComboBoxItemSource(DominatorHouseInitializer.ActiveSocialNetwork.ToString());

            MainGrid.DataContext = objCampaignDetails;

            objCampaignDetails.CampaignCollection = CollectionViewSource.GetDefaultView(data);
        }


        private void SetComboBoxItemSource(string networks)
        {
            List<string> lstCampaignType = new List<string>();

            lstCampaignType.Add("All");

            switch (networks)
            {
                case "Instagram":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        if (EnumDescriptionConverter.GetDescription(ConvertToEnum(name)).Contains("Instagram"))
                            lstCampaignType.Add(name);
                    }
                    break;
                case "Facebook":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        if (EnumDescriptionConverter.GetDescription(ConvertToEnum(name)).Contains("Facebook"))
                            lstCampaignType.Add(name);
                    }
                    break;
                case "Twitter":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        if (EnumDescriptionConverter.GetDescription(ConvertToEnum(name)).Contains("Twitter"))
                            lstCampaignType.Add(name);
                    }
                    break;
                case "PinInterest":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        if (EnumDescriptionConverter.GetDescription(ConvertToEnum(name)).Contains("PinInterest"))
                            lstCampaignType.Add(name);
                    }
                    break;
                case "Quora":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        if (EnumDescriptionConverter.GetDescription(ConvertToEnum(name)).Contains("Quora"))
                            lstCampaignType.Add(name);
                    }
                    break;
            }

            CmbCampaignType.ItemsSource = lstCampaignType;
        }

        private static ActivityType ConvertToEnum(string name)
        {
            return (ActivityType)Enum.Parse(typeof(ActivityType), name);
        }

        private void BtnCampaignSetting_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Button)sender).ContextMenu.DataContext = ((Button)sender).DataContext;
                ((Button)sender).ContextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Raises when campaign status changed:
        /// - Active/Paused
        /// - Switch to Campaigns tab
        /// - Create/Update Campaign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleActivatePause_Campaign(object sender, EventArgs e)
        {
            objCampaignDetails.ObjCampaignDetails =
                new ObservableCollectionBase<CampaignDetails>(CampaignsFileManager.GetCampaignByNetwork(DominatorHouseInitializer.ActiveSocialNetwork));
            var lstAccountDetails = AccountsFileManager.GetAll(DominatorHouseInitializer.ActiveSocialNetwork);

            var selectedCampaign = ((FrameworkElement)sender).DataContext as CampaignDetails;
            if (selectedCampaign == null)
                return;

            var module = (ActivityType)Enum.Parse(typeof(ActivityType), selectedCampaign.SubModule);

            // Update module configuration inside Account details and save it back
            foreach (var account in lstAccountDetails)
            {
                var moduleConfiguration = account.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(y => y.ActivityType == module);
                if (moduleConfiguration?.TemplateId == selectedCampaign.TemplateId)
                    moduleConfiguration.IsEnabled = (bool)(sender as ToggleSwitch).IsChecked;
            }

            AccountsFileManager.SaveAll(lstAccountDetails);


            // Run/Stop job process in campaigns
            try
            {
                bool isToggleActivate = (sender as ToggleSwitch).IsChecked ?? false;
                foreach (var campaign in objCampaignDetails.ObjCampaignDetails)
                {
                    if (campaign.CampaignName != selectedCampaign.CampaignName)
                        continue;

                    bool isCampaignAlreadyActive = campaign.Status == "Active";
                    if (isCampaignAlreadyActive == isToggleActivate)
                        continue;

                    if (isToggleActivate)
                    {
                        campaign.Status = "Active";

                        GlobusLogHelper.log.Info($"Starting campaign {campaign.CampaignName}");
                        foreach (var accountModel in lstAccountDetails.Where(x => selectedCampaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                        {
                            DominatorScheduler.ScheduleTodayJobs(accountModel, SocialNetworks.Instagram, module);
                        }
                    }
                    else
                    {
                        campaign.Status = "Paused";
                        campaign.SelectedAccountList.ForEach(acc =>
                        {
                            DominatorScheduler.StopActivity(acc, campaign.SubModule, campaign.TemplateId);
                        });
                    }

                }

                CampaignsFileManager.Save(objCampaignDetails.ObjCampaignDetails.ToList());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var testResult = CampaignsFileManager.Get();
        }


        private void EditCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            var campaignDetails = CampaignsFileManager.GetCampaignById(campName.CampaignId);

            var templateDetails = TemplatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            EditOrDuplicateCampaign(templateDetails, campaignDetails, false, Visibility.Visible, ConstantVariable.UpdateCampaign, campaignDetails.TemplateId);
        }

        private void DuplicateCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            var campaignDetails = CampaignsFileManager.GetCampaignById(campName.CampaignId);

            var templateDetails = TemplatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            campaignDetails.CampaignName = campName.CampaignName.Split('[')[0] + $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

            EditOrDuplicateCampaign(templateDetails, campaignDetails, true, Visibility.Collapsed, ConstantVariable.CreateCampaign, campaignDetails.TemplateId);

        }


        private void DeleteSingleCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campaign = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirmation", "If you delete it will delete [ " + campaign.CampaignName + " ] Campaign permanently from campaign\nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton());
            if (dialogResult != MessageDialogResult.Affirmative)
                return;

            CampaignsFileManager.Delete(campaign);

            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(
                CampaignsFileManager.GetCampaignByNetwork(DominatorHouseInitializer.ActiveSocialNetwork));

            var allAccounts = AccountsFileManager.GetAll(DominatorHouseInitializer.ActiveSocialNetwork);

            // remove template from each account
            allAccounts.ForEach(x =>
            {
                var moduleConfig = x.ActivityManager.LstModuleConfiguration.FirstOrDefault(mc => mc.TemplateId == campaign.TemplateId);

                if (moduleConfig != null)
                {
                    // Stop active task related to campaign
                    JobProcess.Stop(x.AccountId, campaign.TemplateId);

                    // Remove task from list
                    x.ActivityManager.LstModuleConfiguration.RemoveAll(y => y.TemplateId == campaign.TemplateId);
                }
            });

            AccountsFileManager.SaveAll(allAccounts);

            GlobusLogHelper.log.Info(campaign.CampaignName + "  Campaign deleted permanently from campaigns.");
            SetDataContext();
        }

        private void CampaignReports_OnClick(object sender, RoutedEventArgs e)
        {
            Reports ObjReports = new Reports();

            ObjReports.ReportModel = new ReportModel();

            Dialog objDialog = new Dialog();


            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            ObjReports.ReportModel.ModuleType = campName.SubModule;

            var ActivitySettings = TemplatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

            ObservableCollectionBase<QueryInfo> lstSavedQuery = ReportManager.GetSavedQuery(campName.SubModule, ActivitySettings);


            Dictionary<string, string> lstCurrentQueries = new Dictionary<string, string>();

            lstSavedQuery.ToList().ForEach(x =>
            {
                lstCurrentQueries.Add(x.QueryValue, x.QueryType.ToString());

                #region Update QueryList for combobox

                if (ObjReports.ReportModel.QueryList.Any(query => query.Content == x.QueryType) == false)
                    ObjReports.ReportModel.QueryList.Add(new ContentSelectGroup() { IsContentSelected = false, Content = x.QueryType });

                #endregion

            });

            try
            {
                #region Update AccountList & StatusList for combobox

                campName.SelectedAccountList.ToList().ForEach(acc =>
                {
                    DominatorAccountModel objDominatorAccountModel = AccountsFileManager.GetAccount(acc);

                    ObjReports.ReportModel.AccountList.Add(new ContentSelectGroup()
                    {
                        IsContentSelected = false,
                        Content = objDominatorAccountModel.AccountBaseModel.UserName
                    });

                    if (ObjReports.ReportModel.StatusList.Count > 1 &&
                        ObjReports.ReportModel.StatusList.Any(status => status.Content == objDominatorAccountModel.AccountBaseModel.Status) ==
                        false)
                        ObjReports.ReportModel.StatusList.Add(new ContentSelectGroup()
                        {
                            IsContentSelected = false,
                            Content = objDominatorAccountModel.AccountBaseModel.Status
                        });

                });
                #endregion

                DominatorHouseCore.DatabaseHandler.CoreModels.DataBaseConnection dataBase =
                   DataBaseHandler.GetDataBaseConnectionInstance(campName.CampaignId, SocialNetworks,DatabaseType.CampaignType);

                ObservableCollection<Object> ReportDetail = ReportManager.GetReportDetail(ObjReports, lstCurrentQueries, dataBase, campName);
                if (ReportDetail.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Report", "Reports for " + campName.CampaignName + " Campaign not available", MessageDialogStyle.Affirmative);
                    return;
                }

                ObjReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReportDetail);

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

            Window win = objDialog.GetMetroWindow(ObjReports, "Reports");

            ObjReports.ExportReport.Click += (senders, events) =>
            {
                var exportPath = FileUtilities.GetExportPath();

                if (string.IsNullOrEmpty(exportPath))
                    return;

                var filename = $"{exportPath}\\{campName.CampaignName}-Reports [{ConstantVariable.DateasFileName}].csv";

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
                ReportManager.ExportReports(campName.SubModule, filename);

            };

            win.ShowDialog();
        }



        private void Campaign_Loaded(object sender, RoutedEventArgs e)
        {
            SetDataContext();
        }

        private void list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridViewColumns.SetGridViewColumnsWidthToStartWidth(list, e);
        }

        private void CmbCampaignType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<CampaignDetails> moduleWiseDetail = null;

            if (!CmbCampaignType.SelectedItem.Equals("All"))
            {
                moduleWiseDetail = CampaignsFileManager.GetCampaignByNetwork(DominatorHouseInitializer.ActiveSocialNetwork)
                       .Where(x => x.SubModule == CmbCampaignType.SelectedItem.ToString()).ToList();

            }
            else
            {
                moduleWiseDetail = CampaignsFileManager.GetCampaignByNetwork(DominatorHouseInitializer.ActiveSocialNetwork);
            }
            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(moduleWiseDetail);
            objCampaignDetails.CampaignCollection = CollectionViewSource.GetDefaultView(objCampaignDetails.ObjCampaignDetails);

        }
    }
}

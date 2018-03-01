using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FluentScheduler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl
    {
        private CampaignDetails objCampaignDetails;

        private Campaigns()
        {
            InitializeComponent();

            objCampaignDetails = new CampaignDetails();
        }

        private void SetDataContext()
        {
            var data = CampaignsFileManager.Get();

            objCampaignDetails.CampaignCollection = CollectionViewSource.GetDefaultView(data);

            MainGrid.DataContext = objCampaignDetails;
        }

        static Campaigns ObjCampaigns = null;
        public static Campaigns GetSingeltonCampaignsObject(SocialNetworks networks)
        {

            if (ObjCampaigns == null)
                ObjCampaigns = new Campaigns();
            ObjCampaigns.SetComboBoxItems(networks.ToString());
            return ObjCampaigns;
        }



        private void SetComboBoxItems(string networks)
        {
            CmbCampaignType.Items.Clear();
            CmbCampaignType.Items.Add("All");
            switch (networks)
            {
                case "Instagram":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        CmbCampaignType.Items.Add(name);
                    }
                    break;
                case "Facebook":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        CmbCampaignType.Items.Add(name);
                    }
                    break;
                case "Twitter":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        CmbCampaignType.Items.Add(name);
                    }
                    break;
                case "PinInterest":
                    foreach (var name in Enum.GetNames(typeof(ActivityType)))
                    {
                        CmbCampaignType.Items.Add(name);
                    }
                    break;
            }
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
                Console.WriteLine("");
            }
        }
        private void CampaignStatusChanged(object sender, EventArgs e)
        {
            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(CampaignsFileManager.Get());
            var lstAccountDetails = AccountsFileManager.GetAll();

            var campaignDetails = ((FrameworkElement)sender).DataContext as CampaignDetails;
            if (campaignDetails == null)
                return;
            var module = (ActivityType)Enum.Parse(typeof(ActivityType), campaignDetails.SubModule);

            foreach (var account in lstAccountDetails)
            {
                var moduleConfiguration = account.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(y => y.ActivityType == module);
                if (moduleConfiguration != null && moduleConfiguration.TemplateId == campaignDetails.TemplateId)
                {
                    moduleConfiguration.IsEnabled = (bool)(sender as ToggleSwitch).IsChecked;
                    AccountsFileManager.Edit(account);
                }
            }


            try
            {
                Schedule schedule = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == $"{module}-{campaignDetails.TemplateId}");
                if ((sender as ToggleSwitch)?.IsChecked ?? false)
                {
                    if (schedule != null)
                    {
                        if (schedule.Disabled)
                        {
                            var accounts = AccountsFileManager.GetAll();
                            foreach (var account in accounts)
                            {
                                if (campaignDetails.SelectedAccountList.Contains(account.AccountBaseModel.UserName))
                                    DominatorScheduler.ScheduleTodayJobs(account, SocialNetworks.Instagram, module);
                            }
                            schedule.Enable();
                        }
                        else
                        {
                            schedule?.Disable();
                        }
                    }
                }
                else
                {
                    campaignDetails.SelectedAccountList.ForEach(x =>
                    {
                        DominatorScheduler.StopActivity(x, module.ToString(), campaignDetails.TemplateId);
                    });
                    GlobusLogHelper.log.Info(module + "-" + campaignDetails.TemplateId + "  Job is disabled");

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(ex.Message);
                ex.DebugLog();
            }


            try
            {
                foreach (var campaign in objCampaignDetails.ObjCampaignDetails)
                {
                    if (campaign.CampaignName == campaignDetails.CampaignName)
                    {

                        if ((sender as ToggleSwitch).IsChecked ?? false)
                        {
                            campaign.Status = "Active";
                            foreach (var accountModel in lstAccountDetails.Where(x => campaignDetails.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                            {
                                DominatorScheduler.ScheduleTodayJobs(accountModel, SocialNetworks.Instagram, module);
                            }
                        }
                        else
                        {
                            campaign.Status = "Paused";
                            campaign.SelectedAccountList.ForEach(x =>
                            {
                                TaskAndThreadUtility.StopTask(x, campaignDetails.TemplateId);
                            });

                        }
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

            CampaignManager.EditOrDuplicateCampaign(templateDetails, campaignDetails, false, Visibility.Visible, ConstantVariable.UpdateCampaign, campaignDetails.TemplateId);
        }

        private void DuplicateCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            var campaignDetails = CampaignsFileManager.GetCampaignById(campName.CampaignId);

            var templateDetails = TemplatesFileManager.GetTemplateById(campaignDetails.TemplateId);

            campaignDetails.CampaignName = campName.CampaignName.Split('[')[0] + $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            CampaignManager.EditOrDuplicateCampaign(templateDetails, campaignDetails, true, Visibility.Collapsed, ConstantVariable.CreateCampaign, campaignDetails.TemplateId);

        }


        private void DeleteSingleCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campaign = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirmation", "If you delete it will delete [ " + campaign.CampaignName + " ] Campaign permanently from campaign\nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton());
            if (dialogResult != MessageDialogResult.Affirmative)
                return;

            CampaignsFileManager.Delete(campaign);

            objCampaignDetails.ObjCampaignDetails = new ObservableCollectionBase<CampaignDetails>(BinFileHelper.GetCampaignDetail());
            
            var allAccounts = AccountsFileManager.GetAll();

            // remove template from each account
            allAccounts.ForEach(x => {                
                var moduleConfig = x.ActivityManager.LstModuleConfiguration.FirstOrDefault(mc => mc.TemplateId == campaign.TemplateId);

                if (moduleConfig != null)
                {
                    // Stop active task related to campaign
                    TaskAndThreadUtility.StopTask(x.AccountId, campaign.TemplateId);

                    // Remove task from list
                    x.ActivityManager.LstModuleConfiguration.RemoveAll(y => y.TemplateId == campaign.TemplateId);
                }
            });            

            AccountsFileManager.SaveAll(allAccounts);

            GlobusLogHelper.log.Info(campaign.CampaignName + "  Campaign deleted permanently from campaigns.");
            SetDataContext();
        }


        public string ReportHeader = string.Empty;
        private void CampaignReports_OnClick(object sender, RoutedEventArgs e)
        {
            //Reports ObjReports = new Reports();

            //ObjReports.ReportModel = new ReportModel();

            //Dialog objDialog = new Dialog();

            //CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            //ObjReports.ReportModel.ModuleType = campName.SubModule;

            //var ActivitySettings = TemplatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

            //ObservableCollectionBase<QueryInfo> lstSavedQuery = ReportManager.GetSavedQuery(campName.SubModule, ActivitySettings);


            //Dictionary<string, string> lstCurrentQueries = new Dictionary<string, string>();

            //lstSavedQuery.ToList().ForEach(x =>
            //{
            //    lstCurrentQueries.Add(x.QueryValue, x.QueryType.ToString());

            //    #region Update QueryList for combobox

            //    if (ObjReports.ReportModel.QueryList.Any(query => query.Content == x.QueryType) == false)
            //        ObjReports.ReportModel.QueryList.Add(new ContentSelectGroup() { IsContentSelected = false, Content = x.QueryType });

            //    #endregion

            //});

            //try
            //{
            //    #region Update AccountList & StatusList for combobox

            //    campName.SelectedAccountList.ToList().ForEach(acc =>
            //    {
            //        DominatorAccountModel objDominatorAccountModel = AccountsFileManager.GetAccount(acc);

            //        ObjReports.ReportModel.AccountList.Add(new ContentSelectGroup()
            //        {
            //            IsContentSelected = false,
            //            Content = objDominatorAccountModel.AccountBaseModel.UserName
            //        });

            //        if (ObjReports.ReportModel.StatusList.Count > 1 &&
            //            ObjReports.ReportModel.StatusList.Any(status => status.Content == objDominatorAccountModel.AccountBaseModel.Status) ==
            //            false)
            //            ObjReports.ReportModel.StatusList.Add(new ContentSelectGroup()
            //            {
            //                IsContentSelected = false,
            //                Content = objDominatorAccountModel.AccountBaseModel.Status
            //            });

            //    });
            //    #endregion

            //    DataBaseConnectionCodeFirst.DataBaseConnection dataBase =
            //       DataBaseHandler.GetDataBaseConnectionInstance(campName.CampaignId, DatabaseType.CampaignType);

            //    ObservableCollection<Object> ReportDetail = ReportManager.GetReportDetail(ObjReports, lstCurrentQueries, dataBase, campName);

            //    ObjReports.ReportModel.ReportCollection = CollectionViewSource.GetDefaultView(ReportDetail);

            //}
            //catch (Exception ex)
            //{
            //    GlobusLogHelper.log.Error(ex.Message);
            //}

            //Window win = objDialog.GetMetroWindow(ObjReports, "Reports");

            //ObjReports.ExportReport.Click += (senders, events) =>
            //{
            //    var exportPath = FileUtilities.GetExportPath();

            //    if (string.IsNullOrEmpty(exportPath))
            //        return;

            //    var filename = $"{exportPath}\\{campName.CampaignName}-Reports [{ConstantVariable.DateasFileName}].csv";

            //    //Header for csv file columns
            //    string header = ReportManager.GetHeader();

            //    if (!File.Exists(filename))
            //    {
            //        using (var streamWriter = new StreamWriter(filename, true))
            //        {
            //            streamWriter.WriteLine(header);
            //        }
            //    }

            //    //Export Reports to csv File
            //    ReportManager.ExportReports(campName.SubModule, filename);

            //};

            //win.ShowDialog();

        }

        private void Campaign_Loaded(object sender, RoutedEventArgs e)
        {
            SetDataContext();

        }

        private void list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridViewColumns.SetGridViewColumnsWidthToStartWidth(list, e);

        }


    }
}

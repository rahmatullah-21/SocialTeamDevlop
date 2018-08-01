
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
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
using System.Text.RegularExpressions;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Process;
using DominatorHouseCore.DatabaseHandler.Utility;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl
    {

        private readonly CampaignDetails objCampaignDetails;
        private bool IsUnCheckedFromCampaignDetails { get; set; }


        public SocialNetworks SocialNetworks { get; set; }


        public Campaigns(SocialNetworks socialNetworks)
        {
            InitializeComponent();
            this.SocialNetworks = socialNetworks;
            objCampaignDetails = new CampaignDetails();
        }

        private void SetComboBoxItemSource(SocialNetworks networks)
        {
            List<string> lstCampaignType = new List<string>();

            lstCampaignType.Add("All");
            CmbCampaignType.SelectedIndex = 0;

            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                if (EnumDescriptionConverter.GetDescription(ConvertToEnum(name)).Contains(networks.ToString()))
                    lstCampaignType.Add(name);
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
        //private void ToggleActivatePause_Campaign(object sender, RoutedEventArgs e)
        //{
        //    //var ActivatePauseAction = new Func<object, RoutedEventArgs>(ActivatePause);
        //    ThreadStart starter = delegate { ActivatePause(sender, e); };
            
           
        //    Action starting = ()  => ActivatePause(sender,e);
        //    ThreadFactory.Instance.Start(() =>
        //    {
                
        //    });
        //}

        //private void ActivatePause(object sender, RoutedEventArgs e)
        //{
        //    var objCampaignDetailsBeforeSave = objCampaignDetails.ObjCampaignDetails;
        //    objCampaignDetails.ObjCampaignDetails =
        //        new ObservableCollection<CampaignDetails>(CampaignsFileManager.GetCampaignByNetwork(SocinatorInitialize.ActiveSocialNetwork));
        //    var lstAccountDetails = AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);

        //    var selectedCampaign = ((FrameworkElement)sender).DataContext as CampaignDetails;
        //    if (selectedCampaign == null)
        //        return;

        //    var module = (ActivityType)Enum.Parse(typeof(ActivityType), selectedCampaign.SubModule);

        //    // Update module configuration inside Account details and save it back
        //    foreach (var account in lstAccountDetails)
        //    {
        //        var moduleConfiguration = account.ActivityManager.LstModuleConfiguration
        //            .FirstOrDefault(y => y.ActivityType == module);
        //        if (moduleConfiguration?.TemplateId == selectedCampaign.TemplateId)
        //            moduleConfiguration.IsEnabled = (bool)(sender as ToggleSwitch).IsChecked;
        //        var socinatorAccountBuilder = new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
        //            .AddOrUpdateModuleSettings(module, moduleConfiguration)
        //            .SaveToBinFile();

        //    }

        //    // AccountsFileManager.SaveAll(lstAccountDetails);

        //    //AccountsFileManager.UpdateAccounts(lstAccountDetails);

        //    // Run/Stop job process in campaigns
        //    try
        //    {
        //        bool isToggleActivate = (sender as ToggleSwitch).IsChecked ?? false;
        //        foreach (var campaign in objCampaignDetails.ObjCampaignDetails)
        //        {
        //            if (campaign.CampaignId != selectedCampaign.CampaignId)
        //                continue;

        //            bool isCampaignAlreadyActive = campaign.Status == "Active";
        //            if (isCampaignAlreadyActive == isToggleActivate)
        //                continue;

        //            if (isToggleActivate)
        //            {
        //                campaign.Status = "Active";
        //                GlobusLogHelper.log.Info(Log.ActivatedCampaign, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);

        //                foreach (var accountModel in lstAccountDetails.Where(x => selectedCampaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
        //                {
        //                    DominatorScheduler.ScheduleNextActivity(accountModel, module);
        //                }
        //            }
        //            else
        //            {
        //                campaign.Status = "Paused";
        //                GlobusLogHelper.log.Info(Log.CampaignPaused, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);
        //                foreach (var accountModel in lstAccountDetails.Where(x => campaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
        //                {
        //                    DominatorScheduler.StopActivity(accountModel, campaign.SubModule, campaign.TemplateId, true);
        //                }
        //            }
        //        }

        //        // CampaignsFileManager.Save(objCampaignDetails.ObjCampaignDetails.ToList());

        //        CampaignsFileManager.UpdateCampaigns(objCampaignDetails.ObjCampaignDetails.ToList());
        //    }

        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    objCampaignDetails.ObjCampaignDetails = objCampaignDetailsBeforeSave;
        //}

        /// <summary>
        /// Raises when campaign status changed:
        /// - Active/Paused
        /// - Switch to Campaigns tab
        /// - Create/Update Campaign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleActivatePause_Campaign(object sender, RoutedEventArgs e)
        {
            var selectedCampaign = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var isChecked = ((ToggleSwitch)sender).IsChecked;
            var isToggleSwitchSelected = isChecked != null && (bool)isChecked;

            ThreadFactory.Instance.Start(() =>
            {
                ActivePauseCampaign(selectedCampaign, isToggleSwitchSelected);
            });
        }


        ImmutableQueue<Action> updatingAccountsBinFiles = ImmutableQueue<Action>.Empty;

        bool allUpdatingBinfilesQueued;

        private void ActivePauseCampaign(CampaignDetails selectedCampaign, bool isToggleSwitchSelected)
        {
            var objCampaignDetailsBeforeSave = objCampaignDetails.ObjCampaignDetails;
            objCampaignDetails.ObjCampaignDetails =
                new ObservableCollection<CampaignDetails>(CampaignsFileManager.GetCampaignByNetwork(SocinatorInitialize.ActiveSocialNetwork));
            var lstAccountDetails = AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);

            if (selectedCampaign == null)
                return;

            var module = (ActivityType)Enum.Parse(typeof(ActivityType), selectedCampaign.SubModule);

            // Update module configuration inside Account details and save it back
            //foreach (var account in lstAccountDetails)
            //{
            //    var moduleConfiguration = account.ActivityManager.LstModuleConfiguration
            //        .FirstOrDefault(y => y.ActivityType == module);
            //    if (moduleConfiguration?.TemplateId == selectedCampaign.TemplateId)
            //        moduleConfiguration.IsEnabled = isToggleSwitchSelected;
            //    var socinatorAccountBuilder = new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
            //        .AddOrUpdateModuleSettings(module, moduleConfiguration)
            //        .SaveToBinFile();

            //}

            try
            {
                new Thread(() =>
                {
                    while (!allUpdatingBinfilesQueued)
                    {
                        Thread.Sleep(50);
                        while (!updatingAccountsBinFiles.IsEmpty)
                        {
                            Action act;
                            updatingAccountsBinFiles = updatingAccountsBinFiles.Dequeue(out act);
                            act();
                        }
                    }
                })
                { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            lstAccountDetails.ForEach(account =>
            {


                updatingAccountsBinFiles= updatingAccountsBinFiles.Enqueue(() =>
                {
                    UpdateAccountCampaignsStatus(selectedCampaign, isToggleSwitchSelected, account, module);
                });

            });


            allUpdatingBinfilesQueued = true;
            // AccountsFileManager.SaveAll(lstAccountDetails);

            //AccountsFileManager.UpdateAccounts(lstAccountDetails);

            // Run/Stop job process in campaigns
            try
            {
                bool isToggleActivate = isToggleSwitchSelected;
                objCampaignDetails.ObjCampaignDetails.ForEach(campaign =>
                {
                    if (campaign.CampaignId != selectedCampaign.CampaignId)
                        return;

                    bool isCampaignAlreadyActive = campaign.Status == "Active";
                    if (isCampaignAlreadyActive == isToggleActivate)
                        return;

                    if (isToggleActivate)
                    {
                        campaign.Status = "Active";
                        GlobusLogHelper.log.Info(Log.ActivatedCampaign, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);

                        foreach (var accountModel in lstAccountDetails.Where(x => selectedCampaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                        {
                            DominatorScheduler.ScheduleNextActivity(accountModel, module);
                        }
                    }
                    else
                    {
                        campaign.Status = "Paused";
                        GlobusLogHelper.log.Info(Log.CampaignPaused, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);
                        foreach (var accountModel in lstAccountDetails.Where(x => campaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                        {
                            DominatorScheduler.StopActivity(accountModel, campaign.SubModule, campaign.TemplateId, true);
                        }
                    }
                });
                //foreach (var campaign in objCampaignDetails.ObjCampaignDetails)
                //{
                //    if (campaign.CampaignId != selectedCampaign.CampaignId)
                //        continue;

                //    bool isCampaignAlreadyActive = campaign.Status == "Active";
                //    if (isCampaignAlreadyActive == isToggleActivate)
                //        continue;

                //    if (isToggleActivate)
                //    {
                //        campaign.Status = "Active";
                //        GlobusLogHelper.log.Info(Log.ActivatedCampaign, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);

                //        foreach (var accountModel in lstAccountDetails.Where(x => selectedCampaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                //        {
                //            DominatorScheduler.ScheduleNextActivity(accountModel, module);
                //        }
                //    }
                //    else
                //    {
                //        campaign.Status = "Paused";
                //        GlobusLogHelper.log.Info(Log.CampaignPaused, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);
                //        foreach (var accountModel in lstAccountDetails.Where(x => campaign.SelectedAccountList.Contains(x.AccountBaseModel.UserName)))
                //        {
                //            DominatorScheduler.StopActivity(accountModel, campaign.SubModule, campaign.TemplateId, true);
                //        }
                //    }
                //}

                // CampaignsFileManager.Save(objCampaignDetails.ObjCampaignDetails.ToList());

                CampaignsFileManager.UpdateCampaigns(objCampaignDetails.ObjCampaignDetails.ToList());
            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
            objCampaignDetails.ObjCampaignDetails = objCampaignDetailsBeforeSave;
        }

        private static void UpdateAccountCampaignsStatus(CampaignDetails selectedCampaign, bool isToggleSwitchSelected, DominatorAccountModel account, ActivityType module)
        {
            var moduleConfiguration = account.ActivityManager.LstModuleConfiguration
                .FirstOrDefault(y => y.ActivityType == module);

            if (moduleConfiguration?.TemplateId == selectedCampaign.TemplateId)
                moduleConfiguration.IsEnabled = isToggleSwitchSelected;

            var socinatorAccountBuilder = new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                .AddOrUpdateModuleSettings(module, moduleConfiguration)
                .SaveToBinFile();
        }

        private void EditCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ViewCampaigns
                               .ViewCampaigns(campName.CampaignId, ConstantVariable.UpdateCampaign);
        }

        private void DuplicateCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;
            CampaignsFileManager.GetCampaignById(campName.CampaignId).
             CampaignName = campName.CampaignName.
             Split('[')[0] + $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
            SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ViewCampaigns
                               .ViewCampaigns(campName.CampaignId, ConstantVariable.CreateCampaign);

        }


        private void DeleteSingleCampaign_OnClick(object sender, RoutedEventArgs e)
        {
            CampaignDetails campaign = ((FrameworkElement)sender).DataContext as CampaignDetails;
            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirmation", "If you delete it will delete [ " + campaign.CampaignName + " ] Campaign permanently from campaign\nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            var selectedAccount = campaign.SelectedAccountList;

            CampaignsFileManager.Delete(campaign);

            objCampaignDetails.ObjCampaignDetails = new ObservableCollection<CampaignDetails>(
                CampaignsFileManager.GetCampaignByNetwork(SocinatorInitialize.ActiveSocialNetwork));

            var allAccounts = AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
            UpdateAccount(allAccounts, campaign, selectedAccount);
            GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork, campaign.CampaignName);


            SetDefaultView();
        }

        private void CampaignReports_OnClick(object sender, RoutedEventArgs e)
        {

            ReportModel ReportModel = new ReportModel();
            Reports ObjReports = new Reports(ReportModel);

            Dialog objDialog = new Dialog();


            CampaignDetails campName = ((FrameworkElement)sender).DataContext as CampaignDetails;

            ObjReports.ReportModel.ModuleType = campName.SubModule;

            var ActivitySettings = TemplatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

            var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campName.SubModule);

            ObservableCollection<QueryInfo> lstSavedQuery = SocinatorInitialize
                .GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ReportFactory
                .GetSavedQuery(activityType, ActivitySettings);

            List<KeyValuePair<string, string>> lstCurrentQueries = new List<KeyValuePair<string, string>>();


            lstSavedQuery?.ToList().ForEach(x =>
            {
                lstCurrentQueries.Add(new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));
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
                    DominatorAccountModel objDominatorAccountModel = AccountsFileManager.GetAccount(acc, campName.SocialNetworks);

                    ObjReports.ReportModel.AccountList.Add(new ContentSelectGroup()
                    {
                        IsContentSelected = false,
                        Content = objDominatorAccountModel.AccountBaseModel.UserName
                    });

                    if (ObjReports.ReportModel.StatusList.Count > 1 &&
                        ObjReports.ReportModel.StatusList.Any(status => status.Content == objDominatorAccountModel.AccountBaseModel.Status.ToString()) ==
                        false)
                        ObjReports.ReportModel.StatusList.Add(new ContentSelectGroup()
                        {
                            IsContentSelected = false,
                            Content = objDominatorAccountModel.AccountBaseModel.Status.ToString()
                        });

                });
                #endregion

                var dataBase = new DbOperations(campName.CampaignId, SocialNetworks,
                    ConstantVariable.GetCampaignDb);
                //DataBaseConnectionCampaign dataBase =
                //   DataBaseHandler.GetDataBaseConnectionCampaignInstance(campName.CampaignId, SocialNetworks);

                if (SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ReportFactory.GetReportDetail(ObjReports.ReportModel, lstCurrentQueries, campName) == 0)
                //  if (ReportManager.GetReportDetail(ObjReports, lstCurrentQueries, dataBase, campName) == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Report", "Reports for " + campName.CampaignName + " Campaign not available", MessageDialogStyle.Affirmative);
                    return;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

            Window win = objDialog.GetMetroWindow(ObjReports, "Reports");
            win.Owner = Application.Current.MainWindow;
            ObjReports.ExportReport.Click += (senders, events) =>
            {
                try
                {
                    var exportPath = FileUtilities.GetExportPath(win);

                    if (string.IsNullOrEmpty(exportPath))
                        return;

                    var filename = Regex.Replace(
                        input: $"{ campName.CampaignName }-Reports[{ DateTimeUtilities.GetEpochTime()}]",
                        pattern: "[\\/:*?<>|\"]",
                        replacement: "-");

                    filename = $"{exportPath}\\{filename}.csv";
                    //TODO

                    SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ReportFactory.ExportReports(activityType, filename, ReportType.Campaign);
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Sucess",
                        "Sucessfully Exported to " + filename);
                }
                catch (Exception)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Fail",
                        "Export fail !!");

                }

            };
            ObjReports.CmbQueries.SelectionChanged += (senders, events) =>
            {
                ReportManager.FilterByQueryType(ObjReports.CmbQueries.SelectedItem.ToString(), ReportModel);
            };
            win.ShowDialog();


        }

        private void Campaign_Loaded(object sender, RoutedEventArgs e)
        {
            var data = CampaignsFileManager.GetCampaignByNetwork(SocialNetworks);

            SetComboBoxItemSource(SocialNetworks);

            MainGrid.DataContext = objCampaignDetails;

            objCampaignDetails.CampaignCollection = CollectionViewSource.GetDefaultView(data);
        }

        private void list_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //  GridViewColumns.SetGridViewColumnsWidthToStartWidth(list, e);
        }

        private void CmbCampaignType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<CampaignDetails> moduleWiseDetail = null;

            if (!CmbCampaignType.SelectedItem.Equals("All"))
            {
                moduleWiseDetail = CampaignsFileManager.GetCampaignByNetwork(SocinatorInitialize.ActiveSocialNetwork)
                       .Where(x => x.SubModule == CmbCampaignType.SelectedItem.ToString()).ToList();

            }
            else
            {
                moduleWiseDetail = CampaignsFileManager.GetCampaignByNetwork(SocinatorInitialize.ActiveSocialNetwork);
            }
            objCampaignDetails.ObjCampaignDetails = new ObservableCollection<CampaignDetails>(moduleWiseDetail);
            objCampaignDetails.CampaignCollection = CollectionViewSource.GetDefaultView(objCampaignDetails.ObjCampaignDetails);

        }

        private void AllCampaignChecked_Checked(object sender, RoutedEventArgs e)
        {
            AllCampaignCheckUncheck(true);

        }

        private void AllCampaignChecked_Unchecked(object sender, RoutedEventArgs e)
        {
            AllCampaignCheckUncheck(false);

        }

        private void CheckUncheckCampaign(object sender, bool Ischecked)
        {
            var checkedItem = ((FrameworkElement)sender).DataContext as CampaignDetails;
            objCampaignDetails.ObjCampaignDetails.Select(x =>
            {
                if (x.CampaignId == checkedItem?.CampaignId)
                    x.IsCampaignChecked = Ischecked;
                return x;
            }).ToList();

        }
        private void AllCampaignCheckUncheck(bool isChecked)
        {
            objCampaignDetails.ObjCampaignDetails =
                new ObservableCollection<CampaignDetails>(CampaignsFileManager.GetCampaignByNetwork(SocialNetworks));
            objCampaignDetails.ObjCampaignDetails.Select(x => { x.IsCampaignChecked = isChecked; return x; }).ToList();
            SetDefaultView();
        }
        private void DeleteMultipleCampaign_Click(object sender, RoutedEventArgs e)
        {
            List<CampaignDetails> campaign = new List<CampaignDetails>();
            objCampaignDetails.CampaignCollection.SourceCollection.Cast<CampaignDetails>().ToList().ForEach(item =>
            {
                if (item.IsCampaignChecked)
                    campaign.Add(item);
            });

            if (campaign.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Warning", "To delete Campaign please select atleast one Campaign !");
                return;
            }

            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                "Confirmation", "If you delete it will delete from Campaign permanently\nAre you sure You want to delete selected Campaign ?",
                MessageDialogStyle.AffirmativeAndNegative,
                Dialog.SetMetroDialogButton("Delete Anyway", "Cancel"));
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            var allAccounts = AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                campaign.ForEach(camp =>
                {
                    var selectedAccount = camp.SelectedAccountList;
                    CampaignsFileManager.Delete(camp);

                    UpdateAccount(allAccounts, camp, selectedAccount);
                    objCampaignDetails.ObjCampaignDetails.Remove(
                        objCampaignDetails.ObjCampaignDetails.FirstOrDefault(x => x.CampaignId == camp.CampaignId));
                    //  GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, camp.CampaignName, camp.SubModule, "  Campaign deleted permanently from campaigns.","");
                });
            });
            GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork, "[ "+campaign.Count+" ] Campaigns");

            if (objCampaignDetails.ObjCampaignDetails.Count == 0 || !objCampaignDetails.ObjCampaignDetails.All(x => x.IsCampaignChecked))
                objCampaignDetails.IsAllCampaignChecked = false;
            SetDefaultView();
        }

        private static void UpdateAccount(List<DominatorAccountModel> allAccounts, CampaignDetails camp, List<string> selectedAccount)
        {
            try
            {
                var module = (ActivityType)Enum.Parse(typeof(ActivityType), camp.SubModule);
                // remove template from each account
                allAccounts.ForEach(x =>
                {
                    var moduleConfig =
                        x.ActivityManager.LstModuleConfiguration.FirstOrDefault(mc => mc.TemplateId == camp.TemplateId);

                    if (moduleConfig != null)
                    {
                        // Stop active task related to campaign
                        JobProcess.Stop(x.AccountId, camp.TemplateId);

                        // Remove task from list
                        x.ActivityManager.LstModuleConfiguration.RemoveAll(y => y.TemplateId == camp.TemplateId);
                        var socinatorAccountBuilder = new SocinatorAccountBuilder(x.AccountBaseModel.AccountId)
                            .RemoveModuleSettings(module)
                            .SaveToBinFile();
                    }

                });

                //AccountsFileManager.UpdateAccounts(allAccounts);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void SetDefaultView()
        {
            objCampaignDetails.CampaignCollection = CollectionViewSource.GetDefaultView(objCampaignDetails.ObjCampaignDetails);
        }


        private void ChkSingleCampaign_OnChecked(object sender, RoutedEventArgs e)
        {
            if (objCampaignDetails.CampaignCollection.SourceCollection.Cast<CampaignDetails>().ToList()
                .All(x => x.IsCampaignChecked))
                objCampaignDetails.IsAllCampaignChecked = true;
        }

        private void ChkSingleCampaign_OnUnchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckCampaign(sender, false);
            IsUnCheckedFromCampaignDetails = true;
            if (!objCampaignDetails.IsAllCampaignChecked)
                return;
            AllCampaign.Unchecked -= AllCampaignChecked_Unchecked;
            objCampaignDetails.IsAllCampaignChecked = false;
            AllCampaign.Unchecked += AllCampaignChecked_Unchecked;
        }



    }
}

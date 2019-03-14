using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.Converters;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DominatorUIUtility.ViewModel
{
    public class CampaignViewModel : INotifyPropertyChanged
    {
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IDataBaseHandler _dataBaseHandler;
        public CampaignViewModel()
        {
            _accountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            _dataBaseHandler = ServiceLocator.Current.GetInstance<IDataBaseHandler>();
            SettingCommand = new BaseCommand<object>((sender) => true, SettingExecute);
            DeleteCommand = new BaseCommand<object>((sender) => true, DeleteExecute);
            EditCommand = new BaseCommand<object>((sender) => true, EditExecute);
            DuplicateCommand = new BaseCommand<object>((sender) => true, DuplicateExecute);
            StatusChangeCommand = new BaseCommand<object>((sender) => true, StatusChangeExecute);
            ReportCommand = new BaseCommand<object>((sender) => true, ReportExecute);
            ExportReportCommand = new BaseCommand<object>((sender) => true, ExportReportExecute);
            CampaignTypeSelectionChange = new BaseCommand<object>((sender) => true, FilterCampaign);
            SelectionCommand = new BaseCommand<object>((sender) => true, SelectionExecute);
            CopyCampaignIdCommand = new BaseCommand<object>((sender) => true, CopyCampaignIdExecute);
            LoadedCommand = new BaseCommand<object>((sender) => true, FilterCampaign);
            BindingOperations.EnableCollectionSynchronization(LstCampaignDetails, _lock);
            LoadCampaign();
        }

        public void LoadCampaign()
        {
            try
            {
                var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                if (LstCampaignDetails.Count == campaignFileManager.Count())
                    return;
                Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() => LstCampaignDetails.Clear()), DispatcherPriority.Render);
                    campaignFileManager.ForEach(camp =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (LstCampaignDetails.All(x => x.CampaignId != camp.CampaignId))
                                LstCampaignDetails.Add(camp);
                        }), DispatcherPriority.Render);
                        Thread.Sleep(5);
                    });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void SetActivityTypes()
        {
            CampaignModel.ActivityType.Add("All");

            foreach (var name in Enum.GetNames(typeof(ActivityType)))
            {
                if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(SocialNetworks.ToString()))
                    CampaignModel.ActivityType.Add(name);
            }
        }

        #region Command
        public ICommand SettingCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DuplicateCommand { get; set; }
        public ICommand StatusChangeCommand { get; set; }
        public ICommand ReportCommand { get; set; }
        public ICommand ExportReportCommand { get; set; }
        public ICommand CampaignTypeSelectionChange { get; set; }
        public ICommand SelectionCommand { get; set; }
        public ICommand CopyCampaignIdCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        #endregion

        #region Properties

        private static object _lock = new object();

        private ICollectionView _campaignCollection;
        public ICollectionView CampaignCollection
        {
            get
            {
                return _campaignCollection;
            }
            set
            {
                if (_campaignCollection != null && _campaignCollection == value)
                    return;
                _campaignCollection = value;
                OnPropertyChanged(nameof(CampaignCollection));
            }
        }
        private ObservableCollection<CampaignDetails> _campaignDetails = new ObservableCollection<CampaignDetails>();

        public ObservableCollection<CampaignDetails> LstCampaignDetails
        {
            get
            {
                return _campaignDetails;
            }
            set
            {
                if (_campaignDetails != null && _campaignDetails == value)
                    return;
                _campaignDetails = value;
                OnPropertyChanged(nameof(LstCampaignDetails));
            }
        }

        private CampaignDetails _campaignModel = new CampaignDetails();

        public CampaignDetails CampaignModel
        {
            get { return _campaignModel; }
            set
            {
                _campaignModel = value;
                OnPropertyChanged(nameof(CampaignModel));
            }
        }
        private SocialNetworks _socialNetworks;

        public SocialNetworks SocialNetworks
        {
            get
            {
                return _socialNetworks;
            }
            set
            {
                _socialNetworks = value;
                OnPropertyChanged(nameof(SocialNetworks));
            }
        }
        private bool _isAllCampaignChecked;

        public bool IsAllCampaignChecked
        {
            get
            {
                return _isAllCampaignChecked;
            }
            set
            {
                if (_isAllCampaignChecked == value)
                    return;
                _isAllCampaignChecked = value;

                OnPropertyChanged(nameof(IsAllCampaignChecked));
                SelectAllCampaign(_isAllCampaignChecked);
                _isUncheckedFromList = false;
            }
        }
        private bool _isUncheckedFromList;

        public void SelectAllCampaign(bool isAllSelected)
        {
            try
            {
                if (_isUncheckedFromList)
                    return;
                if (CampaignModel.SelectedActivity == "All")
                    LstCampaignDetails.Where(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
                    {
                        x.IsCampaignChecked = isAllSelected; return x;
                    }).ToList();
                else
                    LstCampaignDetails.Where(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork && x.SubModule == CampaignModel.SelectedActivity).Select(x =>
                     {
                         x.IsCampaignChecked = isAllSelected; return x;
                     }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ReportExecute(object sender)
        {
            try
            {
                CampaignDetails campName = sender as CampaignDetails;
                if (campName != null)
                {
                    Reports reportControl = new Reports(campName);
                    Dialog objDialog = new Dialog();

                    var templatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();

                    var activitySettings = templatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

                    var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campName.SubModule);

                    var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory();

                    ObservableCollection<QueryInfo> lstSavedQuery = networkCoreFactory.ReportFactory.GetSavedQuery(activityType, activitySettings);

                    lstSavedQuery?.ToList().ForEach(x =>
                    {
                        reportControl.ReportModel.LstCurrentQueries.Add(new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));
                        #region Update QueryList for combobox

                        if (reportControl.ReportModel.QueryList.All(query => query.Content != x.QueryType))
                            reportControl.ReportModel.QueryList.Add(new ContentSelectGroup() { Content = x.QueryType });

                        #endregion

                    });
                    try
                    {
                        #region Update AccountList & StatusList for combobox

                        campName.SelectedAccountList.ToList().ForEach(acc =>
                        {
                            DominatorAccountModel objDominatorAccountModel = _accountsFileManager.GetAccount(acc, campName.SocialNetworks);

                            reportControl.ReportModel.AccountList.Add(new ContentSelectGroup()
                            {
                                IsContentSelected = false,
                                Content = objDominatorAccountModel.AccountBaseModel.UserName
                            });

                            if (reportControl.ReportModel.StatusList.Count > 1 &&
                                reportControl.ReportModel.StatusList.All(status => status.Content != objDominatorAccountModel.AccountBaseModel.Status.ToString()))
                                reportControl.ReportModel.StatusList.Add(new ContentSelectGroup()
                                {
                                    IsContentSelected = false,
                                    Content = objDominatorAccountModel.AccountBaseModel.Status.ToString()
                                });

                        });
                        #endregion

                        var reportDetails = networkCoreFactory.ReportFactory.GetReportDetail(reportControl.ReportModel,
                            reportControl.ReportModel.LstCurrentQueries, campName);
                        if (reportDetails.Count == 0)
                        {
                            Dialog.ShowDialog("Report", "Reports for " + campName.CampaignName + " Campaign not available");
                            return;
                        }

                        Window win = objDialog.GetMetroWindow(reportControl, "Reports");
                        win.Owner = Application.Current.MainWindow;
                        win.WindowStartupLocation = WindowStartupLocation.Manual;
                        win.Top = 0;
                        win.Left = 0;
                        reportControl.ReportModel.LstReports = new ObservableCollection<object>();
                        reportControl.ReportModel.ReportCollection =
                            CollectionViewSource.GetDefaultView(reportControl.ReportModel.LstReports);

                        Task.Factory.StartNew(() =>
                        {
                            reportDetails.ForEach(item =>
                            {
                                Application.Current.Dispatcher.Invoke(() => reportControl.ReportModel.LstReports.Add(item));
                                Thread.Sleep(10);
                            });
                        });

                        win.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void ExportReportExecute(object sender)
        {
            try
            {
                CampaignDetails campaign = sender as CampaignDetails;

                if (campaign != null)
                {
                    var templatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();

                    var activitySettings = templatesFileManager.GetTemplateById(campaign.TemplateId).ActivitySettings;

                    var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campaign.SubModule);

                    var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(campaign.SocialNetworks).GetNetworkCoreFactory();

                    ObservableCollection<QueryInfo> lstSavedQuery = networkCoreFactory.ReportFactory.GetSavedQuery(activityType, activitySettings);

                    ReportModel ReportModel = new ReportModel();
                    lstSavedQuery?.ToList().ForEach(x =>
                    {
                        ReportModel.LstCurrentQueries.Add(new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));
                    });

                    var reportDetails = networkCoreFactory.ReportFactory.GetReportDetail(ReportModel,
                        ReportModel.LstCurrentQueries, campaign);

                    if (reportDetails.Count == 0)
                    {
                        Dialog.ShowDialog("Report", "Reports for " + campaign.CampaignName + " Campaign not available");
                        return;
                    }

                    var exportPath = FileUtilities.GetExportPath();

                    if (string.IsNullOrEmpty(exportPath))
                        return;

                    var filename = Regex.Replace(
                                      input: $"{ campaign.CampaignName }-Reports[{DateTimeUtilities.GetEpochTime()}]",
                                      pattern: "[\\/:*?<>|\"]",
                                      replacement: "-");

                    filename = $"{exportPath}\\{filename}.csv";
                    SocinatorInitialize.GetSocialLibrary(campaign.SocialNetworks).GetNetworkCoreFactory().ReportFactory.ExportReports(activityType, filename, ReportType.Campaign);
                    
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void StatusChangeExecute(object sender)
        {
            try
            {
                var selectedCampaign = ((FrameworkElement)sender).DataContext as CampaignDetails;

                if (selectedCampaign?.SelectedAccountList.Count == 0)
                {
                    if (selectedCampaign.Status == "Paused")
                        return;
                    GlobusLogHelper.log.Info(Log.CustomMessage, selectedCampaign.SocialNetworks, selectedCampaign.CampaignName, "Status Change Failed", $"Account is not present in {selectedCampaign.CampaignName}");
                    selectedCampaign.Status = "Paused";
                    return;
                }

                var isChecked = ((ToggleSwitch)sender).IsChecked;
                var isToggleSwitchSelected = isChecked != null && (bool)isChecked;

                ThreadFactory.Instance.Start(() =>
                {
                    ActivePauseCampaign(selectedCampaign, isToggleSwitchSelected);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DuplicateExecute(object sender)
        {
            try
            {
                CampaignDetails camp = sender as CampaignDetails;
                var campName = camp.DeepCloneObject();
                campName.CampaignName = camp?.CampaignName.
                                              Split('[')[0] + $"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";

                SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ViewCampaigns
                    .ViewCampaigns(campName.CampaignId, ConstantVariable.CreateCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditExecute(object sender)
        {
            try
            {
                CampaignDetails campName = sender as CampaignDetails;

                SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory().ViewCampaigns
                    .ViewCampaigns(campName.CampaignId, ConstantVariable.UpdateCampaign);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteExecute(object sender)
        {
            if (sender is CampaignDetails)
            {
                try
                {
                    CampaignDetails campaign = sender as CampaignDetails;
                    var dialogResult = Dialog.ShowCustomDialog("Confirmation",
                        "If you delete it will delete [ " + campaign.CampaignName + " ] Campaign permanently from campaign\nAre you sure ?", "Delete Anyways", "Don't delete");
                    if (dialogResult != MessageDialogResult.Affirmative)
                        return;
                    var selectedAccount = campaign.SelectedAccountList;

                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.Delete(campaign);
                    _dataBaseHandler.DeleteDatabase(new List<string> { campaign.CampaignId }, DatabaseType.CampaignType);
                    LstCampaignDetails.Remove(LstCampaignDetails.FirstOrDefault(x => x.CampaignId == campaign.CampaignId));
                    Uncheck();
                    var allAccounts = _accountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
                    UpdateAccount(allAccounts, campaign, selectedAccount);
                    GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork,
                        campaign.CampaignName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }
            else
            {
                try
                {
                    List<CampaignDetails> campaign = new List<CampaignDetails>();
                    LstCampaignDetails.ToList().ForEach(item =>
                    {
                        if (item.IsCampaignChecked)
                            campaign.Add(item);
                    });

                    if (campaign.Count == 0)
                    {
                        Dialog.ShowDialog("Warning", "To delete Campaign please select atleast one Campaign !");
                        return;
                    }

                    var dialogResult = Dialog.ShowCustomDialog("Confirmation", "If you delete it will delete from Campaign permanently\nAre you sure You want to delete selected Campaign ?",
                       "Delete Anyway", "Cancel");
                    if (dialogResult != MessageDialogResult.Affirmative)
                        return;
                    var allAccounts = _accountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        campaign.ForEach(camp =>
                        {
                            var selectedAccount = camp.SelectedAccountList;
                            campaignFileManager.Delete(camp);

                            UpdateAccount(allAccounts, camp, selectedAccount);
                            LstCampaignDetails.Remove(
                                LstCampaignDetails.FirstOrDefault(x => x.CampaignId == camp.CampaignId));
                        });
                        _dataBaseHandler.DeleteDatabase(campaign.Select(acct => acct.CampaignId), DatabaseType.CampaignType);
                        Uncheck();
                    });
                    GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork, "[ " + campaign.Count + " ] Campaigns");

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private void Uncheck()
        {
            if (LstCampaignDetails.Count(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork) == 0 &&
                IsAllCampaignChecked)
                IsAllCampaignChecked = false;
        }

        private void CopyCampaignIdExecute(object sender)
        {
            try
            {
                CampaignDetails campName = sender as CampaignDetails;

                if (!string.IsNullOrEmpty(campName.CampaignId))
                {
                    Clipboard.SetText(campName.CampaignId);
                    ToasterNotification.ShowSuccess("CampaignId copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SettingExecute(object sender)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                {
                    var contextMenu = ((Button)sender).ContextMenu;
                    if (contextMenu != null)
                    {
                        contextMenu.DataContext = ((Button)sender).DataContext;
                        contextMenu.IsOpen = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SelectionExecute(object sender)
        {
            try
            {
                // To check whether all destinations are selected, then make the tick mark on column header
                if (LstCampaignDetails.Where(x => x.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork).All(y => y.IsCampaignChecked))
                    IsAllCampaignChecked = true;
                else
                {
                    if (IsAllCampaignChecked)
                        _isUncheckedFromList = true;
                    // If not so, dont tick the column header 
                    IsAllCampaignChecked = false;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private void FilterCampaign(object sender)
        {
            try
            {
                if (CampaignModel.SelectedActivity == "All" || string.IsNullOrEmpty(CampaignModel.SelectedActivity))
                    CampaignCollection.Filter = (x) =>
                        ((CampaignDetails)x).SocialNetworks == SocinatorInitialize.ActiveSocialNetwork;
                else
                    CampaignCollection.Filter = (x) =>
                        ((CampaignDetails)x).SocialNetworks == SocinatorInitialize.ActiveSocialNetwork &&
                        ((CampaignDetails)x).SubModule == CampaignModel.SelectedActivity;
            }
            catch (Exception ex)
            {
                CampaignCollection.Filter =
                    (x) => ((CampaignDetails)x)?.SocialNetworks == SocinatorInitialize.ActiveSocialNetwork;
                ex.DebugLog();
            }
        }

        private void ActivePauseCampaign(CampaignDetails selectedCampaign, bool isToggleSwitchSelected)
        {
            try
            {
                ImmutableQueue<Action> updatingAccountsBinFiles = ImmutableQueue<Action>.Empty;

                var addedAccountDetails = ServiceLocator.Current.GetInstance<IAccountCollectionViewModel>().BySocialNetwork(selectedCampaign.SocialNetworks);
                var lstAccountDetails = _accountsFileManager.GetAllAccounts(selectedCampaign.SelectedAccountList, selectedCampaign.SocialNetworks);
                var module = (ActivityType)Enum.Parse(typeof(ActivityType), selectedCampaign.SubModule);

                lstAccountDetails.ForEach(account =>
                {
                    try
                    {
                        if (!addedAccountDetails.Any(x => x.AccountId == account.AccountId))
                        {
                            return;
                        }

                        updatingAccountsBinFiles = updatingAccountsBinFiles.Enqueue(() =>
                         {
                             UpdateAccountCampaignsStatus(selectedCampaign, isToggleSwitchSelected, account, module);
                         });

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                try
                {
                    new Thread(() =>
                    {
                        while (!updatingAccountsBinFiles.IsEmpty)
                        {
                            Action act;
                            updatingAccountsBinFiles = updatingAccountsBinFiles.Dequeue(out act);

                            act();
                        }
                    })
                    { IsBackground = true }.Start();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                // Run/Stop job process in campaigns
                try
                {
                    if (isToggleSwitchSelected)
                    {
                        LstCampaignDetails.FirstOrDefault(x => x.CampaignId == selectedCampaign.CampaignId).Status = "Active";
                        GlobusLogHelper.log.Info(Log.ActivatedCampaign, SocinatorInitialize.ActiveSocialNetwork, selectedCampaign.CampaignName);
                    }
                    else
                    {
                        LstCampaignDetails.FirstOrDefault(x => x.CampaignId == selectedCampaign.CampaignId).Status = "Paused";
                        GlobusLogHelper.log.Info(Log.CampaignPaused, SocinatorInitialize.ActiveSocialNetwork, selectedCampaign.CampaignName);
                    }

                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.UpdateCampaigns(LstCampaignDetails.ToList());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private static void UpdateAccountCampaignsStatus(CampaignDetails selectedCampaign, bool isToggleSwitchSelected, DominatorAccountModel account, ActivityType module)
        {
            try
            {
                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
                var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                var moduleConfiguration = jobActivityConfigurationManager[account.AccountId, module];

                if (moduleConfiguration?.TemplateId == selectedCampaign.TemplateId)
                    moduleConfiguration.IsEnabled = isToggleSwitchSelected;

                jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, moduleConfiguration.ActivityType, moduleConfiguration);
                accountsCacheService.UpsertAccounts(account);
                if (isToggleSwitchSelected)
                    dominatorScheduler.ScheduleNextActivity(account, module);
                else
                    dominatorScheduler.StopActivity(account, selectedCampaign.SubModule, selectedCampaign.TemplateId, false);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private static void UpdateAccount(List<DominatorAccountModel> allAccounts, CampaignDetails camp, List<string> selectedAccount)
        {
            try
            {
                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
                var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                // remove template from each account
                allAccounts.ForEach(x =>
                {
                    var moduleConfig = jobActivityConfigurationManager[x.AccountId].FirstOrDefault(mc => mc.TemplateId == camp.TemplateId);
                    if (moduleConfig != null)
                    {
                        // Stop active task related to campaign
                        dominatorScheduler.StopActivity(x, camp.SubModule, camp.TemplateId, false);

                        // Remove task from list
                        foreach (var moduleConfiguration in jobActivityConfigurationManager[x.AccountId].Where(mc => mc.TemplateId == camp.TemplateId).ToList())
                        {
                            jobActivityConfigurationManager.Delete(x.AccountId, moduleConfiguration.ActivityType);
                        }
                    }
                });

                accountsCacheService.UpsertAccounts(allAccounts.ToArray());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
    public class BindingData : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new BindingData();
        }

        #endregion

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingData), new UIPropertyMetadata(null));
    }
}
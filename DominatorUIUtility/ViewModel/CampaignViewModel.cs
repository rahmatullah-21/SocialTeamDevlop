using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.Converters;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel
{
    public class CampaignViewModel : INotifyPropertyChanged
    {
        public CampaignViewModel()
        {
            SettingCommand = new BaseCommand<object>((sender) => true, SettingExecute);
            DeleteCommand = new BaseCommand<object>((sender) => true, DeleteExecute);
            EditCommand = new BaseCommand<object>((sender) => true, EditExecute);
            DuplicateCommand = new BaseCommand<object>((sender) => true, DuplicateExecute);
            StatusChangeCommand = new BaseCommand<object>((sender) => true, StatusChangeExecute);
            ReportCommand = new BaseCommand<object>((sender) => true, ReportExecute);
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
                Task.Factory.StartNew(() =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstCampaignDetails.Clear());
                    campaignFileManager.ForEach(camp =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (LstCampaignDetails.All(x => x.CampaignId != camp.CampaignId))
                                LstCampaignDetails.Add(camp);
                        });
                        Thread.Sleep(50);
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
                Reports reportControl = new Reports(campName);
                Dialog objDialog = new Dialog();

                var TemplatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
                var ActivitySettings = TemplatesFileManager.GetTemplateById(campName.TemplateId).ActivitySettings;

                var activityType = (ActivityType)Enum.Parse(typeof(ActivityType), campName.SubModule);

                var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(campName.SocialNetworks).GetNetworkCoreFactory();

                ObservableCollection<QueryInfo> lstSavedQuery = networkCoreFactory.ReportFactory.GetSavedQuery(activityType, ActivitySettings);

                lstSavedQuery?.ToList().ForEach(x =>
                {
                    reportControl.ReportModel.LstCurrentQueries.Add(new KeyValuePair<string, string>(x.QueryValue, x.QueryType.ToString()));
                    #region Update QueryList for combobox

                    if (reportControl.ReportModel.QueryList.Any(query => query.Content == x.QueryType) == false)
                        reportControl.ReportModel.QueryList.Add(new ContentSelectGroup() { IsContentSelected = false, Content = x.QueryType });

                    #endregion

                });
                try
                {
                    #region Update AccountList & StatusList for combobox

                    campName.SelectedAccountList.ToList().ForEach(acc =>
                    {
                        DominatorAccountModel objDominatorAccountModel = AccountsFileManager.GetAccount(acc, campName.SocialNetworks);

                        reportControl.ReportModel.AccountList.Add(new ContentSelectGroup()
                        {
                            IsContentSelected = false,
                            Content = objDominatorAccountModel.AccountBaseModel.UserName
                        });

                        if (reportControl.ReportModel.StatusList.Count > 1 &&
                            reportControl.ReportModel.StatusList.Any(status => status.Content == objDominatorAccountModel.AccountBaseModel.Status.ToString()) ==
                            false)
                            reportControl.ReportModel.StatusList.Add(new ContentSelectGroup()
                            {
                                IsContentSelected = false,
                                Content = objDominatorAccountModel.AccountBaseModel.Status.ToString()
                            });

                    });
                    #endregion

                    if (networkCoreFactory.ReportFactory.GetReportDetail(reportControl.ReportModel, reportControl.ReportModel.LstCurrentQueries, campName) == 0)
                    {
                        Dialog.ShowDialog("Report", "Reports for " + campName.CampaignName + " Campaign not available");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                Window win = objDialog.GetMetroWindow(reportControl, "Reports");
                win.Owner = Application.Current.MainWindow;
                win.ShowDialog();
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
                CampaignDetails campName = sender as CampaignDetails;
                var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                campaignFileManager.GetCampaignById(campName.CampaignId).
                    CampaignName = campName.CampaignName.
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
                    var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(
                        Application.Current.MainWindow, "Confirmation",
                        "If you delete it will delete [ " + campaign.CampaignName +
                        " ] Campaign permanently from campaign\nAre you sure ?",
                        MessageDialogStyle.AffirmativeAndNegative,
                        Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));
                    if (dialogResult != MessageDialogResult.Affirmative)
                        return;
                    var selectedAccount = campaign.SelectedAccountList;

                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.Delete(campaign);

                    LstCampaignDetails.Remove(LstCampaignDetails.FirstOrDefault(x => x.CampaignId == campaign.CampaignId));

                    var allAccounts = AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);
                    UpdateAccount(allAccounts, campaign, selectedAccount);
                    GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork,
                        campaign.CampaignName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                if (LstCampaignDetails.Count == 0 && IsAllCampaignChecked)
                    IsAllCampaignChecked = false;
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
                            //  GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, camp.CampaignName, camp.SubModule, "  Campaign deleted permanently from campaigns.","");
                        });
                        if (LstCampaignDetails.Count == 0 && IsAllCampaignChecked)
                            IsAllCampaignChecked = false;

                    });
                    GlobusLogHelper.log.Info(Log.CampaignDeleted, SocinatorInitialize.ActiveSocialNetwork, "[ " + campaign.Count + " ] Campaigns");


                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

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
                ((Button)sender).ContextMenu.DataContext = ((Button)sender).DataContext;
                ((Button)sender).ContextMenu.IsOpen = true;
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
                if (LstCampaignDetails.All(x => x.IsCampaignChecked))
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

                var lstAccountDetails = AccountsFileManager.GetAllAccounts(selectedCampaign.SelectedAccountList, selectedCampaign.SocialNetworks);
                if (selectedCampaign == null)
                    return;

                var module = (ActivityType)Enum.Parse(typeof(ActivityType), selectedCampaign.SubModule);

                lstAccountDetails.ForEach(account =>
                {
                    try
                    {
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
                var moduleConfiguration = jobActivityConfigurationManager[account.AccountId, module];

                if (moduleConfiguration?.TemplateId == selectedCampaign.TemplateId)
                    moduleConfiguration.IsEnabled = isToggleSwitchSelected;

                jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, moduleConfiguration.ActivityType, moduleConfiguration);
                accountsCacheService.UpsertAccounts(account);
                if (isToggleSwitchSelected)
                {
                    DominatorScheduler.ScheduleNextActivity(account, module);
                }
                else
                {
                    DominatorScheduler.StopActivity(account, selectedCampaign.SubModule, selectedCampaign.TemplateId, true);
                }
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
                var module = (ActivityType)Enum.Parse(typeof(ActivityType), camp.SubModule);
                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
                // remove template from each account
                allAccounts.ForEach(x =>
                {
                    var moduleConfig = jobActivityConfigurationManager[x.AccountId].FirstOrDefault(mc => mc.TemplateId == camp.TemplateId);
                    if (moduleConfig != null)
                    {
                        // Stop active task related to campaign
                        JobProcess.Stop(x.AccountId, camp.TemplateId);

                        // Remove task from list
                        foreach (var moduleConfiguration in jobActivityConfigurationManager[x.AccountId].Where(mc => mc.TemplateId == camp.TemplateId))
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
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingData), new UIPropertyMetadata(null));
    }
}
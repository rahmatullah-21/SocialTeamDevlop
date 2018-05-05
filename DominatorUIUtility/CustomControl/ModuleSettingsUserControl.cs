using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.LogHelper;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Base class which handles events from IHeaderControl, IHelpContol, IFooterControl
    /// Implements handlers for: _OnSelectAccountChanged, _OnCreateCampaignChanged, _OnCustomFilterChanged,
    ///  _OnInfoChanged, _OnAddQuery, SetDataContex
    /// Uses as base for: Follower, Unfollower, Like, Comment, DownloadPhotos, etc.
    /// </summary>
    public abstract class ModuleSettingsUserControl<TViewModel, TModel> : UserControl, INotifyPropertyChanged, IHeaderControl, IHelpControl, IFooterControl, IAccountGrowthModeHeader
        where TModel : class, new()
        where TViewModel : class, new()
    {
        HeaderControl _headerControl;
        FooterControl _footerControl;
        SearchQueryControl _queryControl;
        Grid _mainGrid;
        AccountGrowthModeHeader _accountGrowthModeHeader;
        ActivityType _activityType;
        string _moduleName;
        SocialNetworks _socialNetwork => SocinatorInitialize.ActiveSocialNetwork;

        bool _initialized = false;
        private bool _isCancelledUpdate = false;
        protected ModuleSettingsUserControl()
        {
        }

        public void InitializeBaseClass(
            Grid MainGrid,
            ActivityType activityType,
            string moduleName,
            HeaderControl header = null,
            FooterControl footer = null,
            SearchQueryControl queryControl = null,
            AccountGrowthModeHeader accountGrowthModeHeader = null)
        {
            //if (queryControl == null) throw new ArgumentNullException(nameof(queryControl));

            if (_initialized) return;

            _headerControl = header;
            _footerControl = footer;
            _queryControl = queryControl;
            _mainGrid = MainGrid;
            _activityType = activityType;
            _moduleName = moduleName;
            _accountGrowthModeHeader = accountGrowthModeHeader;
            _initialized = true;
        }


        #region IHeaderControl

        private string _campaignName;

        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private bool _isEditCampaignName = true;

        public bool IsEditCampaignName
        {
            get
            {
                return _isEditCampaignName;
            }
            set
            {
                _isEditCampaignName = value;
                OnPropertyChanged(nameof(IsEditCampaignName));
            }
        }


        private Visibility _cancelEditVisibility = Visibility.Collapsed;

        public Visibility CancelEditVisibility
        {
            get
            {
                return _cancelEditVisibility;
            }
            set
            {
                _cancelEditVisibility = value;
                OnPropertyChanged(nameof(CancelEditVisibility));
            }
        }

        private string _templateId;
        public string TemplateId
        {
            get
            {
                return _templateId;
            }

            set
            {
                _templateId = value;
                OnPropertyChanged(nameof(_templateId));
            }
        }
        #endregion

        #region IHelpControl

        // 
        public string VideoTutorialLink { get; set; } = "!Pass ConstantHelpDetails.VideoTutorialsLink in Derived Class";

        public string KnowledgeBaseLink { get; set; } = "!Pass ConstantHelpDetails.KnowledgeBaseLink";

        public string ContactSupportLink { get; set; } = "!Pass ConstantHelpDetails.ContactLink";

        #endregion

        #region IFooterControl

        private string _selectedAccountCount = ConstantVariable.NoAccountSelected;

        public string SelectedAccountCount
        {
            get
            {
                return _selectedAccountCount;
            }
            set
            {
                _selectedAccountCount = value;
                OnPropertyChanged(nameof(SelectedAccountCount));
            }
        }

        private string _campaignButtonContent = ConstantVariable.CreateCampaign;
        public string CampaignButtonContent
        {
            get
            {
                return _campaignButtonContent;
            }
            set
            {
                _campaignButtonContent = value;
                OnPropertyChanged(nameof(CampaignButtonContent));
            }
        }

        #endregion


        private TViewModel _ObjViewModel = new TViewModel();
        private ObservableCollectionBase<string> _accountItemSource;
        private string _selectedItem;


        public TViewModel ObjViewModel
        {
            get { return _ObjViewModel; }
            set
            {
                _ObjViewModel = value;
                OnPropertyChanged(nameof(ObjViewModel));
            }
        }

        // NOTE: ViewModel must contain Model field
        public dynamic Model => (ObjViewModel as dynamic).Model as TModel;


        #region PropertyChanged section

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion


        /// <summary>
        /// Add Query handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchQueryControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue))
                {
                    _queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryValue = query.Trim();

                        currentQuery.QueryTypeDisplayName = currentQuery.QueryTypeAsDisplayName();

                        currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                        if (IsQueryExistWithoutDialog(currentQuery, Model.SavedQueries)) return;

                        Model.SavedQueries.Add(currentQuery);

                    });


                }
                else
                {
                    _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryTypeAsDisplayName();

                    var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    currentQuery.QueryValue = currentQuery.QueryValue.Trim();

                    if (IsQueryExist(currentQuery, Model.SavedQueries)) return;

                    Model.SavedQueries.Add(currentQuery);

                    _queryControl.CurrentQuery = new QueryInfo();

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        /// <summary>
        /// Add Query handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="queryParameterType"></param>
        protected void SearchQueryControl_OnAddQuery(object sender, RoutedEventArgs e, Type queryParameterType)
        {
            try
            {
                List<int> queryValuIndex=new List<int>();
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue))
                {
                    _queryControl.QueryCollection.ForEach(query =>
                    {
                        var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                        if (currentQuery == null) return;

                        currentQuery.QueryValue = query;
                        currentQuery.QueryTypeDisplayName = currentQuery.QueryType;
                      //  currentQuery.QueryTypeDisplayName = currentQuery.QueryTypeAsDisplayName(queryParameterType);

                        currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                        if (IsQueryExistWithoutDialog(currentQuery, Model.SavedQueries))
                        {
                            queryValuIndex.Add(_queryControl.QueryCollection.IndexOf(query));
                            return;
                        }

                        Model.SavedQueries.Add(currentQuery);

                    });
                    if (queryValuIndex.Count > 0)
                    {
                        if (queryValuIndex.Count <= 10)
                        {
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork, "{ "+ string.Join(" },{ ", queryValuIndex.ToArray())+" }", _activityType);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount, SocinatorInitialize.ActiveSocialNetwork, queryValuIndex.Count, _activityType);

                        } 
                    }
                }
                else
                {
                    _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryType;

                   // _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryTypeAsDisplayName(queryParameterType);

                    var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    if (IsQueryExist(currentQuery, Model.SavedQueries)) return;

                    Model.SavedQueries.Add(currentQuery);

                    _queryControl.CurrentQuery = new QueryInfo();
                    var listQueryInfo = _queryControl.ListQueryInfo;
                    var listQueryType = _queryControl.ListQueryType;
                    var queryCollection = _queryControl.QueryCollection;
                    var selectedIndex = _queryControl.SelectedIndex;
                    var searchQueries = _queryControl.SearchQueries;
                    var ModelSavedQueries = Model.SavedQueries;
                    var datagridItemSource = _queryControl.SearchQueries.ItemsSource;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool IsQueryExist(QueryInfo currentQuery, ObservableCollection<QueryInfo> queryToSave)
        {
            try
            {
                if (queryToSave.Any(x =>
                        x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                        "Query already Exist !!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }
        private bool IsQueryExistWithoutDialog(QueryInfo currentQuery, ObservableCollection<QueryInfo> queryToSave)
        {
            try
            {
                if (queryToSave.Any(x =>
                    x.QueryType == currentQuery.QueryType && x.QueryValue == currentQuery.QueryValue))
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        /// <summary>
        /// On Custom filter  changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchQueryControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {
            UserFilterAction.UserFilterControl(_queryControl);
            //UserFiltersControl objUserFiltersControl = new UserFiltersControl();
            //Dialog objDialog = new Dialog();
            //var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");
            //objUserFiltersControl.SaveButton.Click += (senders, Events) =>
            //{
            //    _queryControl.CurrentQuery.CustomFilters = JsonConvert.SerializeObject(objUserFiltersControl.UserFilter);
            //    FilterWindow.Close();
            //};
            //FilterWindow.ShowDialog();
        }

        public virtual void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType) { }

        public virtual void AddNewCampaign(List<string> lstSelectedAccounts, ActivityType moduleType) { }

        protected virtual void SetModuleValues(bool isToggleButtonActive, TemplateModel templateModel) { }

        protected virtual bool ValidateCampaign()
        {
            if (_footerControl.list_SelectedAccounts.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please select at least one account.", MessageDialogStyle.Affirmative);
                return false;
            }
            // Check timings
            return ValidateRunningTime();
        }

        private bool ValidateRunningTime()
        {
            if (((IEnumerable<RunningTimes>)Model.JobConfiguration.RunningTime).All(rt => rt.Timings.Count == 0))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one time range when to run and stop the activity.",
                    MessageDialogStyle.Affirmative);
                return false;
            }
            return true;
        }


        protected virtual bool ValidateExtraProperty() => true;

        /// <summary>
        /// Event handler called when user Creates or Updates campaign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FooterControl_OnCreateCampaignChanged(object sender, RoutedEventArgs e)
        {
            if (!ValidateCampaign()) return;

            if (!ValidateExtraProperty()) return;

            // TODO: implement saving and add campaign
            // CampaignGlobalRoutines.Instance.Create((TModel)Model, _activityType, CampaignName, _footerControl.list_SelectedAccounts);

            TemplateId = TemplateModel.SaveTemplate((TModel)Model,
                _activityType.ToString(), _socialNetwork,
                CampaignName);


            SaveDetails(_footerControl.list_SelectedAccounts, _activityType);

            AddNewCampaign(_footerControl.list_SelectedAccounts, _activityType);

            SetDataContext();

            TabSwitcher.GoToCampaign();
        }


        #region Save Campaign Implementations

        protected void FooterControl_OnCreateCampaignChanged(object sender, RoutedEventArgs e, string errorMessage, List<RunningTimes> runningTime)
        {
            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;


            TemplateId = TemplateModel.SaveTemplate((TModel)Model,
                _activityType.ToString(), _socialNetwork,
                CampaignName);

            _isCancelledUpdate = false;

            SaveCampaign(_footerControl.list_SelectedAccounts, _activityType, errorMessage, runningTime);

            if (!_isCancelledUpdate)
                AddNewCampaign(_footerControl.list_SelectedAccounts);

            SetDataContext();

            TabSwitcher.GoToCampaign();
        }


     
        #region Campaign Implementations

        protected void CreateCampaign(string errorMessage, List<RunningTimes> runningTime)
        {
            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;

            var schedulePending = ImmutableQueue<Action>.Empty;

            bool allScheduleQueued;

            if (IsNeedToSaveTemplate(errorMessage))
            {
                TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), _socialNetwork, CampaignName);

                SaveTemplateToAccounts(TemplateId, runningTime);

                SaveTemplateToCampaigns();

                var accountDetails = AccountsFileManager.GetAll(_footerControl.list_SelectedAccounts);

                allScheduleQueued = false;

                try
                {
                    new Thread(() =>
                        {
                            while (!allScheduleQueued)
                            {
                                Thread.Sleep(50);
                                while (!schedulePending.IsEmpty)
                                {
                                    Action startSchedule;
                                    schedulePending = schedulePending.Dequeue(out startSchedule);
                                    startSchedule();
                                }
                            }
                        })
                    { IsBackground = true }.Start();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                Thread.Sleep(50);

                foreach (var account in accountDetails)
                {
                    Action scheduleAccount = () =>
                    {
                        DominatorScheduler.ScheduleTodayJobs(account, _socialNetwork, _activityType);
                        DominatorScheduler.ScheduleForEachModule(_activityType, account, _socialNetwork);
                    };
                    schedulePending = schedulePending.Enqueue(scheduleAccount);
                }

                allScheduleQueued = true;

                SetDataContext();

                TabSwitcher.GoToCampaign();
            }
        }

        public bool IsNeedToSaveTemplate(string errorMessage)
        {

            #region Get the accounts which holds template Id

            var accountDetails = AccountsFileManager.GetAll(_footerControl.list_SelectedAccounts);

            var accountHavingTemplates = accountDetails.Where(x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == _activityType)?.TemplateId != null).ToList();

            if (accountHavingTemplates.Count == 0)
                return true;

            #endregion

            var objErrorModelControl = new ErrorModelControl { WarningText = errorMessage };

            #region Adding the accounts to Warning window

            accountHavingTemplates.ForEach(account =>
            {
                var moduleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module => module.ActivityType == _activityType);

                if (moduleSettings != null && moduleSettings.IsEnabled)
                {
                    objErrorModelControl.Accounts.Add(new ErrorModelControl { UserName = account.AccountBaseModel.UserName });
                }
            });

            var warningWindow = new Dialog().GetMetroWindow(objErrorModelControl, "Warning");

            #endregion

            #region Warning windows save button event

            objErrorModelControl.BtnSave.Click += (senders, events) =>
            {
                #region Remove not selected accounts from errorModelControl

                var nonSelectedAccounts = objErrorModelControl.Accounts.Where(x => !x.IsChecked).Select(x => x.UserName)
                    .ToList();

                nonSelectedAccounts.ForEach(removingAccount =>
                {
                    _footerControl.list_SelectedAccounts.Remove(removingAccount);
                });

                #endregion

                var selectedAccount = objErrorModelControl.Accounts.Where(x => x.IsChecked).Select(x => x.UserName).ToList();

                if (selectedAccount.Count == 0)
                {
                    warningWindow.Close();
                    return;
                }

                accountDetails.ForEach(account =>
                {
                    if (!selectedAccount.Contains(account.AccountBaseModel.UserName))
                        return;

                    var moduleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module => module.ActivityType == _activityType);

                    if (moduleSettings == null)
                        return;

                    CampaignsFileManager.DeleteSelectedAccount(moduleSettings.TemplateId, account.AccountBaseModel.UserName);

                    DominatorScheduler.StopActivity(account.AccountBaseModel.AccountId, _activityType.ToString(), moduleSettings.TemplateId);

                    account.ActivityManager.LstModuleConfiguration.Remove(moduleSettings);

                });

                AccountsFileManager.UpdateAccounts(accountDetails);

                warningWindow.Close();
            };

            #endregion

            #region Warning windows cancel button event

            objErrorModelControl.BtnCancel.Click += (senders, events) =>
            {

                #region Remove not selected accounts from errorModelControl

                var nonSelectedAccounts = objErrorModelControl.Accounts.Where(x => !x.IsChecked).Select(x => x.UserName)
                    .ToList();

                nonSelectedAccounts.ForEach(removingAccount =>
                {
                    _footerControl.list_SelectedAccounts.Remove(removingAccount);
                });

                #endregion

                warningWindow.Close();
            };

            #endregion

            warningWindow.ShowDialog();

            if (_footerControl.list_SelectedAccounts.Count == 0)
            {
                return false;
            }
            return true;
        }

        public void SaveTemplateToAccounts(string templateId, List<RunningTimes> runningTime)
        {
            var accountDetails = AccountsFileManager.GetAll(_footerControl.list_SelectedAccounts);

            accountDetails.ForEach(account =>
            {
                var moduleConfiguration = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == _activityType);

                if (moduleConfiguration == null)
                {
                    moduleConfiguration = new ModuleConfiguration { ActivityType = _activityType };

                    account.ActivityManager.LstModuleConfiguration?.Add(moduleConfiguration);
                }

                moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();

                moduleConfiguration.IsEnabled = true;

                moduleConfiguration.Status = "Active";

                moduleConfiguration.TemplateId = templateId;

                runningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings)
                    {
                        timingRange.Module = _activityType.ToString();
                    }
                });

                account.ActivityManager.RunningTime = runningTime;

                moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);

                account.IsCretedFromNormalMode = true;

                AccountsFileManager.Edit(account);
            });


            var finalAccountDetails = AccountsFileManager.GetAll();

        }

        public void SaveTemplateToCampaigns()
        {
            var campaignDetails = new CampaignDetails
            {
                CampaignName = CampaignName,
                MainModule = _moduleName,
                SubModule = _activityType.ToString(),
                SocialNetworks = _socialNetwork,
                SelectedAccountList = _footerControl.list_SelectedAccounts,
                TemplateId = TemplateId,
                CreationDate = DateTimeUtilities.GetEpochTime(),
                Status = "Active",
                LastEditedDate = DateTimeUtilities.GetEpochTime(),
            };

            DataBaseHandler.CreateDataBase(campaignDetails.CampaignId, _socialNetwork, DatabaseType.CampaignType);

            CampaignsFileManager.Add(campaignDetails);
        }

        #endregion




        public void SaveCampaign(List<string> selectedAccounts, ActivityType moduleType, string errorMessage, List<RunningTimes> runningTime)
        {
            var accountDetails = AccountsFileManager.GetAll(selectedAccounts);

            var accountHavingTemplates = accountDetails.Where(x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == moduleType)
                                     ?.TemplateId != null).ToList();

            //it will check if any account having setting or not 
            if (accountHavingTemplates.Count > 0)
            {

                var objErrorModelControl = new ErrorModelControl
                {
                    WarningText = errorMessage
                };

                try
                {
                    foreach (var account in selectedAccounts)
                    {
                        accountDetails.ForEach(accountDetail =>
                        {
                            var moduleConfig = accountDetail.ActivityManager.LstModuleConfiguration.FirstOrDefault(mc => mc.ActivityType == _activityType);

                            if (moduleConfig.IsEnabled)
                                objErrorModelControl.Accounts.Add(new ErrorModelControl
                                {
                                    UserName = accountHavingTemplates
                                    .FirstOrDefault(x => x.AccountBaseModel.UserName == account)?.AccountBaseModel.UserName
                                });
                        });

                    }

                }
                catch (Exception ex)
                {
                }
                try
                {
                    //Check if account is running with campaign or not if any account running with campaign then it will show ErrorModel
                    //there you can update campaign
                    if (objErrorModelControl.Accounts.Count > 0)
                    {
                        var objDialog = new Dialog();

                        var warningWindow = objDialog.GetMetroWindow(objErrorModelControl, "Warning");

                        //if we want to replace prvious setting we need to click save button
                        objErrorModelControl.BtnSave.Click += (senders, events) =>
                        {
                            try
                            {
                                var unSelectedAccountForModification = objErrorModelControl.Accounts.Where(x => x.IsChecked == false).Select(x => x.UserName).ToList();

                                //To remove the account which we don't want to update with new Configuration
                                unSelectedAccountForModification.ForEach(item => selectedAccounts.Remove(item));

                                //it will check if account is updated or not, if updated then will delete account and save that updated details
                                UpdateSelectedAccountDetails(accountDetails, selectedAccounts, runningTime);

                                //To update campaign file calling UpdateCampaignBinFile() method
                                UpdateCampaignBinFile(accountDetails, selectedAccounts);
                                warningWindow.Visibility = Visibility.Hidden;
                                warningWindow.Close();
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        };
                        objErrorModelControl.BtnCancel.Click += (senders, events) =>
                        {
                            _isCancelledUpdate = true;
                            warningWindow.Close();
                        };
                        warningWindow.ShowDialog();
                    }

                    else // if account is not running with any campaign then it will save 
                    {
                        //it will check if account is update or not if updated then will delete account and save that updated details
                        UpdateSelectedAccountDetails(accountDetails, selectedAccounts, runningTime);

                        //To update campaign file calling UpdateCampaignBinFile() method
                        UpdateCampaignBinFile(accountDetails, selectedAccounts);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else // if account is not running with any campaign then it will save 

            {
                //it will check if account is update or not if updated then will delete account and save that updated details
                UpdateSelectedAccountDetails(accountDetails, selectedAccounts, runningTime);

                //To update campaign file calling UpdateCampaignBinFile() method
                UpdateCampaignBinFile(accountDetails, selectedAccounts);
            }
        }

        public bool UpdateSelectedAccountDetails(List<DominatorAccountModel> allAccountDetails, List<string> listSelectedAccounts, List<RunningTimes> runningTime)
        {
            var isAccountDetailsUpdated = false;

            var selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);

            foreach (var account in allAccountDetails)
            {
                if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
                    continue;

                isAccountDetailsUpdated = true;
                try
                {
                    if (account.ActivityManager.RunningTime == null)
                        account.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

                    var moduleConfiguration = account.ActivityManager.LstModuleConfiguration?.FirstOrDefault(y => y.ActivityType == _activityType);
                    if (moduleConfiguration == null)
                    {
                        moduleConfiguration = new ModuleConfiguration { ActivityType = _activityType };
                        account.ActivityManager.LstModuleConfiguration?.Add(moduleConfiguration);
                    }

                    DominatorScheduler.StopActivity(account.AccountBaseModel.AccountId, _activityType.ToString(), moduleConfiguration.TemplateId);

                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";

                    moduleConfiguration.TemplateId = TemplateId;
                    runningTime.ForEach(x =>
                    {
                        foreach (var timingRange in x.Timings)
                        {
                            timingRange.Module = _activityType.ToString();
                        }
                    });
                    account.ActivityManager.RunningTime = runningTime;
                    moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);
                    account.IsCretedFromNormalMode = true;

                    selectedAccounts.Add(account);

                    AccountsFileManager.Edit(account);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            // save all accounts and schedule actitvities of selected accounts            
            foreach (var account in selectedAccounts)
            {
                DominatorScheduler.ScheduleTodayJobs(account, _socialNetwork, _activityType);
                DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: account, network: _socialNetwork);
            }
            return isAccountDetailsUpdated;

        }

        public void UpdateCampaignBinFile(List<DominatorAccountModel> allAccountDetails, List<string> selectedAccounts)
        {
            var campaignsList = CampaignsFileManager.Get();

            if (campaignsList.Count == 0)
                return;

            try
            {
                foreach (var selectedAccount in selectedAccounts)
                {
                    var dominatorAccountModel = allAccountDetails.ToList()?.FirstOrDefault(x => x.AccountBaseModel.UserName == selectedAccount);

                    if (dominatorAccountModel == null) continue;

                    var templateId = dominatorAccountModel.ActivityManager.LstModuleConfiguration
                        .FirstOrDefault(y => y.ActivityType == _activityType)?.TemplateId;

                    //  DominatorScheduler.StopActivity(dominatorAccountModel.AccountBaseModel.AccountId, _activityType.ToString(), TemplateId);

                    foreach (var campaign in campaignsList)
                        if (templateId != null && campaign.TemplateId == templateId)
                            campaign.SelectedAccountList.Remove(selectedAccount);
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }

            CampaignsFileManager.UpdateCampaigns(campaignsList);
        }

        public void AddNewCampaign(List<string> listSelectedAccounts)
        {
            var existingCampaign = CampaignsFileManager.Get().FirstOrDefault(x => x.CampaignName == CampaignName);

            // If campaign with such name already exists
            if (existingCampaign != null)
            {
                var warningMessege = "This account is already running with " + _activityType + " configuration from another campaign. Saving this settings will override previous settings and remove this account from the campaign.\r\nWould you still like to proceed?";

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                        warningMessege, MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Yes", "No"));

                if (dialogResult == MessageDialogResult.Negative)
                    return;

                // Update campaign
                existingCampaign.CampaignName = CampaignName;
                existingCampaign.MainModule = _moduleName;
                existingCampaign.SubModule = _activityType.ToString();
                existingCampaign.SocialNetworks = _socialNetwork;
                existingCampaign.SelectedAccountList = listSelectedAccounts;
                existingCampaign.TemplateId = TemplateId;
                existingCampaign.CreationDate = DateTimeUtilities.GetEpochTime();
                existingCampaign.Status = "Active";
                existingCampaign.LastEditedDate = DateTimeUtilities.GetEpochTime();

                // update exitsting Campaign 
                CampaignsFileManager.Edit(existingCampaign);
            }

            // add new campaign
            else
            {
                var newCampaign = new CampaignDetails()
                {
                    CampaignName = CampaignName,
                    MainModule = _moduleName,
                    SubModule = _activityType.ToString(),
                    SocialNetworks = _socialNetwork,
                    SelectedAccountList = listSelectedAccounts,
                    TemplateId = TemplateId,
                    CreationDate = DateTimeUtilities.GetEpochTime(),
                    Status = "Active",
                    LastEditedDate = DateTimeUtilities.GetEpochTime(),
                };

                // create new database for campaign
                DataBaseHandler.CreateDataBase(newCampaign.CampaignId, _socialNetwork, DatabaseType.CampaignType);
                CampaignsFileManager.Add(newCampaign);
            }
        }

        #endregion

        [Obsolete("Dont use AccountGrowthHeader_OnSaveClick method instead use SaveAccountGrowthSettings")]
        protected void AccountGrowthHeader_OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateExtraProperty()) return;

            // Getting details of account
            var accounts = AccountsFileManager.GetAll();

            //Getting details of account having the user name  as selected account
            var selectedAccountDetails = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == _accountGrowthModeHeader.SelectedItem);

            if (selectedAccountDetails == null)
                return;

            var accountstemplateId = selectedAccountDetails.ActivityManager.LstModuleConfiguration
                .FirstOrDefault(y => y.ActivityType == _activityType)
                ?.TemplateId;

            if (selectedAccountDetails.IsCretedFromNormalMode)
            {
                selectedAccountDetails.IsCretedFromNormalMode = false;
                CampaignsFileManager.DeleteSelectedAccount(accountstemplateId, _accountGrowthModeHeader.SelectedItem);
                AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, selectedAccountDetails);
            }
            else
            {
                if (string.IsNullOrEmpty(accountstemplateId))
                    AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, selectedAccountDetails);

                // Updating existing template
                else
                    TemplatesFileManager.UpdateActivitySettings(accountstemplateId,
                        JsonConvert.SerializeObject((TModel)Model));
            }

            AccountsFileManager.Edit(selectedAccountDetails);

            if (!ValidateRunningTime()) return;

            UpdateRunningTime(Model.JobConfiguration, selectedAccountDetails);

            DominatorScheduler.ScheduleTodayJobs(selectedAccountDetails, SocialNetworks.Instagram, _activityType);
            DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: selectedAccountDetails, network: selectedAccountDetails.AccountBaseModel.AccountNetwork);
            DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!", MessageDialogStyle.Affirmative);
        }

        protected bool SaveAccountGrowthSettings()
        {
            if (!ValidateExtraProperty()) return false;

            // Getting details of account
            var accounts = AccountsFileManager.GetAll();

            //Getting details of account having the user name  as selected account
            var selectedAccountDetails = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == _accountGrowthModeHeader.SelectedItem);

            if (selectedAccountDetails == null)
                return false;

            var accountstemplateId = selectedAccountDetails.ActivityManager.LstModuleConfiguration
                .FirstOrDefault(y => y.ActivityType == _activityType)
                ?.TemplateId;

            if (selectedAccountDetails.IsCretedFromNormalMode)
            {
                selectedAccountDetails.IsCretedFromNormalMode = false;
                CampaignsFileManager.DeleteSelectedAccount(accountstemplateId, _accountGrowthModeHeader.SelectedItem);
                AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, selectedAccountDetails);
            }
            else
            {
                if (string.IsNullOrEmpty(accountstemplateId))
                    AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, selectedAccountDetails);

                // Updating existing template
                else
                    TemplatesFileManager.UpdateActivitySettings(accountstemplateId,
                        JsonConvert.SerializeObject((TModel)Model));
            }

            AccountsFileManager.Edit(selectedAccountDetails);

            if (!ValidateRunningTime()) return false;

            UpdateRunningTime(Model.JobConfiguration, selectedAccountDetails);

            DominatorScheduler.ScheduleTodayJobs(selectedAccountDetails, SocialNetworks.Instagram, _activityType);
            DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: selectedAccountDetails, network: selectedAccountDetails.AccountBaseModel.AccountNetwork);
            DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!", MessageDialogStyle.Affirmative);
            return true;
        }



        public void UpdateRunningTime(JobConfiguration jobConfiguration, DominatorAccountModel account)
        {
            jobConfiguration.RunningTime.ForEach(x =>
            {
                foreach (var timingRange in x.Timings)
                    timingRange.Module = _activityType.ToString();
            });
            account.ActivityManager.RunningTime = jobConfiguration.RunningTime;
            var accountModuleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);

            if (accountModuleSettings != null)
                accountModuleSettings.LstRunningTimes = jobConfiguration.RunningTime;
        }

        private static void AddNewTemplate<T>(T moduleToSave, string userName, ActivityType moduleType, DominatorAccountModel account) where T : class
        {

            var first = account.ActivityManager?.LstModuleConfiguration.FirstOrDefault
                (x => x.ActivityType == moduleType);

            if (first != null)
                first.TemplateId =
                    TemplateModel.SaveTemplate(moduleToSave,
                        moduleType.ToString(), account.AccountBaseModel.AccountNetwork,
                        userName + "_" + moduleType + "_Template");
        }


        // Update Campaign        
        protected void FooterControl_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
        {
            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;

            var Module = _activityType.ToString();

            // Update Template Detail
            TemplatesFileManager.ApplyFunc(template =>
            {
                if (template.Id != TemplateId)
                    return false;

                TModel model = JsonConvert.DeserializeObject<TModel>(template.ActivitySettings);
                var firstRunningTime = ((dynamic)model).JobConfiguration.RunningTime;
                var secondRunningTime = Model.JobConfiguration.RunningTime;

                var result = DominatorScheduler.CompareRunningTime(firstRunningTime, secondRunningTime);
                if (!result)
                {
                    try
                    {
                        var lstAccountDetails = AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);

                        foreach (var accountModel in lstAccountDetails.Where(x => _footerControl.list_SelectedAccounts.Contains(x.AccountBaseModel.UserName)))
                            DominatorScheduler.StopActivity(accountModel.AccountBaseModel.AccountId, Module, TemplateId);

                        //foreach (var acc in _footerControl.list_SelectedAccounts)
                        //    DominatorScheduler.StopActivity(acc, Module, TemplateId);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    // Update the template configuration 
                    template.ActivitySettings = JsonConvert.SerializeObject((TModel)Model);
                }

                return true;
            });


            // Update Campaign Details
            CampaignsFileManager.ApplyAction(campaign =>
            {

                if (campaign.TemplateId == TemplateId)
                {
                    campaign.CampaignName = CampaignName;
                    campaign.MainModule = _moduleName;
                    campaign.SubModule = _activityType.ToString();
                    campaign.SocialNetworks = _socialNetwork;
                    campaign.SelectedAccountList = _footerControl.list_SelectedAccounts;
                    campaign.TemplateId = TemplateId;
                    campaign.CreationDate = DateTimeUtilities.GetEpochTime();
                    campaign.Status = "Active";
                    campaign.LastEditedDate = DateTimeUtilities.GetEpochTime();
                }
            });


            // Update Account Detail
            var AccountDetails = AccountsFileManager.GetAll();
            if (UpdateSelectedAccountDetails(AccountDetails, _footerControl.list_SelectedAccounts, Model.JobConfiguration))
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Update", "Update Successfull", MessageDialogStyle.Affirmative);

            SetDataContext();
            TabSwitcher.GoToCampaign();
        }




        /// <summary>
        /// Calls when user click 'Select Accounts' button in footer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FooterControl_OnSelectAccountChanged(object sender, RoutedEventArgs e)
        {
            try
            {

                var objSelectAccountControl = new SelectAccountControl(_footerControl.list_SelectedAccounts,
                    filterForActiveSocialNetwork: true);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl, "Select Account");

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    if (objSelectAccountControl.GetSelectedAccount().Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        this.SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
                        GlobusLogHelper.log.Info(Log.SelectedAccount,SocinatorInitialize.ActiveSocialNetwork, _footerControl.list_SelectedAccounts.Count,_activityType);
                    }
                    else
                    {
                        this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
                    }
                    window.Close();
                };

                objSelectAccountControl.btnCancel.Click += (senders, events) => window.Close();
                window.ShowDialog();

            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
            }
        }

        protected void HeaderControl_OnCancelEditClick(object sender, RoutedEventArgs e)
        {
            SetDataContext();
        }

        protected virtual void SetDataContext()
        {
            IsEditCampaignName = true;

            CancelEditVisibility = Visibility.Collapsed;

            _footerControl.CampaignManager = ConstantVariable.CreateCampaign;

            this.SelectedAccountCount = ConstantVariable.NoAccountSelected;

            ObjViewModel = new TViewModel();

            _footerControl.list_SelectedAccounts = new List<string>();

            _mainGrid.DataContext = Model as TModel;

            _headerControl.DataContext = _footerControl.DataContext = this;

            CampaignName = $"{_socialNetwork} {_activityType.ToString()} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
        }
        [Obsolete("This method is going to remove, use instead with name but pass network as the parameter from respective configuration module")]
        protected virtual void SetAccountModeDataContext()
        {
            try
            {
                var accountDetails = AccountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem);

                var moduleConfiguration = accountDetails.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(y => y.ActivityType == _activityType);

                if (moduleConfiguration == null)
                {
                    if (accountDetails.ActivityManager.RunningTime == null)
                        accountDetails.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

                    moduleConfiguration = new ModuleConfiguration() { ActivityType = _activityType };
                    accountDetails.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);

                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";
                    AccountsFileManager.Edit(accountDetails);
                    SetModuleValues(false, null);
                }

                else
                {
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }

                _mainGrid.DataContext = Model as TModel;
                _accountGrowthModeHeader.DataContext = this;
                SetSelectedAccounts(accountDetails.AccountBaseModel.AccountNetwork, _accountGrowthModeHeader.SelectedItem);


            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        protected virtual void SetAccountModeDataContext(SocialNetworks network)
        {
            try
            {
                var accountDetails = AccountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, network);

                var moduleConfiguration = accountDetails.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(y => y.ActivityType == _activityType);

                if (moduleConfiguration == null)
                {
                    if (accountDetails.ActivityManager.RunningTime == null)
                        accountDetails.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

                    moduleConfiguration = new ModuleConfiguration() { ActivityType = _activityType };
                    accountDetails.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);

                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";
                    AccountsFileManager.Edit(accountDetails);
                    SetModuleValues(false, null);
                }

                else
                {
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }

                _mainGrid.DataContext = Model as TModel;
                _accountGrowthModeHeader.DataContext = this;
                SetSelectedAccounts(accountDetails.AccountBaseModel.AccountNetwork, _accountGrowthModeHeader.SelectedItem);


            }

            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void SetSelectedAccounts(SocialNetworks networks, string selectedAccounts)
        {
            var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.UserName));
            _accountGrowthModeHeader.AccountItemSource = accounts;
            switch (networks)
            {
                case SocialNetworks.Facebook:
                    SelectedDominatorAccounts.FdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Instagram:
                    SelectedDominatorAccounts.GdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Twitter:
                    SelectedDominatorAccounts.TdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Pinterest:
                    SelectedDominatorAccounts.PdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.LinkedIn:
                    SelectedDominatorAccounts.LdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Reddit:
                    SelectedDominatorAccounts.RdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Quora:
                    SelectedDominatorAccounts.QdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Gplus:
                    SelectedDominatorAccounts.GplusAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Youtube:
                    SelectedDominatorAccounts.YdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Tumblr:
                    SelectedDominatorAccounts.TumblrAccounts = selectedAccounts;
                    break;
            }
            _accountGrowthModeHeader.SelectedItem = selectedAccounts ?? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "");
        }

        /// <summary>
        /// UpdateSelectedAccountDetails method will take AccountDetails and list of selected account to modify as  argument and
        /// will update that account with setting and return status
        /// </summary>
        /// <param name="allAccountDetails"></param>
        /// <param name="listSelectedAccounts"></param>
        /// <returns></returns>
        protected bool UpdateSelectedAccountDetails(IEnumerable<DominatorAccountModel> allAccountDetails,
                    List<string> listSelectedAccounts, JobConfiguration jobConfiguration)
        {
            bool isAccountDetailsUpdated = false;

            List<DominatorAccountModel> selectedAccounts = new List<DominatorAccountModel>(listSelectedAccounts.Count);
            foreach (var account in allAccountDetails)
            {
                if (!listSelectedAccounts.Contains(account.AccountBaseModel.UserName))
                    continue;

                isAccountDetailsUpdated = true;
                try
                {
                    if (account.ActivityManager.RunningTime == null)
                        account.ActivityManager.RunningTime = RunningTimes.DayWiseRunningTimes;

                    var moduleConfiguration = account.ActivityManager.LstModuleConfiguration?.FirstOrDefault(y => y.ActivityType == _activityType);
                    if (moduleConfiguration == null)
                    {
                        moduleConfiguration = new ModuleConfiguration() { ActivityType = _activityType };
                        account.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
                    }
                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";
                    moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);
                    // Update running times for current activity
                    UpdateRunningTime(jobConfiguration, account);

                    account.IsCretedFromNormalMode = true;

                    selectedAccounts.Add(account);

                    AccountsFileManager.Edit(account);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            // save all accounts and schedule actitvities of selected accounts            
            foreach (var account in selectedAccounts)
            {
                DominatorScheduler.ScheduleTodayJobs(account, account.AccountBaseModel.AccountNetwork, _activityType);
                DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: account, network: account.AccountBaseModel.AccountNetwork);
            }

            return isAccountDetailsUpdated;
        }
        protected void ScheduleJobFromGrowthMode(bool isStart, string selectedAccount, SocialNetworks socialNetworks)
        {
            var accountModel = AccountsFileManager.GetAccount(selectedAccount);
            var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);
            if (moduleConfiguration.IsEnabled && isStart)
                return;
            var accountstemplateId = moduleConfiguration.TemplateId;
            if (isStart)
            {
                moduleConfiguration.IsEnabled = true;
                DominatorScheduler.ScheduleTodayJobs(accountModel, socialNetworks, _activityType);
            }
            else
            {
                moduleConfiguration.IsEnabled = false;
                DominatorScheduler.StopActivity(accountModel.AccountBaseModel.AccountId,
                    _activityType.ToString(), accountstemplateId);
            }
            UpdateAccountAndTemplate(accountModel, accountstemplateId);
        }

        private void UpdateAccountAndTemplate(DominatorAccountModel accountModel, string accountstemplateId)
        {
            if (accountModel.IsCretedFromNormalMode)
            {
                accountModel.IsCretedFromNormalMode = false;
                CampaignsFileManager.DeleteSelectedAccount(accountstemplateId, _accountGrowthModeHeader.SelectedItem);
                AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, accountModel);
            }
            else
            {
                if (string.IsNullOrEmpty(accountstemplateId))
                    AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, accountModel);

                // Updating existing template
                else
                    TemplatesFileManager.UpdateActivitySettings(accountstemplateId,
                        JsonConvert.SerializeObject((TModel)Model));
            }

            AccountsFileManager.Edit(accountModel);
        }


        #region IAccountGrowthModeHeader

        public ObservableCollectionBase<string> AccountItemSource
        {
            get
            {
                return _accountItemSource;
            }
            set
            {
                _accountItemSource = value;
                OnPropertyChanged(nameof(_accountItemSource));
            }
        }

        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(_selectedItem));
            }
        }

        #endregion
    }
}
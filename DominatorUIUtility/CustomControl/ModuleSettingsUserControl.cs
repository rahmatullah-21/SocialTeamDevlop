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
using DominatorHouseCore.DatabaseHandler.Utility;
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

        #region Constructor

        protected ModuleSettingsUserControl() { }

        #endregion

        #region Properties

        HeaderControl _headerControl;
        FooterControl _footerControl;
        SearchQueryControl _queryControl;
        Grid _mainGrid;
        AccountGrowthModeHeader _accountGrowthModeHeader;
        ActivityType _activityType;
        string _moduleName;
        protected SocialNetworks SocialNetwork = SocinatorInitialize.ActiveSocialNetwork;

        bool _initialized = false;
        private bool _isCancelledUpdate = false;

        #endregion

        #region Module base Initializer

        public void InitializeBaseClass(
            Grid MainGrid,
            ActivityType activityType,
            string moduleName,
            HeaderControl header = null,
            FooterControl footer = null,
            SearchQueryControl queryControl = null,
            AccountGrowthModeHeader accountGrowthModeHeader = null
            )
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

        #endregion

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

        #region Properties

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

        #endregion

        #region PropertyChanged section

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region Base Methods for module settings 

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


        #endregion

        #region Set Data context

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

            CampaignName = $"{SocialNetwork} {_activityType.ToString()} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
        }

        #endregion

        #region Create Campaign 

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
                TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), SocialNetwork, CampaignName);

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
                        DominatorScheduler.ScheduleTodayJobs(account, SocialNetwork, _activityType);
                        DominatorScheduler.ScheduleForEachModule(_activityType, account, SocialNetwork);
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

                if (moduleSettings != null)
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

            if (objErrorModelControl.Accounts.Count != 0)
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

                moduleConfiguration.IsTemplateMadeByCampaignMode = true;

                runningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings)
                    {
                        timingRange.Module = _activityType.ToString();
                    }
                });

                account.ActivityManager.RunningTime = runningTime;

                moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);

                AccountsFileManager.Edit(account);
            });

        }

        public void SaveTemplateToCampaigns()
        {
            var campaignDetails = new CampaignDetails
            {
                CampaignName = CampaignName,
                MainModule = _moduleName,
                SubModule = _activityType.ToString(),
                SocialNetworks = SocialNetwork,
                SelectedAccountList = _footerControl.list_SelectedAccounts,
                TemplateId = TemplateId,
                CreationDate = DateTimeUtilities.GetEpochTime(),
                Status = "Active",
                LastEditedDate = DateTimeUtilities.GetEpochTime(),
            };

            var dbOperations =
                new DbOperations(campaignDetails.CampaignId, SocialNetwork, ConstantVariable.GetCampaignDb);

            DataBaseHandler.DbCampaignInitialCounters[SocialNetwork](dbOperations);

            CampaignsFileManager.Add(campaignDetails);
        }

        #endregion

        #region Campaign related functions

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
                    var selectedAccount = objSelectAccountControl.GetSelectedAccount();
                    if (selectedAccount.Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        this.SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
                        GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork, _footerControl.list_SelectedAccounts.Count, _activityType);
                    }
                    else
                    {
                        this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
                        _footerControl.list_SelectedAccounts = selectedAccount.ToList();
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
            TabSwitcher.GoToCampaign();
        }


        #endregion

        #region Update campaign

        protected void UpdateCampaign(string errorMessage)
        {
            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;

            var currentCampaign = CampaignsFileManager.Get().FirstOrDefault(camp => camp.TemplateId == TemplateId);

            try
            {
                var newlyAddedAccounts = _footerControl.list_SelectedAccounts.Except(currentCampaign?.SelectedAccountList).ToList();
                var existingAccounts = _footerControl.list_SelectedAccounts.Except(newlyAddedAccounts).ToList();
                var removedAccounts = currentCampaign?.SelectedAccountList.Except(existingAccounts).ToList();

                var accountToRemoveModuleConfiguration = AccountsFileManager.GetAll(removedAccounts);

                #region Remove TemplateId from removed account from campaign selected account list

                accountToRemoveModuleConfiguration.ForEach(account =>
                {

                    try
                    {
                        var moduleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module =>
                            module.ActivityType == _activityType);
                        DominatorScheduler.StopActivity(account.AccountBaseModel.AccountId, _activityType.ToString(), moduleSettings?.TemplateId);
                        moduleSettings.TemplateId = null;
                        AccountsFileManager.Edit(account);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }


                });

                #endregion

                if (newlyAddedAccounts.Count != 0)
                    UpdateNewlyAddedAccounts(newlyAddedAccounts);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }



            // Update Template Details
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
                            DominatorScheduler.StopActivity(accountModel.AccountBaseModel.AccountId, _activityType.ToString(), TemplateId);

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
                    campaign.SocialNetworks = SocialNetwork;
                    campaign.SelectedAccountList = _footerControl.list_SelectedAccounts;
                    campaign.TemplateId = TemplateId;
                    campaign.CreationDate = DateTimeUtilities.GetEpochTime();
                    campaign.Status = "Active";
                    campaign.LastEditedDate = DateTimeUtilities.GetEpochTime();
                }
            });


            // Update Account Detail
            var accountDetails = AccountsFileManager.GetAll();
            if (UpdateSelectedAccountDetails(accountDetails, _footerControl.list_SelectedAccounts, Model.JobConfiguration))
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Update", "Update Successfull", MessageDialogStyle.Affirmative);

            SetDataContext();
            TabSwitcher.GoToCampaign();

        }

        protected void UpdateCampaign()
        {

            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;

            var currentCampaign = CampaignsFileManager.Get().FirstOrDefault(camp => camp.TemplateId == TemplateId);

            try
            {
                var newlyAddedAccounts = _footerControl.list_SelectedAccounts.Except(currentCampaign?.SelectedAccountList).ToList();
                var existingAccounts = _footerControl.list_SelectedAccounts.Except(newlyAddedAccounts).ToList();
                var removedAccounts = currentCampaign?.SelectedAccountList.Except(existingAccounts).ToList();

                var accountToRemoveModuleConfiguration = AccountsFileManager.GetAll(removedAccounts);

                #region Remove TemplateId from removed account from campaign selected account list

                accountToRemoveModuleConfiguration.ForEach(account =>
                {

                    try
                    {
                        var moduleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module =>
                            module.ActivityType == _activityType);
                        DominatorScheduler.StopActivity(account.AccountBaseModel.AccountId, _activityType.ToString(), moduleSettings?.TemplateId);
                        moduleSettings.TemplateId = null;
                        AccountsFileManager.Edit(account);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }


                });

                #endregion

                if (newlyAddedAccounts.Count != 0)
                    UpdateNewlyAddedAccounts(newlyAddedAccounts);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }



            // Update Template Details
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
                            DominatorScheduler.StopActivity(accountModel.AccountBaseModel.AccountId, _activityType.ToString(), TemplateId);

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
                    campaign.SocialNetworks = SocialNetwork;
                    campaign.SelectedAccountList = _footerControl.list_SelectedAccounts;
                    campaign.TemplateId = TemplateId;
                    campaign.CreationDate = DateTimeUtilities.GetEpochTime();
                    campaign.Status = "Active";
                    campaign.LastEditedDate = DateTimeUtilities.GetEpochTime();
                }
            });


            // Update Account Detail
            var accountDetails = AccountsFileManager.GetAll();
            if (UpdateSelectedAccountDetails(accountDetails, _footerControl.list_SelectedAccounts, Model.JobConfiguration))
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Update", "Update Successfull", MessageDialogStyle.Affirmative);

            SetDataContext();
            TabSwitcher.GoToCampaign();

        }

        string GetWarningLangRsrc()
        {
            try
            {
                return Application.Current.FindResource("DHStartWarning") + $" {_activityType} " +
                                       Application.Current.FindResource("DHEndWarning");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "Account Warning";
            }
        }

        void UpdateNewlyAddedAccounts(List<string> newlyAddedAccounts)
        {
            #region Get the accounts which holds template Id

            var accountDetails = AccountsFileManager.GetAll(newlyAddedAccounts);

            var accountHavingTemplates = accountDetails.Where(x => x.ActivityManager.LstModuleConfiguration.FirstOrDefault(y => y.ActivityType == _activityType)?.TemplateId != null).ToList();

            #endregion

            #region If account having any template then Show ErrorModelControl with warning

            if (accountHavingTemplates.Count != 0)
            {
                var objErrorModelControl = new ErrorModelControl { WarningText = GetWarningLangRsrc() };

                #region Adding the accounts to Warning window

                accountHavingTemplates.ForEach(account =>
                {
                    var moduleSettings =
                        account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module =>
                            module.ActivityType == _activityType);

                    if (moduleSettings != null)
                    {
                        objErrorModelControl.Accounts.Add(new ErrorModelControl
                        {
                            UserName = account.AccountBaseModel.UserName
                        });
                    }
                });

                var warningWindow = new Dialog().GetMetroWindow(objErrorModelControl, "Warning");

                #endregion

                #region Warning windows save button event

                objErrorModelControl.BtnSave.Click += (senders, events) =>
                {
                    #region Remove not selected accounts from errorModelControl

                    var nonSelectedAccounts = objErrorModelControl.Accounts.Where(x => !x.IsChecked)
                        .Select(x => x.UserName)
                        .ToList();

                    nonSelectedAccounts.ForEach(removingAccount =>
                    {
                        _footerControl.list_SelectedAccounts.Remove(removingAccount);
                    });

                    #endregion

                    var selectedAccount = objErrorModelControl.Accounts.Where(x => x.IsChecked).Select(x => x.UserName)
                        .ToList();

                    if (selectedAccount.Count == 0)
                    {
                        warningWindow.Close();
                        return;
                    }

                    #region Update already existing account setting

                    accountDetails.ForEach(account =>
                    {
                        if (!selectedAccount.Contains(account.AccountBaseModel.UserName))
                            return;

                        var moduleSettings =
                            account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module =>
                                module.ActivityType == _activityType);

                        if (moduleSettings == null)
                            return;
                        CampaignsFileManager.DeleteSelectedAccount(moduleSettings.TemplateId,
                            account.AccountBaseModel.UserName);

                        moduleSettings.TemplateId = TemplateId;

                        DominatorScheduler.StopActivity(account.AccountBaseModel.AccountId, _activityType.ToString(),
                            moduleSettings.TemplateId);
                    });

                    #endregion


                    AccountsFileManager.UpdateAccounts(accountDetails);

                    warningWindow.Close();
                };

                #endregion

                #region Warning windows cancel button event

                objErrorModelControl.BtnCancel.Click += (senders, events) =>
                {

                    #region Remove not selected accounts from errorModelControl

                    var nonSelectedAccounts = objErrorModelControl.Accounts.Where(x => !x.IsChecked)
                        .Select(x => x.UserName)
                        .ToList();

                    nonSelectedAccounts.ForEach(removingAccount =>
                    {
                        _footerControl.list_SelectedAccounts.Remove(removingAccount);
                    });

                    #endregion

                    warningWindow.Close();
                };

                #endregion

                if (objErrorModelControl.Accounts.Count != 0)
                    warningWindow.ShowDialog();

            }
            #endregion

            #region If account don't have any template then set that account template to current template

            else
            {

                #region Update newyly added account setting

                accountDetails.ForEach(account =>
                {
                    var moduleSettings =
                         account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module =>
                             module.ActivityType == _activityType);

                    moduleSettings.TemplateId = TemplateId;

                });

                #endregion

                AccountsFileManager.UpdateAccounts(accountDetails);
            }
            #endregion
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

                    moduleConfiguration.IsTemplateMadeByCampaignMode = true;

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
                DominatorScheduler.ScheduleTodayJobs(account, SocialNetwork, _activityType);
                DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: account, network: SocialNetwork);
            }
            return isAccountDetailsUpdated;

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



        #endregion

        #region Custom filter 

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



        #endregion

        #region Query adding 

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
                List<int> queryValuIndex = new List<int>();
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
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork, "{ " + string.Join(" },{ ", queryValuIndex.ToArray()) + " }", _activityType);
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

        #endregion

        #region  Old Save account configuration

        [Obsolete("Don't use SaveAccountGrowthSettings method with parameter instead use SaveIndividualAccountConfiguration with 2 parameters")]
        protected void SaveIndividualAccountConfiguration(string selectedAccount)
        {
            try
            {
                if (!ValidateExtraProperty()) return;
                if (!ValidateRunningTime()) return;
                var accountModel = AccountsFileManager.GetAccount(selectedAccount);
                var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);

                if (moduleConfiguration != null)
                {
                    var accountstemplateId = moduleConfiguration.TemplateId;
                    UpdateTemplate(accountModel, accountstemplateId);
                    UpdateRunningTime(Model.JobConfiguration, accountModel);
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                        "Successfully Saved !!!", MessageDialogStyle.Affirmative);

                }
                else
                {

                    #region Module Configuration Initialize

                    moduleConfiguration = new ModuleConfiguration { ActivityType = _activityType };

                    accountModel.ActivityManager.LstModuleConfiguration?.Add(moduleConfiguration);

                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();

                    TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), SocialNetwork, $"{accountModel.AccountBaseModel.AccountId}-Configuration");

                    moduleConfiguration.TemplateId = TemplateId;

                    var runningTime = (List<RunningTimes>)Model.JobConfiguration.RunningTime;

                    runningTime.ForEach(x =>
                    {
                        foreach (var timingRange in x.Timings)
                        {
                            timingRange.Module = _activityType.ToString();
                        }
                    });

                    accountModel.ActivityManager.RunningTime = runningTime;

                    moduleConfiguration.LstRunningTimes = new List<RunningTimes>(accountModel.ActivityManager.RunningTime);

                    accountModel.IsCretedFromNormalMode = false;

                    AccountsFileManager.Edit(accountModel);

                    #endregion

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        protected void SaveIndividualAccountConfiguration()
        {
            try
            {
                if (!ValidateExtraProperty()) return;
                if (!ValidateRunningTime()) return;
                //var selectedAccountDetails = accounts.FirstOrDefault(x => x.AccountBaseModel.UserName == _accountGrowthModeHeader.SelectedItem);
                var accountModel = AccountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, SocialNetwork);
                var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);
                var accountstemplateId = moduleConfiguration.TemplateId;
                UpdateTemplate(accountModel, accountstemplateId);
                UpdateRunningTime(Model.JobConfiguration, accountModel);
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!", MessageDialogStyle.Affirmative);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateTemplate(DominatorAccountModel accountModel, string accountstemplateId)
        {
            try
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
            }
            catch (Exception ex)
            {
                ex.DebugLog("Accounts details not saved!");
            }
        }


        #endregion

        #region Save Configuration Implementation

        #region Toggle Module Status changes 

        protected bool ChangeAccountsModuleStatus(bool isStart, string selectedAccount, SocialNetworks socialNetworks)
        {
            try
            {
                var accountModel = AccountsFileManager.GetAccount(selectedAccount, socialNetworks);

                var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);

                if (moduleConfiguration?.TemplateId == null || moduleConfiguration.LstRunningTimes == null)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please save your settings, before starting the activity.");
                    return false;
                }

                moduleConfiguration.IsEnabled = isStart;

                if (moduleConfiguration.IsEnabled)
                    DominatorScheduler.ScheduleTodayJobs(accountModel, accountModel.AccountBaseModel.AccountNetwork,
                        _activityType);
                else
                    DominatorScheduler.StopActivity(accountModel.AccountBaseModel.AccountId, _activityType.ToString(),
                        moduleConfiguration.TemplateId);

                AccountsFileManager.Edit(accountModel);

                return moduleConfiguration.IsEnabled;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        #endregion

        #region Account selection changed events

        protected virtual void SetAccountModeDataContext(SocialNetworks network)
        {
            try
            {
                SocialNetwork = network;

                var accountDetails = AccountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, network);

               SocinatorInitialize.GetSocialLibrary(network)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount = _accountGrowthModeHeader.SelectedItem;

                var moduleConfiguration = accountDetails.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(y => y.ActivityType == _activityType);

                if (moduleConfiguration != null)
                {
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }
                else
                    SetModuleValues(false, null);

                _mainGrid.DataContext = Model as TModel;
                _accountGrowthModeHeader.DataContext = this;
                SetSelectedAccounts(accountDetails.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Save last selected accounts in account configuration mode

        [Obsolete("Dont use this method instead use SetSelectedAccounts with single parameter")]
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

        public void SetSelectedAccounts(SocialNetworks networks)
        {
            var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.UserName));

            _accountGrowthModeHeader.AccountItemSource = accounts;

            _accountGrowthModeHeader.SelectedItem = SocinatorInitialize.GetSocialLibrary(networks)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
        }

        #endregion

        public void SaveConfigurations()
        {

            if (!ValidateExtraProperty()) return;

            if (!ValidateRunningTime()) return;

            var accountModel = AccountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, SocialNetwork);

            var accounts = AccountCustomControl.GetAccountCustomControl(SocialNetwork).DominatorAccountViewModel
                  .LstDominatorAccountModel.FirstOrDefault(x => x.AccountBaseModel.AccountId == accountModel.AccountBaseModel.AccountId);

            accounts?.NotifyCancelled();

            var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);

            if (moduleConfiguration?.TemplateId != null)
            {
                #region Template Id present case

                // need to check whether its running or not, if its running then need to stop process
                DominatorScheduler.StopActivity(accountModel.AccountBaseModel.AccountId, _activityType.ToString(),
                    moduleConfiguration.TemplateId);

                // update the template
                UpdateTemplate(accountModel, moduleConfiguration.TemplateId,
                    moduleConfiguration.IsTemplateMadeByCampaignMode);

                // update the running time
                UpdateRunningTime(Model.JobConfiguration, accountModel);

                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!");

                #endregion
            }
            else
            {
                #region Template not present case

                moduleConfiguration = new ModuleConfiguration { ActivityType = _activityType };

                accountModel.ActivityManager.LstModuleConfiguration?.Add(moduleConfiguration);

                moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();

                TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), SocialNetwork, $"{accountModel.AccountBaseModel.AccountId}-Configuration");

                moduleConfiguration.TemplateId = TemplateId;

                var runningTime = (List<RunningTimes>)Model.JobConfiguration.RunningTime;

                runningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings)
                    {
                        timingRange.Module = _activityType.ToString();
                    }
                });

                accountModel.ActivityManager.RunningTime = runningTime;

                moduleConfiguration.LstRunningTimes = new List<RunningTimes>(accountModel.ActivityManager.RunningTime);

                moduleConfiguration.IsTemplateMadeByCampaignMode = false;

                AccountsFileManager.Edit(accountModel);

                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!");
                #endregion

            }
        }

        private static void AddNewTemplate<T>(T moduleToSave, string userName, ActivityType moduleType, DominatorAccountModel account) where T : class
        {
            var accountModuleConfiguration = account.ActivityManager?
                .LstModuleConfiguration
                .FirstOrDefault(x => x.ActivityType == moduleType);

            if (accountModuleConfiguration == null)
                return;

            accountModuleConfiguration.TemplateId = TemplateModel.SaveTemplate(
                moduleToSave,
                moduleType.ToString(),
                account.AccountBaseModel.AccountNetwork,
                userName + "_" + moduleType + "_Template");

            accountModuleConfiguration.IsTemplateMadeByCampaignMode = false;
        }

        private void UpdateTemplate(DominatorAccountModel accountModel, string accountstemplateId, bool isTemplateMadeByCampaignMode)
        {
            try
            {
                if (isTemplateMadeByCampaignMode)
                {
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
            }
            catch (Exception ex)
            {
                ex.DebugLog("Accounts details not saved!");
            }
        }

        public void UpdateRunningTime(JobConfiguration jobConfiguration, DominatorAccountModel account)
        {
            try
            {
                jobConfiguration.RunningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings)
                        timingRange.Module = _activityType.ToString();
                });

                account.ActivityManager.RunningTime = jobConfiguration.RunningTime;

                var accountModuleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == _activityType);

                if (accountModuleSettings != null)
                {
                    if (accountModuleSettings.LstRunningTimes == null)
                        accountModuleSettings.LstRunningTimes = new List<RunningTimes>();

                    accountModuleSettings.LstRunningTimes = jobConfiguration.RunningTime;
                }

                AccountsFileManager.Edit(account);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        #endregion

    }
}
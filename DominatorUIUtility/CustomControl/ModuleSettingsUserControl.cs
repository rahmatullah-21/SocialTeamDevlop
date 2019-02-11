using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorUIUtility.Navigations;

using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ScreenTipMode;
using DominatorUIUtility.ScreenTip.PopUpstyle;

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
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IAccountsCacheService _accountsCacheService;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IDominatorScheduler _dominatorScheduler;
        private readonly IDataBaseHandler _dataBaseHandler;


        #region Constructor

        protected ModuleSettingsUserControl()
        {
            _jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            _accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
            _accountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            _dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
            _dataBaseHandler = ServiceLocator.Current.GetInstance<IDataBaseHandler>();
            CreateCampaignCommand = new BaseCommand<object>((sender) => true, CreateOrUpdateCampaign);
            SelectAccountCommand = new BaseCommand<object>((sender) => true, (sender) => SelectAccount());
            CancelEditCommand = new BaseCommand<object>((sender) => true, (sender) =>
            {
                SetDataContext();
                TabSwitcher.GoToCampaign();
            });
            InfoCommand = new BaseCommand<object>((sender) => true, (sender) => { IsOpen = true; });
            SaveConfigurationsCommand = new BaseCommand<object>((sender) => true, (sender) => SaveConfigurations());
            LoadedCommand = new BaseCommand<object>((sender) => true, (sender) => SetSelectedAccounts());
            SelectionChangedCommand = new BaseCommand<object>((sender) => true, (sender) => SetAccountModeDataContext());
            StatusChangedCommand = new BaseCommand<object>((sender) => true, (sender) => AccountModeStatusChange());
            CmdStartModuleOverView = new BaseCommand<object>((sender) => true, StartModuleOverView);
        }

        private void CreateOrUpdateCampaign(object sender)
        {

            var control = sender as FooterControl;
            if (control.CampaignManager.Equals(ConstantVariable.CreateCampaign, StringComparison.CurrentCultureIgnoreCase))
                CreateCampaign();
            else
                UpdateCampaign();
        }

        public ICommand CreateCampaignCommand { get; set; }
        public ICommand SelectAccountCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand SaveConfigurationsCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand StatusChangedCommand { get; set; }
        public ICommand CmdStartModuleOverView { get; set; }

        #endregion

        #region Properties


        public HeaderControl _headerControl;
        public FooterControl _footerControl;
        public SearchQueryControl _queryControl;
        private Grid _mainGrid;
        public AccountGrowthModeHeader _accountGrowthModeHeader;
        private ActivityType _activityType;
        private string _moduleName;
        protected SocialNetworks SocialNetwork = SocinatorInitialize.ActiveSocialNetwork;
        private bool _initialized;
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

            if (_initialized)
                return;

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
        private bool _isOpen;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }


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

        public static TViewModel Instances { get; } = new TViewModel();

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
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please select at least one account.");
                return false;
            }
            //Check Query
            //if (!ValidateQuery())
            //    return false;
            // Check timings
            return ValidateRunningTime();
        }

        private bool ValidateRunningTime()
        {
            if (((IEnumerable<RunningTimes>)Model.JobConfiguration.RunningTime).All(rt => rt.Timings.Count == 0))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one time range when to run and stop the activity.");
                return false;
            }
            return true;
        }

        protected virtual bool ValidateExtraProperty() => true;

        protected virtual bool ValidateQuery()
        {

            if (Model.SavedQueries.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one query.");
                return false;
            }

            return true;
        }

        #endregion

        #region Set Data context
        protected virtual void SetDataContext()
        {
            IsEditCampaignName = true;

            CancelEditVisibility = Visibility.Collapsed;

            _footerControl.CampaignManager = ConstantVariable.CreateCampaign;

            SelectedAccountCount = ConstantVariable.NoAccountSelected;

            ObjViewModel = new TViewModel();

            _footerControl.list_SelectedAccounts = new List<string>();

            // _mainGrid.DataContext = Model as TModel;
            _mainGrid.DataContext = ObjViewModel;

            _headerControl.DataContext = _footerControl.DataContext = this;

            CampaignName = $"{SocialNetwork} {_activityType.ToString()} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
        }

        #endregion

        #region Create Campaign 

        protected void CreateCampaign()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!ValidateCampaign())
                    return;

                if (!ValidateExtraProperty())
                    return;

                var schedulePending = ImmutableQueue<Action>.Empty;
                if (IsNeedToSaveTemplate())
                {
                    //     UpdateJobconfiguration();
                    TemplateId = TemplateModel.SaveTemplate((TModel)Model, _activityType.ToString(), SocialNetwork,
                        CampaignName);
                    SaveTemplateToAccounts(TemplateId);
                    SaveTemplateToCampaigns();

                    var accountDetails =
                        _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);

                    try
                    {
                        new Thread(() =>
                        {
                            foreach (var account in accountDetails)
                            {
                                Action scheduleAccount = () =>
                                {
                                    _dominatorScheduler.ScheduleNextActivity(account, _activityType);
                                };
                                schedulePending = schedulePending.Enqueue(scheduleAccount);
                            }

                            while (!schedulePending.IsEmpty)
                            {
                                Action startSchedule;
                                schedulePending = schedulePending.Dequeue(out startSchedule);
                                startSchedule();
                            }

                        })
                        { IsBackground = true }.Start();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    SetDataContext();

                    TabSwitcher.GoToCampaign();

                }
            });

        }

        public void SaveTemplateToAccounts(string templateId, List<RunningTimes> runningTime)
        {
            var accountDetails = _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);

            accountDetails.ForEach(account =>
            {
                var moduleConfiguration = _jobActivityConfigurationManager[account.AccountId, _activityType] ?? new ModuleConfiguration { ActivityType = _activityType };
                moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                moduleConfiguration.IsEnabled = true;
                moduleConfiguration.Status = "Active";
                moduleConfiguration.TemplateId = templateId;
                moduleConfiguration.IsTemplateMadeByCampaignMode = true;

                _jobActivityConfigurationManager.AddOrUpdate(account.AccountId, _activityType, moduleConfiguration);

                runningTime.ForEach(x =>
                {
                    foreach (var timingRange in x.Timings)
                    {
                        timingRange.Module = _activityType.ToString();
                    }
                });

                account.ActivityManager.RunningTime = runningTime;

                moduleConfiguration.LstRunningTimes = new List<RunningTimes>(account.ActivityManager.RunningTime);

                _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, moduleConfiguration);
            });
            _accountsCacheService.UpsertAccounts(accountDetails.ToArray());

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

            _dataBaseHandler.DbCampaignInitialCounters[SocialNetwork](dbOperations);

            var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
            campaignFileManager.Add(campaignDetails);

            //Updating Campaign UI
            Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel.LstCampaignDetails.Add(campaignDetails);
        }


        #region Createcampaign Method without errorMessage and runningTime

        public bool IsNeedToSaveTemplate()
        {
            bool needToCancel = false;
            #region Get the accounts which holds template Id

            var accountDetails = _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);

            var accountHavingTemplates = accountDetails.Where(x => _jobActivityConfigurationManager[x.AccountId, _activityType]?.TemplateId != null).ToList();

            if (accountHavingTemplates.Count == 0)
                return true;

            #endregion

            var objErrorModelControl = new ErrorModelControl { WarningText = GetWarningLangRsrc() };

            #region Adding the accounts to Warning window

            accountHavingTemplates.ForEach(account =>
            {
                if (_jobActivityConfigurationManager[account.AccountId, _activityType] != null)
                {
                    objErrorModelControl.Accounts.Add(new AccountDetails { UserName = account.AccountBaseModel.UserName });
                }
            });

            var warningWindow = new Dialog().GetMetroWindowWithOutClose(objErrorModelControl, "Warning");
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
                    accountDetails.Remove(accountDetails.FirstOrDefault(x => x.UserName == removingAccount));
                });
                #endregion

                var remainingAccount = _footerControl.list_SelectedAccounts.DeepCloneObject();
                var selectedAccount = objErrorModelControl.Accounts.Where(x => x.IsChecked).Select(x => x.UserName).ToList();

                if (selectedAccount.Count == 0)
                {
                    warningWindow.Close();
                    return;
                }

                var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                accountDetails.ForEach(account =>
                {
                    var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];

                    if (moduleSettings == null)
                        return;

                    campaignFileManager.DeleteSelectedAccount(moduleSettings.TemplateId,
                        account.AccountBaseModel.UserName);
                    var campToUpdate = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel.LstCampaignDetails.FirstOrDefault(x => x.TemplateId == moduleSettings.TemplateId);
                    campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);

                    _dominatorScheduler.StopActivity(account, _activityType.ToString(), moduleSettings.TemplateId, false);

                    _jobActivityConfigurationManager.Delete(account.AccountId, _activityType);
                });

                _accountsCacheService.UpsertAccounts(accountDetails.ToArray());
                _footerControl.list_SelectedAccounts = remainingAccount;
                warningWindow.Close();
            };

            #endregion

            #region Warning windows cancel button event

            objErrorModelControl.BtnCancel.Click += (senders, events) =>
            {
                needToCancel = true;
                warningWindow.Close();

            };

            #endregion

            if (objErrorModelControl.Accounts.Count != 0)
                warningWindow.ShowDialog();

            SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
            if (needToCancel || _footerControl.list_SelectedAccounts.Count == 0)
            {
                return false;
            }
            return true;
        }
        public void SaveTemplateToAccounts(string templateId)
        {
            List<RunningTimes> runningTime = Model.JobConfiguration.RunningTime;
            var accountDetails = _accountsFileManager.GetAllAccounts(_footerControl.list_SelectedAccounts, SocialNetwork);

            accountDetails.ForEach(account =>
            {
                AddTemplateToAccount(templateId, account, runningTime);

                //AccountsFileManager.Edit(account);
            });
            var accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
            accountsCacheService.UpsertAccounts(accountDetails.ToArray());

        }

        private void AddTemplateToAccount(string templateId, DominatorAccountModel account, List<RunningTimes> runningTime)
        {
            var moduleConfiguration =
                _jobActivityConfigurationManager[account.AccountBaseModel.AccountId, _activityType] ??
                new ModuleConfiguration { ActivityType = _activityType };

            moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
            moduleConfiguration.IsEnabled = true;
            moduleConfiguration.Status = "Active";
            moduleConfiguration.TemplateId = templateId;
            moduleConfiguration.IsTemplateMadeByCampaignMode = true;
            moduleConfiguration.DelayBetweenJobs = Model.JobConfiguration.DelayBetweenJobs;
            runningTime.ForEach(x =>
            {
                foreach (var timingRange in x.Timings)
                {
                    timingRange.Module = _activityType.ToString();
                }
            });
            moduleConfiguration.LstRunningTimes = new List<RunningTimes>(runningTime);

            moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration, 0);
            _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, moduleConfiguration.ActivityType, moduleConfiguration);
        }

        #endregion

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

                var objSelectAccountControl = new SelectAccountControl(_footerControl.list_SelectedAccounts);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl, "Select Account");

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    var selectedAccount = objSelectAccountControl.GetSelectedAccount();
                    if (selectedAccount.Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
                        GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork, CampaignName, _footerControl.list_SelectedAccounts.Count, CampaignName);
                    }
                    else
                    {
                        SelectedAccountCount = ConstantVariable.NoAccountSelected;
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

        public void FooterControl_OnSelectAccountChanged(List<string> listOfSelectedAccounts)
        {
            if (listOfSelectedAccounts.Count > 0)
            {
                _footerControl.list_SelectedAccounts = listOfSelectedAccounts;
                SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
                GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork, "", _footerControl.list_SelectedAccounts.Count, _activityType);
            }
            else
            {
                SelectedAccountCount = ConstantVariable.NoAccountSelected;
                _footerControl.list_SelectedAccounts = listOfSelectedAccounts;
            }
        }


        #endregion

        #region Update campaign

        protected void UpdateCampaign()
        {

            if (!ValidateCampaign())
                return;

            if (!ValidateExtraProperty())
                return;

            var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
            var currentCampaign = campaignFileManager.FirstOrDefault(camp => camp.TemplateId == TemplateId);

            try
            {
                var newlyAddedAccounts = _footerControl.list_SelectedAccounts.Except(currentCampaign?.SelectedAccountList).ToList();
                var existingAccounts = _footerControl.list_SelectedAccounts.Except(newlyAddedAccounts).ToList();
                var removedAccounts = currentCampaign?.SelectedAccountList.Except(existingAccounts).ToList();
                if (newlyAddedAccounts.Count() != 0)
                {
                    if (!UpdateNewlyAddedAccounts(newlyAddedAccounts)) return;
                }

                var accountToRemoveModuleConfiguration = _accountsFileManager.GetAllAccounts(removedAccounts, SocialNetwork);

                #region Remove TemplateId from removed account from campaign selected account list

                accountToRemoveModuleConfiguration.ForEach(account =>
                {

                    try
                    {

                        var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];
                        _jobActivityConfigurationManager.Delete(account.AccountId, _activityType);
                        _dominatorScheduler.StopActivity(account, _activityType.ToString(),
                            moduleSettings?.TemplateId, true);
                        _jobActivityConfigurationManager.Delete(account.AccountId, _activityType);

                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }


                });

                _accountsCacheService.UpsertAccounts(accountToRemoveModuleConfiguration.ToArray());

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }



            // Update Template Details
            var TemplatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            TemplatesFileManager.ApplyFunc(template =>
            {
                if (template.Id != TemplateId)
                    return false;

                TModel model = JsonConvert.DeserializeObject<TModel>(template.ActivitySettings);
                var firstRunningTime = ((dynamic)model).JobConfiguration.RunningTime;
                var secondRunningTime = Model.JobConfiguration.RunningTime;

                var result = _dominatorScheduler.CompareRunningTime(firstRunningTime, secondRunningTime);
                if (!result)
                {
                    try
                    {
                        var lstAccountDetails = _accountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork);

                        foreach (var accountModel in lstAccountDetails.Where(x =>
                            _footerControl.list_SelectedAccounts.Contains(x.AccountBaseModel.UserName)))
                        {
                            var moduleSettings = _jobActivityConfigurationManager[accountModel.AccountId, _activityType];
                            moduleSettings.LstRunningTimes = secondRunningTime;
                            _dominatorScheduler.StopActivity(accountModel, _activityType.ToString(), TemplateId, true);
                        }

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
            campaignFileManager.ForEach(campaign =>
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

                    #region Updating Campaign UI

                    var LstCampaignDetails = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel.LstCampaignDetails;

                    var oldCampaignindex = LstCampaignDetails.IndexOf(LstCampaignDetails.FirstOrDefault(x => x.CampaignId == campaign.CampaignId));

                    if (oldCampaignindex >= 0)
                    {
                        LstCampaignDetails[oldCampaignindex].CampaignName = CampaignName;
                        LstCampaignDetails[oldCampaignindex].SelectedAccountList = _footerControl.list_SelectedAccounts;
                        LstCampaignDetails[oldCampaignindex].TemplateId = TemplateId;
                        LstCampaignDetails[oldCampaignindex].CreationDate = campaign.CreationDate;
                        LstCampaignDetails[oldCampaignindex].Status = "Active";
                        LstCampaignDetails[oldCampaignindex].LastEditedDate = campaign.LastEditedDate;
                    }

                    #endregion

                }
            });


            // Update Account Detail
            var accountDetails = _accountsFileManager.GetAll(SocialNetwork);
            if (UpdateSelectedAccountDetails(accountDetails, _footerControl.list_SelectedAccounts, Model.JobConfiguration))
                ToasterNotification.ShowSuccess($"Campaign:- {CampaignName } updated successfully");


            SetDataContext();
            TabSwitcher.GoToCampaign();

        }

        private string GetWarningLangRsrc()
        {
            try
            {
                return Application.Current.FindResource("LangKeyStartWarning") + $" {_activityType }. " +
                                       Application.Current.FindResource("LangKeyEndWarning");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return $"These accounts are already having configuration settings for activity: {_activityType} . Saving this settings will override previous settings and remove this account from the campaign.";
            }
        }

        private bool UpdateNewlyAddedAccounts(List<string> newlyAddedAccounts)
        {
            bool isProcessSuccessful = false;
            bool needToCancel = false;
            #region Get the accounts which holds template Id

            var accountDetails = _accountsFileManager.GetAllAccounts(newlyAddedAccounts, SocialNetwork);

            var accountHavingTemplates = accountDetails.Where(x => _jobActivityConfigurationManager[x.AccountId, _activityType]?.TemplateId != null).ToList();

            #endregion

            #region If account having any template then Show ErrorModelControl with warning

            if (accountHavingTemplates.Count != 0)
            {
                var objErrorModelControl = new ErrorModelControl { WarningText = GetWarningLangRsrc() };

                #region Adding the accounts to Warning window

                accountHavingTemplates.ForEach(account =>
                {
                    var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];

                    if (moduleSettings != null)
                    {
                        objErrorModelControl.Accounts.Add(new AccountDetails
                        {
                            UserName = account.AccountBaseModel.UserName
                        });
                    }
                });

                var warningWindow = new Dialog().GetMetroWindowWithOutClose(objErrorModelControl, "Warning");

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
                        isProcessSuccessful = true;
                        warningWindow.Close();
                        return;
                    }

                    #region Update already existing account setting

                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    accountDetails.ForEach(account =>
                    {
                        if (!selectedAccount.Contains(account.AccountBaseModel.UserName))
                            return;

                        var moduleSettings = _jobActivityConfigurationManager[account.AccountId, _activityType];

                        if (moduleSettings == null)
                            return;
                        campaignFileManager.DeleteSelectedAccount(moduleSettings.TemplateId,
                            account.AccountBaseModel.UserName);

                        var campToUpdate = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel.LstCampaignDetails.FirstOrDefault(x => x.TemplateId == moduleSettings.TemplateId);
                        campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);

                        moduleSettings.TemplateId = TemplateId;
                        moduleSettings.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleSettings);
                        _dominatorScheduler.StopActivity(account, _activityType.ToString(),
                            moduleSettings.TemplateId, true);
                        _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, moduleSettings);
                    });

                    _accountsCacheService.UpsertAccounts(accountDetails.ToArray());
                    isProcessSuccessful = true;
                    #endregion
                    warningWindow.Close();
                };

                #endregion

                #region Warning windows cancel button event

                objErrorModelControl.BtnCancel.Click += (senders, events) =>
                {
                    needToCancel = true;
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
                    AddTemplateToAccount(TemplateId, account, Model.JobConfiguration.RunningTime);
                });
                isProcessSuccessful = true;

                #endregion
            }
            #endregion
            SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
            if (_footerControl.list_SelectedAccounts.Count == 0 || needToCancel) return false;
            return isProcessSuccessful;
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

                    var moduleConfiguration =
                        _jobActivityConfigurationManager[account.AccountId, _activityType] ??
                        new ModuleConfiguration { ActivityType = _activityType };
                    _dominatorScheduler.StopActivity(account, _activityType.ToString(), moduleConfiguration.TemplateId, true);
                    _jobActivityConfigurationManager.AddOrUpdate(account.AccountId, _activityType, moduleConfiguration);
                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();

                    moduleConfiguration.IsEnabled = true;

                    moduleConfiguration.Status = "Active";

                    moduleConfiguration.TemplateId = TemplateId;
                    moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration);

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
                    moduleConfiguration.IsTemplateMadeByCampaignMode = true;

                    selectedAccounts.Add(account);
                    _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, moduleConfiguration);

                    // AccountsFileManager.Edit(account);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            _accountsCacheService.UpsertAccounts(allAccountDetails.ToArray());
            // save all accounts and schedule actitvities of selected accounts            
            foreach (var account in selectedAccounts)
            {
                _dominatorScheduler.ScheduleNextActivity(account, _activityType);
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

                    var moduleConfiguration = _jobActivityConfigurationManager[account.AccountId, _activityType] ?? new ModuleConfiguration() { ActivityType = _activityType };
                    _jobActivityConfigurationManager.AddOrUpdate(account.AccountId, _activityType, moduleConfiguration);
                    moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
                    moduleConfiguration.IsEnabled = true;
                    moduleConfiguration.Status = "Active";
                    moduleConfiguration.IsTemplateMadeByCampaignMode = true;
                    // Update running times for current activity
                    UpdateRunningTime(jobConfiguration, account);

                    selectedAccounts.Add(account);
                    _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, moduleConfiguration);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            _accountsCacheService.UpsertAccounts(allAccountDetails.ToArray());

            // save all accounts and schedule actitvities of selected accounts            
            foreach (var account in selectedAccounts)
            {
                _dominatorScheduler.ScheduleNextActivity(account, _activityType);
                //DominatorScheduler.ScheduleTodayJobs(account, account.AccountBaseModel.AccountNetwork, _activityType);
                //DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: account, network: account.AccountBaseModel.AccountNetwork);
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
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue) && _queryControl.QueryCollection.Count != 0)
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
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork, CampaignName, _activityType, "{ " + string.Join(" },{ ", queryValuIndex.ToArray()) + " }");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount, SocinatorInitialize.ActiveSocialNetwork, CampaignName, _activityType, queryValuIndex.Count);

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

        #region Save Configuration Implementation

        #region Toggle Module Status changes 

        protected bool ChangeAccountsModuleStatus(bool isStart, string selectedAccount, SocialNetworks socialNetworks)
        {

            try
            {
                var accountModel = _accountsFileManager.GetAccount(selectedAccount, socialNetworks);
                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[accountModel.AccountId, _activityType];
                if (moduleConfiguration?.TemplateId == null || moduleConfiguration.LstRunningTimes == null)
                {
                    Model.IsAccountGrowthActive = !isStart;

                    if (isStart)
                    {
                        Dialog.ShowDialog("Error", "Please save your settings, before starting the activity.");
                        return false;
                    }
                }
                moduleConfiguration.IsEnabled = isStart;
                try
                {
                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    var campaignStatus = campaignFileManager
                        .FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId)
                        ?.Status;
                    if (campaignStatus == "Paused" && moduleConfiguration.IsEnabled)
                    {
                        DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "This account belongs to campaign configuration, which is paused state. Please make the campaign active before changing activity status for this account.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountBaseModel.AccountId, _activityType, moduleConfiguration);
                _accountsCacheService.UpsertAccounts(accountModel);
                if (!moduleConfiguration.IsEnabled)
                {
                    _dominatorScheduler.StopActivity(accountModel, _activityType.ToString(),
                        moduleConfiguration.TemplateId, moduleConfiguration.IsEnabled);
                    ToasterNotification.ShowSuccess("Successfully stopped");
                }
                else
                {
                    ToasterNotification.ShowSuccess("Successfully activated");
                    _dominatorScheduler.ScheduleNextActivity(accountModel, _activityType);
                }
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

                var accountDetails = _accountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, network);

                SocinatorInitialize.GetSocialLibrary(network)
                     .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount = _accountGrowthModeHeader.SelectedItem;

                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[accountDetails.AccountId, _activityType];
                if (moduleConfiguration != null)
                {
                    var TemplatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }
                else
                    SetModuleValues(false, null);

                _mainGrid.DataContext = ObjViewModel;
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

        public void SetSelectedAccounts(SocialNetworks networks)
        {
            var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.UserName));

            _accountGrowthModeHeader.AccountItemSource = accounts;

            _accountGrowthModeHeader.SelectedItem = SocinatorInitialize.GetSocialLibrary(networks)
                .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;

        }

        #endregion

        public void SaveConfigurations()
        {
            //Todo
            //if (!ValidateQuery()) return;

            if (!ValidateExtraProperty()) return;

            if (!ValidateRunningTime()) return;

            var accountModel = _accountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, SocialNetwork);
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[accountModel.AccountId, _activityType];
            if (moduleConfiguration?.TemplateId != null)
            {
                if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                                "This account is already running with another campaign,saving this settings will override previous settings and remove this account from that campaign.",
                                MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Yes", "No"));
                    if (dialogResult == MessageDialogResult.Negative)
                        return;
                }
                #region Template Id present case

                // need to check whether its running or not, if its running then need to stop process
                _dominatorScheduler.StopActivity(accountModel, _activityType.ToString(),
                    moduleConfiguration.TemplateId, moduleConfiguration.IsEnabled);

                // update the template
                UpdateTemplate(accountModel, moduleConfiguration.TemplateId,
                    moduleConfiguration.IsTemplateMadeByCampaignMode);

                // update the running time
                UpdateRunningTime(Model.JobConfiguration, accountModel);
                //_dominatorScheduler.ScheduleNextActivity(accountModel, _activityType);
                ToasterNotification.ShowSuccess("Successfully Saved !!!");
                #endregion
            }
            else
            {
                #region Template not present case
                moduleConfiguration = new ModuleConfiguration { ActivityType = _activityType };
                jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountId, _activityType, moduleConfiguration);

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

                moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration);
                _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountBaseModel.AccountId, _activityType, moduleConfiguration);
                _accountsCacheService.UpsertAccounts(accountModel);
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!");
                #endregion

            }
        }

        private static void AddNewTemplate<T>(T moduleToSave, string userName, ActivityType moduleType, DominatorAccountModel account) where T : class
        {
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var accountModuleConfiguration = jobActivityConfigurationManager[account.AccountId, moduleType];
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
                    var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                    campaignFileManager.DeleteSelectedAccount(accountstemplateId, _accountGrowthModeHeader.SelectedItem);

                    var campToUpdate = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel.LstCampaignDetails.FirstOrDefault(x => x.TemplateId == accountstemplateId);
                    campToUpdate?.SelectedAccountList.Remove(accountModel.AccountBaseModel.UserName);

                    AddNewTemplate((TModel)Model, _accountGrowthModeHeader.SelectedItem, _activityType, accountModel);
                }
                else
                {
                    var TemplatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
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

                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var accountModuleSettings = jobActivityConfigurationManager[account.AccountId, _activityType];
                if (accountModuleSettings != null)
                {
                    if (accountModuleSettings.LstRunningTimes == null)
                        accountModuleSettings.LstRunningTimes = new List<RunningTimes>();

                    accountModuleSettings.LstRunningTimes = jobConfiguration.RunningTime;
                }
                accountModuleSettings.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(accountModuleSettings);
                _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId, _activityType, accountModuleSettings);
                _accountsCacheService.UpsertAccounts(account);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        #endregion

        public virtual void SelectAccount()
        {
            try
            {
                var objSelectAccountControl = new SelectAccountControl(_footerControl.list_SelectedAccounts);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl, "Select Account");

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    var selectedAccount = objSelectAccountControl.GetSelectedAccount();
                    if (selectedAccount.Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
                        GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork, CampaignName, _footerControl.list_SelectedAccounts.Count, CampaignName);
                    }
                    else
                    {
                        SelectedAccountCount = ConstantVariable.NoAccountSelected;
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

        public void AddQuery(Type queryParameterType)
        {
            try
            {
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue.Trim()) && _queryControl.QueryCollection.Count == 0)
                    return;

                if (_queryControl.CurrentQuery.QueryValue.Contains(","))
                {
                    _queryControl.QueryCollection.Clear();
                    _queryControl.QueryCollection.AddRange(_queryControl.CurrentQuery.QueryValue.Split(',').Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim()).Distinct());
                    _queryControl.CurrentQuery.QueryValue = String.Empty;
                }

                List<int> queryValuIndex = new List<int>();
                if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue) && _queryControl.QueryCollection.Count != 0)
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
                        currentQuery.Index = Model.SavedQueries.IndexOf(currentQuery) + 1;
                    });
                    if (queryValuIndex.Count > 0)
                    {
                        if (queryValuIndex.Count <= 10)
                        {
                            GlobusLogHelper.log.Info(Log.AlreadyExistQuery, SocinatorInitialize.ActiveSocialNetwork, CampaignName, _activityType, "{ " + string.Join(" },{ ", queryValuIndex.ToArray()) + " }");
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.AlreadyExistQueryCount, SocinatorInitialize.ActiveSocialNetwork, CampaignName, _activityType, queryValuIndex.Count);

                        }
                    }
                }
                else
                {
                    _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryType;

                    // _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryTypeAsDisplayName(queryParameterType);

                    var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = currentQuery.QueryValue.Trim();

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    if (IsQueryExist(currentQuery, Model.SavedQueries)) return;

                    Model.SavedQueries.Add(currentQuery);
                    currentQuery.Index = Model.SavedQueries.IndexOf(currentQuery) + 1;
                    _queryControl.CurrentQuery.QueryValue = string.Empty;

                }
                _queryControl.IsEnabled = true;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void CustomFilter()
        {
            try
            {
                UserFilterAction.UserFilterControl(_queryControl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void SetSelectedAccounts()
        {
            try
            {
                var networks = SocinatorInitialize.AccountModeActiveSocialNetwork;
                var accounts = new ObservableCollectionBase<string>(_accountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == networks && x.AccountBaseModel.Status == AccountStatus.Success).Select(x => x.UserName));

                _accountGrowthModeHeader.AccountItemSource = accounts;

                _accountGrowthModeHeader.SelectedItem = SocinatorInitialize.GetSocialLibrary(networks)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                SetAccountModeDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void SetAccountModeDataContext()
        {
            try
            {
                var network = SocinatorInitialize.AccountModeActiveSocialNetwork;
                SocialNetwork = network;

                var accountDetails = _accountsFileManager.GetAccount(_accountGrowthModeHeader.SelectedItem, network);

                SocinatorInitialize.GetSocialLibrary(network)
                     .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount = _accountGrowthModeHeader.SelectedItem;


                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[accountDetails.AccountId, _activityType];

                if (moduleConfiguration != null)
                {
                    var TemplatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
                    var templateDetails = TemplatesFileManager.GetTemplateById(moduleConfiguration.TemplateId);
                    SetModuleValues(moduleConfiguration.IsEnabled, templateDetails);
                }
                else
                    SetModuleValues(false, null);

                _mainGrid.DataContext = ObjViewModel;
                _accountGrowthModeHeader.DataContext = this;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountModeStatusChange()
        {
            try
            {
                var network = SocinatorInitialize.AccountModeActiveSocialNetwork;
                if (!ChangeAccountsModuleStatus(Model.IsAccountGrowthActive, _accountGrowthModeHeader.SelectedItem, network))
                    Model.IsAccountGrowthActive = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region ToolTip commands
        public static void StartModuleOverView(object sender)
        {
            PopUpStarter.istutorial = false;
            var tour = new ScreenInfo
            {
                Name = "Overview",
                ShowNextButtonDefault = false,
                Steps = new[]
               {
                    new Step("ModuleButtonOverView", "Welcome - Start your Tutorial", (TViewModel)Instances)
                    {
                        ContentDataTemplateKey = "SelectSearchQueryTemplate"
                    },
                }
            };
            tour.Start();
        }

        public static void StartSelectQueryIntro(object sender)
        {
            var tour = new ScreenInfo
            {
                Name = "Introduction",
                ShowNextButtonDefault = true,
                Steps = new[]
                {
                    new Step("SearchQuery", "SearchQuery", "Login with Accounts "),
                     new Step("JobConfiguration", "JobConfiguration", "Shows you growth rate of accounts"),
                      new Step("AfterFollow", "AfterFollow", "Shows you Info regarding dashboard"),
                       new Step("OtherConfiguration", "AccountsActivityInfo", "Shows you Info regarding dashboard"),
                        new Step("BlacklistUsers", "BlacklistUsers", "Shows you Info regarding dashboard"),

                }
            };
            tour.Start();
        }
        #endregion

    }
}
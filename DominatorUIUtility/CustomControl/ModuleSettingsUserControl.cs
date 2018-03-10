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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        SocialNetworks _socialNetwork => DominatorHouseInitializer.ActiveSocialNetwork;

        bool _initialized = false;

        protected ModuleSettingsUserControl()
        {
        }

        public void InitializeBaseClass( 
            Grid MainGrid,
            ActivityType activityType,
            string moduleName,
            HeaderControl header = null,
            FooterControl footer = null,
            SearchQueryControl queryControl =null,
            AccountGrowthModeHeader accountGrowthModeHeader=null)
        {
            if (queryControl == null) throw new ArgumentNullException(nameof(queryControl));
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
        
        // IHeaderControl
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
            if (string.IsNullOrEmpty(_queryControl.CurrentQuery.QueryValue))
            {
                _queryControl.QueryCollection.ForEach(query =>
                {
                    var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                    if (currentQuery == null) return;

                    currentQuery.QueryValue = query;

                    currentQuery.QueryTypeDisplayName = currentQuery.QueryTypeAsDisplayName();

                    currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                    Model.SavedQueries.Add(currentQuery);

                });
            }
            else
            {                
                _queryControl.CurrentQuery.QueryTypeDisplayName = _queryControl.CurrentQuery.QueryTypeAsDisplayName();

                var currentQuery = _queryControl.CurrentQuery.Clone() as QueryInfo;

                if (currentQuery == null) return;

                currentQuery.QueryPriority = Model.SavedQueries.Count + 1;

                Model.SavedQueries.Add(currentQuery);

                _queryControl.CurrentQuery = new QueryInfo();

            }
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

        public abstract void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType);
        public abstract void AddNewCampaign(List<string> lstSelectedAccounts, ActivityType moduleType);
     
        protected virtual bool ValidateCampaign()
        {
            if (_footerControl.list_SelectedAccounts.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please select at least one account.",
                    MessageDialogStyle.Affirmative);
                return false;
            }
           

            // Check timings
            if (((IEnumerable<RunningTimes>)Model.JobConfiguration.RunningTime).All(rt => rt.Timings.Count == 0))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one time range when to run and stop the activity.",
                    MessageDialogStyle.Affirmative);
                return false; 
            }

            return true;
        }


        /// <summary>
        /// Event handler called when user Creates or Updates campaign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FooterControl_OnCreateCampaignChanged(object sender, RoutedEventArgs e)
        {
            if (!ValidateCampaign())
                return;
                    
            var objTemplateModel = new TemplateModel();
            
            // TODO: implement saving and add campaign
            if(false) CampaignGlobalRoutines.Instance.Create((TModel)Model, _activityType, CampaignName, _footerControl.list_SelectedAccounts);

            TemplateId = objTemplateModel.SaveTemplate((TModel)Model,
                _activityType.ToString(), _socialNetwork,
                CampaignName);

            SaveDetails(_footerControl.list_SelectedAccounts, _activityType);

            AddNewCampaign(_footerControl.list_SelectedAccounts, _activityType);

            SetDataContext();
            TabSwitcher.ChangeTabIndex?.Invoke(6, 0);
        }

        protected void AccountGrowthHeader_OnSaveClick(object sender, RoutedEventArgs e)
        {                       
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

            DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success", "Successfully Saved !!!", MessageDialogStyle.Affirmative);

        }

        private static void AddNewTemplate<T>(T moduleToSave, string userName, ActivityType moduleType, DominatorAccountModel account) where T : class
        {
            var objTemplateModel = new TemplateModel();
            var first = account.ActivityManager?.LstModuleConfiguration.FirstOrDefault
                (x => x.ActivityType == moduleType);

            if (first != null)
                first.TemplateId =
                    objTemplateModel.SaveTemplate(moduleToSave,
                        moduleType.ToString(), SocialNetworks.Instagram,
                        userName + "_" + moduleType + "_Template");
        }


        // Update Campaign        
        protected void FooterControl_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
        {
            if (!ValidateCampaign())
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
                        foreach (var acc in _footerControl.list_SelectedAccounts)
                            DominatorScheduler.StopActivity(acc, Module, TemplateId);
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
            DominatorHouseCore.Utility.TabSwitcher.ChangeTabIndex(6, 0);
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
                    filterForActiveSocialNetwrok: true);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl, "Select Account");

                objSelectAccountControl.btnSave.Click += (senders, Events) =>
                {
                    if (objSelectAccountControl.GetSelectedAccount().Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        this.SelectedAccountCount = _footerControl.list_SelectedAccounts.Count + " Account Selected";
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
            IsEditCampaignName = true;
            CancelEditVisibility = Visibility.Collapsed;
            _footerControl.CampaignManager = ConstantVariable.CreateCampaign;
            SetDataContext();
        }

    
        protected virtual void SetDataContext()
        {
            this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
            ObjViewModel = new TViewModel();
            _footerControl.list_SelectedAccounts = new List<string>();

            _mainGrid.DataContext = Model as TModel;

            if (_headerControl != null && _footerControl != null)
                _headerControl.DataContext = _footerControl.DataContext = this;

            if (_accountGrowthModeHeader != null)
                _accountGrowthModeHeader.DataContext = this;

            CampaignName = $"{_socialNetwork} {_activityType.ToString()} [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
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

                    // Update running times for current activity
                    jobConfiguration.RunningTime.ForEach(x =>
                            {
                                foreach (var timingRange in x.Timings)
                                    timingRange.Module = _activityType.ToString();
                            });

                    account.ActivityManager.RunningTime = jobConfiguration.RunningTime;


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
                DominatorScheduler.ScheduleTodayJobs(account, SocialNetworks.Instagram, _activityType);
                DominatorScheduler.ScheduleForEachModule(moduleToIgnore: _activityType, account: account, network: SocialNetworks.Instagram);
            }

            return isAccountDetailsUpdated;
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
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace Socinator.Social.AutoActivity.Views
{
    /// <summary>
    /// Interaction logic for SocialAutoActivity.xaml
    /// </summary>
    public partial class SocialAutoActivity : UserControl
    {
        private DominatorAutoActivityViewModel DominatorAutoActivityViewModel { get; set; }

        private SocialAutoActivity()
        {
            DialogParticipation.SetRegister(this, this);
            DominatorAutoActivityViewModel = DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();
            InitializeComponent();
        }

        private static SocialAutoActivity ObjSocialAutoActivity { get; set; } = null;

        public static SocialAutoActivity GetSingletonSocialAutoActivity()
        {
            if (ObjSocialAutoActivity == null)
                ObjSocialAutoActivity = new SocialAutoActivity();
            ObjSocialAutoActivity.SetDataContext();
            return ObjSocialAutoActivity;
        }

        public bool NewAutoActivityObject(SocialNetworks soicalNetworks, string selectedAccounts)
        {
            try
            {
                ObjSocialAutoActivity = new SocialAutoActivity();
                ObjSocialAutoActivity.SetDataContext();

                ObjSocialAutoActivity.DominatorAutoActivityViewModel.CallRespectiveView(soicalNetworks);

                SocinatorInitialize.GetSocialLibrary(soicalNetworks)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount = selectedAccounts;
             
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        private void SetDataContext()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                    {
                        SocialActivity.DataContext = DominatorAutoActivityViewModel;
                    });
            }
            else
            {
                SocialActivity.DataContext = DominatorAutoActivityViewModel;
            }
        }

        private void GotoTools(object sender)
        {
            var accountsActivityDetailModel =
                ((FrameworkElement)sender).DataContext as AccountsActivityDetailModel;

            if (accountsActivityDetailModel == null)
                return;

            SocinatorInitialize.GetSocialLibrary(accountsActivityDetailModel.AccountNetwork).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount =
                accountsActivityDetailModel.AccountName;

            DominatorAutoActivityViewModel =
                DominatorAutoActivityViewModel.GetSingletonDominatorAutoActivityViewModel();

            DominatorAutoActivityViewModel.CallRespectiveView(accountsActivityDetailModel.AccountNetwork);
        }

        private void SocialAutoActivity_OnLoaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                DominatorAutoActivityViewModel.InitializeAccounts();
                SetDataContext();
            });
        }

        private void GotoToolsByName_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
            => GotoTools(sender);

        private void ButtonViewActivityStatus_OnClick(object sender, RoutedEventArgs e)
            => GotoTools(sender);


        private void ActivityStatusChanged_OnClick(object sender, RoutedEventArgs e)
        {
            var currentDataContext = ((FrameworkElement)sender).DataContext as ActivityDetailsModel;

            try
            {
                var currentAccountActivity = AccountsFileManager.GetAccountById(currentDataContext.AccountId).ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == currentDataContext.Title);
                var account = AccountsFileManager.GetAccountById(currentDataContext.AccountId);
                var campaignStatus = CampaignsFileManager.Get()
                    .FirstOrDefault(x => x.TemplateId == currentAccountActivity.TemplateId).Status;
                if (campaignStatus == "Paused" && currentDataContext.Status)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", $"This account belongs to campaign configuration, which is paused state. Please make the campaign active before changing activity status for this account.");
                    currentDataContext.Status = false;
                    return;
                }
                if (currentDataContext == null)
                    return;
            }
            catch (Exception ex)
            {

               
            }
            var accountDetails = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social).DominatorAccountViewModel
                .LstDominatorAccountModel
                .FirstOrDefault(x => x.AccountBaseModel.AccountId == currentDataContext.AccountId);

           // accountDetails?.NotifyCancelled();

            var status = DominatorScheduler.ChangeAccountsRunningStatus(currentDataContext.Status, currentDataContext.AccountId,
                currentDataContext.Title);

            if (!status)
            {
                try
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", $"Please configure your {currentDataContext.Title} settings, before starting the activity. Make sure you have added enough queries and have clicked on SAVE button");
                    currentDataContext.Status = false;
                }
                catch (Exception ex)
                {

                    GlobusLogHelper.log.Error(ex.Message + ex.StackTrace);
                }
            }               
        }
    }
}

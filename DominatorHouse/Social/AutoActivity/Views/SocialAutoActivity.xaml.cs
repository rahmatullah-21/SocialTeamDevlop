using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
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

                switch (soicalNetworks)
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

            if (currentDataContext == null)
                return;

            var accountDetails = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social).DominatorAccountViewModel
                .LstDominatorAccountModel
                .FirstOrDefault(x => x.AccountBaseModel.AccountId == currentDataContext.AccountId);

            accountDetails?.NotifyCancelled();

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

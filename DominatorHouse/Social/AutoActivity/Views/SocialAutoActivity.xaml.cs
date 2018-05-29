using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;

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

            if (currentDataContext == null)
                return;

            var status = DominatorScheduler.ChangeAccountsRunningStatus(currentDataContext.Status, currentDataContext.AccountId,
                currentDataContext.Title);

            if (!status)
            {
                GlobusLogHelper.log.Info($"{currentDataContext.Title} doesn't register with any template before with particular account!");
                currentDataContext.Status = false;
            }               
        }
    }
}

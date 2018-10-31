using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace Socinator.Social.AutoActivity.Views
{
    /// <summary>
    /// Interaction logic for SocialAutoActivity.xaml
    /// </summary>
    public partial class SocialAutoActivity : UserControl
    {
        private IDominatorAutoActivityViewModel DominatorAutoActivityViewModel { get; set; }

        public SocialAutoActivity(IDominatorAutoActivityViewModel dominatorAutoActivityViewModel)
        {
            DialogParticipation.SetRegister(this, this);
            DominatorAutoActivityViewModel = dominatorAutoActivityViewModel;
            InitializeComponent();
        }

        private static SocialAutoActivity ObjSocialAutoActivity { get; set; } = null;

        public static SocialAutoActivity GetSingletonSocialAutoActivity()
        {
            if (ObjSocialAutoActivity == null)
                ObjSocialAutoActivity = IoC.Container.Resolve<SocialAutoActivity>();
            ObjSocialAutoActivity.SetDataContext();
            return ObjSocialAutoActivity;
        }

        public bool NewAutoActivityObject(SocialNetworks soicalNetworks, string selectedAccounts)
        {
            try
            {
                ObjSocialAutoActivity = IoC.Container.Resolve<SocialAutoActivity>();
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

        private void SocialAutoActivity_OnLoaded(object sender, RoutedEventArgs e)
        {
            ThreadFactory.Instance.Start(() =>
            {
                DominatorAutoActivityViewModel.InitializeAccounts();
                SetDataContext();
            });
        }
    }
}

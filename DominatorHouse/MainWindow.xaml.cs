#region Namespaces
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Navigations;
using DominatorUIUtility.ScreenTip.ViewModel;
using EmbeddedBrowser;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorUIUtility.ScreenTip.PopUpstyle;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.Models;

#endregion

namespace Socinator
{
    public interface IMainWindow
    {

    }
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IMainWindow
    {
        private bool IsClickedFromMainWindow { get; set; } = true;
        public object PropertyChanged { get; private set; }

        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);

                InitializeComponent();

                SocinatorInitialize.LogInitializer(this);
                FeatureTour.SetViewModelFactoryMethod(tourRun => new CustomTourViewModel(tourRun));
                var navigator = FeatureTour.GetNavigator();

                var mainViewModel = ServiceLocator.Current.GetInstance<IMainViewModel>();
                SocinatorWindow.DataContext = mainViewModel;
                Loaded += (o, e) =>
                {
                    GlobusLogHelper.log.Info($"Welcome to {ConstantVariable.ApplicationName}!");
                };

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
     
        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.CheckStatus(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountUpdate(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.UpdateDetails(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var browserWindow = new BrowserWindow(dominatorAccountModel);
                browserWindow.Show();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

               private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                TextBlock ovjtext = (TextBlock)sender;
                PopUpStarter.StartIntroduction(null);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
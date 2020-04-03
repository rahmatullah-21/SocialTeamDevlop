#region Namespaces
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Socinator.Social.Settings.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

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
        IMainViewModel mainViewModel;
        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);
                Application.Current.MainWindow = this;
                InitializeComponent();

                SocinatorInitialize.LogInitializer(this);

                var interopHelper = new WindowInteropHelper(Application.Current.MainWindow);
                var activeScreen = System.Windows.Forms.Screen.FromHandle(interopHelper.Handle);

                mainViewModel = ServiceLocator.Current.GetInstance<IMainViewModel>();

                mainViewModel.ScreenResolution = new KeyValuePair<int, int>
                    (activeScreen.WorkingArea.Width, activeScreen.WorkingArea.Height);

                SocinatorWindow.DataContext = mainViewModel;
                Loaded += (o, e) =>
                {
                    GlobusLogHelper.log.Info(String.Format("LangKeyWelcomeToApplication".FromResourceDictionary(), ConstantVariable.ApplicationName));
                };
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void InitialTabablzControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            statusbar.IsEnabled = false;
            if (IsClickedFromMainWindow)
            {
                var dialog = new Dialog();
                
                var activityLogWindow = dialog.GetMetroWindow(Logger, "LangKeyActivityLog".FromResourceDictionary());

                IsClickedFromMainWindow = false;
                activityLogWindow.Closing += (senders, events) =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                activityLogWindow.Content = null;
                                Grid.SetRow(Logger, 2);
                                MainGrid.Children.Add(Logger);

                                Logger.Children.Remove(RootLayout);
                                Logger.Children.Add(RootLayout);
                                MainGrid.RowDefinitions[2].Height = new GridLength(200);
                                IsClickedFromMainWindow = true;
                                statusbar.IsEnabled = true;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        });
                    });
                };

                MainGrid.RowDefinitions[2].Height = new GridLength(0);
                MainGrid.Children.Remove(Logger);
                activityLogWindow.Show();

            }
        }

      
    }
}
#region Namespaces
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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


        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);

                InitializeComponent();
                var constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();
                SocinatorInitialize.LogInitializer(this);

                var mainViewModel = ServiceLocator.Current.GetInstance<IMainViewModel>();
                SocinatorWindow.DataContext = mainViewModel;
                Loaded += (o, e) =>
                {
                    GlobusLogHelper.log.Info($"Welcome to {constantVariable.ApplicationName}!");
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

                var activityLogWindow = dialog.GetMetroWindow(Logger, "Activity Log");

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
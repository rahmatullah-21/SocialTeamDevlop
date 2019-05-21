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
        IMainViewModel mainViewModel;
        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);
                Application.Current.MainWindow = this;
                InitializeComponent();

                SocinatorInitialize.LogInitializer(this);

                mainViewModel = ServiceLocator.Current.GetInstance<IMainViewModel>();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainViewModel.IsPopUpOpen = false;
        }

        private void Popup_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPoint = e.GetPosition(null);
                pop.HorizontalOffset = pop.HorizontalOffset + (currentPoint.X - _initialMousePosition.X);
                pop.VerticalOffset = pop.VerticalOffset + (currentPoint.Y - _initialMousePosition.Y);
            }
        }

        Point _initialMousePosition;
        private void pop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            _initialMousePosition = e.GetPosition(null);
            element.CaptureMouse();

            e.Handled = true;
        }

    }
}
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

        private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var setThemeString = "Light\r\nDark";
                var selected = (sender as ComboBox).SelectedItem as string;

                string themeName = $"Base{selected}";
                Accent newAccent;
                AppTheme newAppTheme;
                string colorName = selected == "Light" ? "PrussianBlue" : "Teal";
                
                switch (selected)
                {
                    case "Light":
                        {
                            Application.Current.Resources["UserControlBackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)); // White
                            Application.Current.Resources["SelectedTabBorderBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 248, 255)); //Black
                            Application.Current.Resources["TextColorBrushAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(240, 248, 255)); // Pure Black
                            Application.Current.Resources["IconFillBrushAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(1,0,0)); // Black
                            Application.Current.Resources["TextColorBrushAccordingTheme1"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(35, 49, 64)); // #233140
                            Application.Current.Resources["ListItemsMouseHoverColorAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(214, 235, 242)); // LightBlue (Much Lighter)
                            Application.Current.Resources["GreenColorAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0)); // Green

                        }
                        break;

                    case "Dark":
                        { //GreenColorAccordingTheme
                            setThemeString = "Dark\r\nLight";

                            Application.Current.Resources["UserControlBackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 37, 41)); // Black
                            Application.Current.Resources["SelectedTabBorderBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 37, 41)); // Black
                            Application.Current.Resources["TextColorBrushAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)); // White
                            Application.Current.Resources["IconFillBrushAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(1, 166, 163)); // Teal
                            Application.Current.Resources["TextColorBrushAccordingTheme1"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(1, 166, 163)); // Teal
                            Application.Current.Resources["ListItemsMouseHoverColorAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(47, 79, 79)); // DarkSlateGrey
                            Application.Current.Resources["GreenColorAccordingTheme"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(144, 238, 144)); // LightGreen

                        }
                        break;
                }

                newAccent = ThemeManager.GetAccent(colorName);
                newAppTheme = ThemeManager.GetAppTheme(themeName);
                ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);

                ServiceLocator.Current.GetInstance<IBinFileHelper>().SetTheme(setThemeString);
            }
            catch (Exception ex) { }
        }
    }
}
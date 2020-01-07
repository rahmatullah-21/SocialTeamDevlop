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
                var selected = (sender as ComboBox).SelectedItem as string;
                var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
                var firstInFile = binFileHelper.ThemesList()[0];
                
                bool delFlag = false;

                switch (selected)
                {
                    case "Light":
                        {
                            List<ResourceDictionary> res = Application.Current.Resources.MergedDictionaries.ToList();
                            ResourceDictionary obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Styles/BaseDarkStyles.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Styles/BaseStyles.xaml", UriKind.RelativeOrAbsolute) });

                            obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Themes/Dark.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Themes/PrussianBlue.xaml", UriKind.RelativeOrAbsolute) });
                            
                            binFileHelper.SetTheme("Light\r\nDark");
                        }
                        break;

                    case "Dark":
                        {
                            List<ResourceDictionary> res = Application.Current.Resources.MergedDictionaries.ToList();
                            ResourceDictionary obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Styles/BaseStyles.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Styles/BaseDarkStyles.xaml", UriKind.RelativeOrAbsolute) });

                            obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Themes/PrussianBlue.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Themes/Dark.xaml", UriKind.RelativeOrAbsolute) });

                            binFileHelper.SetTheme("Dark\r\nLight");
                        }
                        break;
                }

                ChangeAppearance(selected);
            }
            catch (Exception ex) { }
        }

        public void ChangeAppearance(string themeName)
        {
            try
            {
                string ThemeName = $"Base{themeName}";
                Accent newAccent;
                AppTheme newAppTheme;
                string ColorName = themeName == "Light" ? "PrussianBlue" : "cyan";
                
                ThemeManager.AddAccent(ColorName, new Uri($"pack://application:,,,/DominatorUIUtility;component/Themes/{(themeName == "Light" ? "PrussianBlue" : "Dark")}.xaml"));
                newAccent = ThemeManager.GetAccent(ColorName);
                newAppTheme = ThemeManager.GetAppTheme(ThemeName);
                
                ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
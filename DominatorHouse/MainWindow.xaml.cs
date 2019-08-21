#region Namespaces
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
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


        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);

                InitializeComponent();

                SocinatorInitialize.LogInitializer(this);

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
        
        private void LangCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selected = (sender as ComboBox).SelectedItem as string;
                var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
                var firstInFile = binFileHelper.LanguagesList()[0];

                var dontRestart = selected == firstInFile;
                  
                if (!dontRestart && Dialog.ShowCustomDialog("LangKeyChangeLanguage".FromResourceDictionary(),
                    "LangKeyConfirmToSetLanguage".FromResourceDictionary(), "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) != MessageDialogResult.Affirmative)
                {
                    LangCombo.SelectedValue = firstInFile;
                    return;
                }
                
                bool delFlag = false;
                
                switch (selected)
                {
                    case "English":
                        {
                            List<ResourceDictionary> res = Application.Current.Resources.MergedDictionaries.ToList();
                            ResourceDictionary obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Resources/Languages/Chinese.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Resources/Languages/English.xaml", UriKind.RelativeOrAbsolute) });
                            binFileHelper.SetLanguages("English\r\nChinese");
                        }
                        break;

                    case "Chinese":
                        {
                            List<ResourceDictionary> res = Application.Current.Resources.MergedDictionaries.ToList();
                            ResourceDictionary obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Resources/Languages/English.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Resources/Languages/Chinese.xaml", UriKind.RelativeOrAbsolute) });
                            binFileHelper.SetLanguages("Chinese\r\nEnglish");
                        }
                        break;
                }
                if(!dontRestart)
                {
                    Application.Current.Shutdown();
                    Process.Start(Application.ResourceAssembly.Location);
                    Process.GetCurrentProcess().Kill();
                    Environment.Exit(0);
                }
            }
            catch (Exception ex) { }
        }
    }
}
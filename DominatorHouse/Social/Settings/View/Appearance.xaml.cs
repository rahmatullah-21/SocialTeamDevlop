using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro;
using Socinator.Social.Settings.ViewModel;

namespace DominatorHouse.Social.Settings.View
{
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance : UserControl
    {
        public Appearance()
        {
            InitializeComponent();
            MainGrid.DataContext = objAppearanceViewModel;
        }

        AppearanceViewModel objAppearanceViewModel = new AppearanceViewModel();
       
        private void lstcolor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedItem = (ColorsCollection)lstcolor.SelectedItem;
            objAppearanceViewModel.SelectedRecentColor = null;
            var AccentColor = objAppearanceViewModel.lstColorsCollection.SingleOrDefault(x => x.Value == SelectedItem.Value);
            objAppearanceViewModel.lstRecentColorsCollection.Add(AccentColor);
            objAppearanceViewModel.lstRecentColorsCollection = new ObservableCollection<ColorsCollection>(objAppearanceViewModel.lstRecentColorsCollection.Distinct());

            ChangeAppearance(sender);
        }

        private void lsttheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeAppearance(sender);           
        }

        private void lstRecentcolor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeAppearance(sender);
        }
        public void ChangeAppearance(object sender)
        {
            try
            {
                string ThemeName;
                Accent newAccent;
                AppTheme newAppTheme;
                string ColorName;
                ColorsCollection SelectedItem;
                string AccentColor;

                try
                {
                    ThemeName = "Base" + (lsttheme.SelectedItem as ThemeCollection).Name;
                }
                catch (Exception )
                {
                    ThemeName = "BaseLight";
                }

                if (sender == lstcolor || sender == lsttheme)
                {
                    ColorName = ((ColorsCollection)lstcolor.SelectedItem).Name;
                    SelectedItem = (ColorsCollection)lstcolor.SelectedItem;
                    AccentColor = objAppearanceViewModel.lstColorsCollection.SingleOrDefault(x => x.Value == SelectedItem.Value).Name;
                }
                else
                {
                    ColorName = ((ColorsCollection)lstRecentcolor.SelectedItem).Name;
                    SelectedItem = (ColorsCollection)lstRecentcolor.SelectedItem;
                    AccentColor = objAppearanceViewModel.lstRecentColorsCollection.FirstOrDefault(x => x.Value == SelectedItem.Value).Name;
                }
                if (ColorName == "Default")
                {
                    ThemeManager.AddAccent("PrussianBlue", new Uri("pack://application:,,,/LegionUIUtility;component/Themes/PrussianBlue.xaml"));
                    newAccent = ThemeManager.GetAccent("PrussianBlue");
                    newAppTheme = ThemeManager.GetAppTheme(ThemeName);
                }
                else
                {
                    newAccent = ThemeManager.GetAccent(AccentColor);
                    newAppTheme = ThemeManager.GetAppTheme(ThemeName);
                }
                ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            SaveCurrentTheme();
        }

        private void SaveCurrentTheme()
        {
            Configuration configuration = new Configuration();
            configuration.ConfigurationDate = DateTime.Now;
            configuration.ConfigurationType = "Theme";
            var Theme = new Themes
            {
                SelectedAccentColor = new AccentColors(objAppearanceViewModel.SelectedAccentColor.Name, objAppearanceViewModel.SelectedAccentColor.Value),
                SelectedTheme = new Theme(objAppearanceViewModel.SelectedTheme.Name, objAppearanceViewModel.SelectedTheme.Value)
            };
            configuration.ConfigurationSetting = Newtonsoft.Json.JsonConvert.SerializeObject(Theme);
            ConfigFileManager.SaveConfig(configuration);
        }
    }
}

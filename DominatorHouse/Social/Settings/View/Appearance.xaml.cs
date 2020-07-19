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
                Accent newAccent;
                AppTheme newAppTheme;
                string colorName;
                ColorsCollection selectedItem;
                string accentColor;

                var themeName = "Base" + ((lsttheme?.SelectedItem as ThemeCollection)?.Name ?? "Light");

                if (Equals(sender, lstcolor) || Equals(sender, lsttheme))
                {
                    colorName = ((ColorsCollection)lstcolor.SelectedItem).Name;
                    selectedItem = (ColorsCollection)lstcolor.SelectedItem;
                    accentColor = objAppearanceViewModel.lstColorsCollection.SingleOrDefault(x => x.Value == selectedItem.Value)?.Name;
                }
                else
                {
                    colorName = ((ColorsCollection)lstRecentcolor.SelectedItem).Name;
                    selectedItem = (ColorsCollection)lstRecentcolor.SelectedItem;
                    accentColor = objAppearanceViewModel.lstRecentColorsCollection.FirstOrDefault(x => x.Value == selectedItem.Value)?.Name;
                }
                if (colorName == "Default")
                {
                    ThemeManager.AddAccent("PrussianBlue", new Uri("pack://application:,,,/DominatorUIUtility;component/Themes/PrussianBlue.xaml"));
                    newAccent = ThemeManager.GetAccent("PrussianBlue");
                    newAppTheme = ThemeManager.GetAppTheme(themeName);
                }
                else
                {
                    newAccent = ThemeManager.GetAccent(accentColor);
                    newAppTheme = ThemeManager.GetAppTheme(themeName);
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

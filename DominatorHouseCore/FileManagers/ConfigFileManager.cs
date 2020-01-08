using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DominatorHouseCore.FileManagers
{
    public class ConfigFileManager
    {
        public static bool SaveConfig(Configuration config)
        {
            try
            {
                var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
                binFileHelper.SaveConfig(config);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static IEnumerable<Configuration> GetAllConfig()
        {
            var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
            return binFileHelper.GetConfigDetails<Configuration>();
        }
        public static Configuration GetConfigWithType(string ConfigType)
        {
            return GetAllConfig().LastOrDefault(config => config.ConfigurationType == ConfigType);
        }
        public static void ApplyTheme()
        {
            try
            {
                Configuration config = GetConfigWithType("Theme");

                string serializedThemes = config?.ConfigurationSetting;
                if (!string.IsNullOrEmpty(serializedThemes))
                {
                    var Themes = Newtonsoft.Json.JsonConvert.DeserializeObject<Themes>(config.ConfigurationSetting);

                    if (Themes == null)
                        return;

                    Accent newAccent;

                    AppTheme newAppTheme = ThemeManager.GetAppTheme("Base" + Themes.SelectedTheme.Name);

                    if (Themes.SelectedTheme.Name == "Default")
                    {
                        ThemeManager.AddAccent("PrussianBlue", new Uri("pack://application:,,,/LegionUIUtility;component/Themes/PrussianBlue.xaml"));
                        newAccent = ThemeManager.GetAccent("PrussianBlue");
                    }
                    else
                        newAccent = ThemeManager.GetAccent(Themes.SelectedAccentColor.Name);

                    ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}

using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DominatorHouseCore.FileManagers
{
    public class ConfigFileManager
    {
        public static bool SaveConfig<T>(T Config) where T : class
        {
            try
            {
                BinFileHelper.SaveConfig(Config);
                GlobusLogHelper.log.Debug($"Configuration successfully saved");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }         
        }
        public static IEnumerable<Configuration> GetAllConfig()
        {
            return BinFileHelper.GetConfigDetails<Configuration>();
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

                    Accent newAccent = null;

                    AppTheme newAppTheme = ThemeManager.GetAppTheme("Base" + Themes.SelectedTheme.Name);

                    if (Themes.SelectedTheme.Name == "Default")
                    {
                        ThemeManager.AddAccent("PrussianBlue", new Uri("pack://application:,,,/DominatorUIUtility;component/Themes/PrussianBlue.xaml"));
                        newAccent = ThemeManager.GetAccent("PrussianBlue");
                    }
                    else
                        newAccent = ThemeManager.GetAccent(Themes.SelectedAccentColor.Name);

                    ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);
                }

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.StackTrace);
            }
        }

    }
}

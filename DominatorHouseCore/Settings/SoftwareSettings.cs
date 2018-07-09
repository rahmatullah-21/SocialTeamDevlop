using System.IO;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Settings
{
    public class SoftwareSettings
    {

        public static SoftwareSettingsModel Settings { get; set; }
        public void InitializeOnLoadConfigurations()
        {
            CheckConfigurationFiles();
            Settings = SoftwareSettingsFileManager.GetSoftwareSettings();

            var shortnerServices =
                GenericFileManager.GetModel<UrlShortnerServicesModel>(ConstantVariable.GetURLShortnerServicesFile())??new UrlShortnerServicesModel();
            ConstantVariable.BitlyLogin= shortnerServices.Login;
            ConstantVariable.BitlyApiKey = shortnerServices.ApiKey;
        }
    
        private void CheckConfigurationFiles()
        {
            if (!File.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()))
            {
                SoftwareSettingsModel softwareSettingsModel = new SoftwareSettingsModel();
                SoftwareSettingsFileManager.SaveSoftwareSettings(softwareSettingsModel);
            }
        }
    }
}

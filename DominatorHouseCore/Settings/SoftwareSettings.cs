using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Settings
{
    public interface ISoftwareSettings
    {
        void InitializeOnLoadConfigurations();
        SoftwareSettingsModel Settings { get; }
        bool Save();
    }

    public class SoftwareSettings : ISoftwareSettings
    {
        private readonly ISoftwareSettingsFileManager _softwareSettingsFileManager;
        private readonly IFileSystemProvider _fileSystemProvider;

        public SoftwareSettings(ISoftwareSettingsFileManager softwareSettingsFileManager, IFileSystemProvider fileSystemProvider)
        {
            _softwareSettingsFileManager = softwareSettingsFileManager;
            _fileSystemProvider = fileSystemProvider;
        }

        public SoftwareSettingsModel Settings { get; private set; }

        public void InitializeOnLoadConfigurations()
        {
            if (CheckConfigurationFiles())
            {
                Settings = _softwareSettingsFileManager.GetSoftwareSettings();
            }

            if (_fileSystemProvider.Exists(ConstantVariable.GetURLShortnerServicesFile()))
            {
                var shortnerServices =
                GenericFileManager.GetModel<UrlShortnerServicesModel>(ConstantVariable.GetURLShortnerServicesFile());
                ConstantVariable.BitlyLogin = shortnerServices.Login;
                ConstantVariable.BitlyApiKey = shortnerServices.ApiKey;
            }
        }

        private bool CheckConfigurationFiles()
        {
            if (!_fileSystemProvider.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()))
            {
                Settings = new SoftwareSettingsModel
                {
                    IsEnableAdvancedUserMode = true
                };

                _softwareSettingsFileManager.SaveSoftwareSettings(Settings);

                return false;
            }

            return true;
        }

        public bool Save()
        {
            return _softwareSettingsFileManager.SaveSoftwareSettings(Settings);
        }
    }
}

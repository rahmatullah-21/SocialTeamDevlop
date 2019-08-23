using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class SoftwareSettingsViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly ISoftwareSettings _softwareSettings;
        public SoftwareSettingsModel SoftwareSettingsModel { get; }
        public DelegateCommand SaveCmd { get; }
        public DelegateCommand ExportCommand { get; }

        public SoftwareSettingsViewModel(ISoftwareSettings softwareSettings) : base("LangKeySoftwareSettings", "SoftwareSettingsControlTemplate")
        {
            _softwareSettings = softwareSettings;
            SaveCmd = new DelegateCommand(Save);
            SoftwareSettingsModel = softwareSettings.Settings;
            ExportCommand = new DelegateCommand(Export);
        }

        private void Export()
        {
            SoftwareSettingsModel.ExportPath = FileUtilities.GetExportPath(true);
        }

        private void Save()
        {
            if (SoftwareSettingsModel.IsDefaultExportPathSelected)
            {
                if (!string.IsNullOrEmpty(SoftwareSettingsModel.ExportPath) && Directory.Exists(SoftwareSettingsModel.ExportPath))
                    SaveSetting();
                else
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), "LangKeyEnterValidFolderPath".FromResourceDictionary());
            }
            else
            {
                SoftwareSettingsModel.ExportPath = string.Empty;
                SaveSetting();
            }

        }

        private void SaveSetting()
        {
            if (_softwareSettings.Save())
            {
                var result = Dialog.ShowCustomDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyConfirmToRestartAfterSoftwareSettingSaved".FromResourceDictionary(),
                    "LangKeyRestartNow".FromResourceDictionary(), "LangKeyRestartLater".FromResourceDictionary());
                if (result == MessageDialogResult.Affirmative)
                {
                    Application.Current.Shutdown();
                    Process.Start(Application.ResourceAssembly.Location);
                    Process.GetCurrentProcess().Kill();
                    Environment.Exit(0);
                }
            }
        }
    }
}

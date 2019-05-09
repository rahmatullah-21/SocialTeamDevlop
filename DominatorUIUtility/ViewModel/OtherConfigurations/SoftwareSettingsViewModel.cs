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
            SoftwareSettingsModel.ExportPath = FileUtilities.GetExportPath();
        }

        private void Save()
        {
            if (SoftwareSettingsModel.IsDefaultExportPathSelected && !string.IsNullOrEmpty(SoftwareSettingsModel.ExportPath))
                if (Directory.Exists(SoftwareSettingsModel.ExportPath))
                {
                    if (_softwareSettings.Save())
                    {
                        var result = Dialog.ShowCustomDialog("Success",
                            "Software Settings sucessfully saved.To apply this setting you need to restart.\nDo you want to Restart?", "Restart now", "Restart later");
                        if (result == MessageDialogResult.Affirmative)
                        {
                            Application.Current.Shutdown();
                            Process.Start(Application.ResourceAssembly.Location);
                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    SoftwareSettingsModel.ExportPath = string.Empty;
                    
                    Dialog.ShowDialog("Error", "Please enter valid folder Path.");
                }


        }
    }
}

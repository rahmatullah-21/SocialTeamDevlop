using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class SoftwareSettingsViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        public SoftwareSettingsModel SoftwareSettingsModel { get; }
        public DelegateCommand SaveCmd { get; }

        public SoftwareSettingsViewModel() : base("LangKeySoftwareSettings", "SoftwareSettingsControlTemplate")
        {
            SaveCmd = new DelegateCommand(Save);
            SoftwareSettingsModel = SoftwareSettingsFileManager.GetSoftwareSettings();
            if (SoftwareSettingsModel == null)
            {
                SoftwareSettingsModel = new SoftwareSettingsModel()
                {
                    IsEnableAdvancedUserMode = true
                };
                SoftwareSettingsFileManager.SaveSoftwareSettings(SoftwareSettingsModel);
            }
        }

        private void Save()
        {
            if (SoftwareSettingsFileManager.SaveSoftwareSettings(SoftwareSettingsModel))
            {
                var result = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Software Settings sucessfully saved.To apply this setting you need to restart.\nDo you want to Restart?", MessageDialogStyle.AffirmativeAndNegative,
                    Dialog.SetMetroDialogButton("Restart now", "Restart later"));
                if (result == MessageDialogResult.Affirmative)
                {
                    Application.Current.Shutdown();
                    Process.Start(Application.ResourceAssembly.Location);
                    //var clickonceApp = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\" + Constants.ProductName + "\\" + Constants.ClickOnceFileName;
                    //try
                    //{
                    //    Process.Start(clickonceApp);
                    //}
                    //catch (Exception ex)
                    //{
                    //    ex.DebugLog();
                    //    Process.Start(Application.ResourceAssembly.Location);
                    //}
                    Environment.Exit(0);

                }
            }
        }
    }
}

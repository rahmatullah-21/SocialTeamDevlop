using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Microsoft.Win32;

namespace DominatorHouseCore.Settings
{
    public class SoftwareSettings
    {

        public static SoftwareSettingsModel Settings { get; set; }
        public void InitializeOnLoadConfigurations()
        {
            CheckConfigurationFiles();
            Settings = SoftwareSettingsFileManager.GetSoftwareSettings();
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

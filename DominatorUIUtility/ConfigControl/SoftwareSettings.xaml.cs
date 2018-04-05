using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for SoftwareSettings.xaml
    /// </summary>
    public partial class SoftwareSettings : UserControl
    {
        private SoftwareSettingsModel SoftwareSettingsModel { get; set; }=new SoftwareSettingsModel();
        private SoftwareSettings()
        {
            InitializeComponent();
            SoftwareSettingsModel = SoftwareSettingsFileManager.GetSoftwareSettings() ?? SoftwareSettingsModel;
            MainGrid.DataContext = SoftwareSettingsModel;
        }

        private static SoftwareSettings ObjSoftwareSettings;

        public static SoftwareSettings GetSingeltonObjectSoftwareSettings()
        {
            return ObjSoftwareSettings ?? (ObjSoftwareSettings = new SoftwareSettings());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (SoftwareSettingsFileManager.SaveSoftwareSettings(SoftwareSettingsModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Software Settings sucessfully saved !!");

        }
    }
}

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
    /// Interaction logic for EmbeddedBrowserSettings.xaml
    /// </summary>
    public partial class EmbeddedBrowserSettings : UserControl
    {
        private EmbeddedBrowserSettingsModel EmbeddedBrowserSettingsModel { get; set; }=new EmbeddedBrowserSettingsModel();
        private EmbeddedBrowserSettings()
        {
            InitializeComponent();
     
            EmbeddedBrowserSettingsModel = EmbeddedBrowserSettingsFileManager.GetEmbeddedBrowserSettings()?? EmbeddedBrowserSettingsModel;
            MainGrid.DataContext = EmbeddedBrowserSettingsModel;
        }
        private static EmbeddedBrowserSettings ObjEmbeddedBrowserSettings;

        public static EmbeddedBrowserSettings GetSingeltonObjectEmbeddedBrowserSettings()
        {
            return ObjEmbeddedBrowserSettings ?? (ObjEmbeddedBrowserSettings = new EmbeddedBrowserSettings());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (EmbeddedBrowserSettingsFileManager.SaveEmbeddedBrowserSettings(EmbeddedBrowserSettingsModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Embedded Browser Settings sucessfully saved !!");
        }
    }
}

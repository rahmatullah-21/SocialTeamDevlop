using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for EmbeddedBrowserSettings.xaml
    /// </summary>
    public partial class EmbeddedBrowserSettings : UserControl
    {
        private EmbeddedBrowserSettingsModel EmbeddedBrowserSettingsModel { get; set; } = new EmbeddedBrowserSettingsModel();
        IOtherConfigFileManager embeddedBrowserSettings;
        private EmbeddedBrowserSettings()
        {
            InitializeComponent();
            embeddedBrowserSettings = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            EmbeddedBrowserSettingsModel = embeddedBrowserSettings.GetOtherConfig<EmbeddedBrowserSettingsModel>() ?? EmbeddedBrowserSettingsModel;
            MainGrid.DataContext = EmbeddedBrowserSettingsModel;
        }
        private static EmbeddedBrowserSettings ObjEmbeddedBrowserSettings;

        public static EmbeddedBrowserSettings GetSingeltonObjectEmbeddedBrowserSettings()
        {
            return ObjEmbeddedBrowserSettings ?? (ObjEmbeddedBrowserSettings = new EmbeddedBrowserSettings());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (embeddedBrowserSettings.SaveOtherConfig(EmbeddedBrowserSettingsModel))
                Dialog.ShowDialog("Success", "Embedded Browser Settings sucessfully saved !!");
        }
    }
}

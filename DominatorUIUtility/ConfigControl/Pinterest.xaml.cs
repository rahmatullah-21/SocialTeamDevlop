using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest
    {
        IOtherConfigFileManager PinterestConfig;
        private PinterestModel PinterestModel { get; set; } = new PinterestModel();
        private Pinterest()
        {
            InitializeComponent();
            PinterestConfig = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            PinterestModel = PinterestConfig.GetOtherConfig<PinterestModel>() ?? PinterestModel;
            MainGrid.DataContext = PinterestModel;
        }
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (PinterestConfig.SaveOtherConfig(PinterestModel))
                Dialog.ShowDialog("Success", "Pinterest Configuration sucessfully saved !!");
        }
    }
}

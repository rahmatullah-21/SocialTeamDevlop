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
    /// Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram
    {
        IOtherConfigFileManager InstagramConfig;
        private InstagramModel InstagramModel { get; set; } = new InstagramModel();
        private Instagram()
        {
            InitializeComponent();
            InstagramConfig = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            InstagramModel = InstagramConfig.GetOtherConfig<InstagramModel>() ?? InstagramModel;
            MainGrid.DataContext = InstagramModel;
        }
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (InstagramConfig.SaveOtherConfig(InstagramModel))
                Dialog.ShowDialog("Success", "Instagram Configuration sucessfully saved !!");
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram : UserControl
    {
        IOtherConfigFileManager InstagramConfig;
        private InstagramModel InstagramModel { get; set; } = new InstagramModel();
        private Instagram()
        {
            InitializeComponent();
            InstagramConfig = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            InstagramModel = InstagramConfig.GetOtherConfig<InstagramModel>() ?? InstagramModel;
            //  InstagramModel = IGFileManager.GetInstagramConfig() ?? InstagramModel;
            MainGrid.DataContext = InstagramModel;
        }

        private static Instagram ObjInstagram;

        public static Instagram GetSingeltonObjectInstagram()
        {
            return ObjInstagram ?? (ObjInstagram = new Instagram());

        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            //if (IGFileManager.SaveInstagramConfig(InstagramModel))
            //    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
            //        "Instagram Configuration sucessfully saved !!");
            if (InstagramConfig.SaveOtherConfig(InstagramModel))
                Dialog.ShowDialog("Success", "Instagram Configuration sucessfully saved !!");
        }
    }
}

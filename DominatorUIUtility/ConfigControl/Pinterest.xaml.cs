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
    public partial class Pinterest : UserControl
    {
        IOtherConfigFileManager PinterestConfig;
        private PinterestModel PinterestModel { get; set; } = new PinterestModel();
        private Pinterest()
        {
            InitializeComponent();
            PinterestConfig = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            PinterestModel = PinterestConfig.GetOtherConfig<PinterestModel>() ?? PinterestModel;
            //PinterestModel = PinFileManager.GetPinterestConfig() ?? PinterestModel;
            MainGrid.DataContext = PinterestModel;
        }
        private static Pinterest ObjPinterest;

        public static Pinterest GetSingeltonObjectPinterest()
        {
            return ObjPinterest ?? (ObjPinterest = new Pinterest());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            //if (PinFileManager.SavePinterestConfig(PinterestModel))
            //    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
            //        "Pinterest Configuration sucessfully saved !!");

            if (PinterestConfig.SaveOtherConfig(PinterestModel))
                Dialog.ShowDialog("Success", "Pinterest Configuration sucessfully saved !!");
        }
    }
}

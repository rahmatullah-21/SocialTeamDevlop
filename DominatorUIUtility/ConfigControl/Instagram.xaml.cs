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
    /// Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram : UserControl
    {

      

        private InstagramModel InstagramModel { get; set; } = new InstagramModel();
        private Instagram()
        {
            InitializeComponent();
            InstagramModel = IGFileManager.GetInstagramConfig() ?? InstagramModel;
            MainGrid.DataContext = InstagramModel;
        }

        private static Instagram ObjInstagram;

        public static Instagram GetSingeltonObjectInstagram()
        {
            return ObjInstagram ?? (ObjInstagram = new Instagram());

        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (IGFileManager.SaveInstagramConfig(InstagramModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Instagram Configuration sucessfully saved !!");
        }
    }
}

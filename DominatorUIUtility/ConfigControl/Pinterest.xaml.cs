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
    /// Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest : UserControl
    {
        private PinterestModel PinterestModel { get; set; }=new PinterestModel();
        private Pinterest()
        {
            InitializeComponent();
            PinterestModel = PinFileManager.GetPinterestConfig() ?? PinterestModel;
            MainGrid.DataContext = PinterestModel;
        }
        private static Pinterest ObjPinterest;

        public static Pinterest GetSingeltonObjectPinterest()
        {
            return ObjPinterest ?? (ObjPinterest = new Pinterest());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (PinFileManager.SavePinterestConfig(PinterestModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Pinterest Configuration sucessfully saved !!");
        }
    }
}

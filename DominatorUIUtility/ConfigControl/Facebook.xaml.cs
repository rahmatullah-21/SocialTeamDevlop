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
    /// Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl
    {
        private FacebookModel FacebookModel { get; set; }=new FacebookModel();
        private Facebook()
        {
            InitializeComponent();
            FacebookModel = FBFileManager.GetFacebookConfig() ?? FacebookModel;
            MainGrid.DataContext = FacebookModel;
        }

        private static Facebook ObjFacebook;

        public static Facebook GetSingeltonObjectFacebook()
        {
            return ObjFacebook ?? (ObjFacebook = new Facebook());

        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (FBFileManager.SaveFacebookConfig(FacebookModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Facebook Configuration sucessfully saved !!");
        }
    }
}

using System.Windows;
using System.Windows.Controls;
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

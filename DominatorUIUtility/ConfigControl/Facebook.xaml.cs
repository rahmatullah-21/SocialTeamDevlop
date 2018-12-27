using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl
    {
        private FacebookModel FacebookModel { get; set; } = new FacebookModel();
        private readonly IFBFileManager fbFilemanager;
        private Facebook()
        {
            InitializeComponent();
            fbFilemanager = ServiceLocator.Current.GetInstance<IFBFileManager>();
            FacebookModel = fbFilemanager.GetFacebookConfig() ?? FacebookModel;
            MainGrid.DataContext = FacebookModel;
        }

        private static Facebook ObjFacebook;

        public static Facebook GetSingeltonObjectFacebook()
        {
            return ObjFacebook ?? (ObjFacebook = new Facebook());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (fbFilemanager.SaveFacebookConfig(FacebookModel))
                Dialog.ShowDialog("Success", "Facebook Configuration sucessfully saved !!");
        }
    }
}

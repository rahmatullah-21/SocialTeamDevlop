using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Youtube.xaml
    /// </summary>
    public partial class Youtube : UserControl
    {
        private YoutubeModel YoutubeModel { get; set; }=new YoutubeModel();
        public Youtube()
        {
            InitializeComponent();
            YoutubeModel = YoutubeFileManager.GetYoutubeConfig() ?? YoutubeModel;
            MainGrid.DataContext = YoutubeModel;
        }
        private static Youtube ObjYoutube;

        public static Youtube GetSingeltonObjectYoutube()
        {
            return ObjYoutube ?? (ObjYoutube = new Youtube());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (YoutubeFileManager.SaveYoutubeConfig(YoutubeModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Youtube Configuration sucessfully saved !!");
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Youtube.xaml
    /// </summary>
    public partial class Youtube : UserControl
    {
        IOtherConfigFileManager _otherConfigFileManager;
        private YoutubeModel YoutubeModel { get; set; } = new YoutubeModel();
        public Youtube()
        {
            InitializeComponent();
            _otherConfigFileManager = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            YoutubeModel = _otherConfigFileManager.GetOtherConfig<YoutubeModel>() ?? YoutubeModel;
            MainGrid.DataContext = YoutubeModel;
        }
        private static Youtube ObjYoutube;

        public static Youtube GetSingeltonObjectYoutube()
        {
            return ObjYoutube ?? (ObjYoutube = new Youtube());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (_otherConfigFileManager.SaveOtherConfig(YoutubeModel))
                Dialog.ShowDialog("Success", "Youtube Configuration sucessfully saved !!");
        }
    }
}

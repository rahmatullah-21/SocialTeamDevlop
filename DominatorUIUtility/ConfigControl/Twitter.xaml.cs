using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Twitter.xaml
    /// </summary>
    public partial class Twitter : UserControl
    {
        private TwitterModel TwitterModel { get; set; }=new TwitterModel();
        private Twitter()
        {
            InitializeComponent();
            TwitterModel = TwitterFileManager.GetTwitterConfig() ?? TwitterModel;
            MainGrid.DataContext = TwitterModel;
        }
        private static Twitter ObjTwitter;

        public static Twitter GetSingeltonObjectTwitter()
        {
            return ObjTwitter ?? (ObjTwitter = new Twitter());
        }

        private void BtnSave_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (TwitterFileManager.SaveTwitterConfig(TwitterModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(System.Windows.Application.Current.MainWindow, "Success",
                    "Twitter Configuration sucessfully saved !!");

        }
    }
}

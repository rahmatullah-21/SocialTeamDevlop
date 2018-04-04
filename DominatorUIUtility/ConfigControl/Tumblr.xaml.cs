using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Tumblr.xaml
    /// </summary>
    public partial class Tumblr : UserControl
    {
        private TumblrModel TumblrModel { get; set; }=new TumblrModel();
        public Tumblr()
        {
            InitializeComponent();
            TumblrModel = TumblrFileManager.GetTumblrConfig() ?? TumblrModel;
            MainGrid.DataContext = TumblrModel;
        }
        private static Tumblr ObjTumblr;

        public static Tumblr GetSingeltonObjectTumblr()
        {
            return ObjTumblr ?? (ObjTumblr = new Tumblr());
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (TumblrFileManager.SaveTumblrConfig(TumblrModel))
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                    "Tumblr Configuration sucessfully saved !!");

        }
    }
}

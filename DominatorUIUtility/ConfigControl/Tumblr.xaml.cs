using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Tumblr.xaml
    /// </summary>
    public partial class Tumblr
    {
        private TumblrModel TumblrModel { get; set; } = new TumblrModel();
        IOtherConfigFileManager _otherConfigFileManager;
        public Tumblr()
        {
            InitializeComponent();
            _otherConfigFileManager = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            TumblrModel = _otherConfigFileManager.GetOtherConfig<TumblrModel>() ?? TumblrModel;
            MainGrid.DataContext = TumblrModel;
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (_otherConfigFileManager.SaveOtherConfig(TumblrModel))
                Dialog.ShowDialog("Success", "Tumblr Configuration sucessfully saved !!");
        }
    }
}

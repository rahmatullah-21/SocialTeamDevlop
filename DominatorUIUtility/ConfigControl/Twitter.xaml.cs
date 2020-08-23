using System.Windows.Controls;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using CommonServiceLocator;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ConfigControl
{
    /// <summary>
    /// Interaction logic for Twitter.xaml
    /// </summary>
    public partial class Twitter
    {
        IOtherConfigFileManager _otherConfigFileManager;
        private TwitterModel TwitterModel { get; set; } = new TwitterModel();
        private Twitter()
        {
            InitializeComponent();
            _otherConfigFileManager = ServiceLocator.Current.GetInstance<IOtherConfigFileManager>();
            TwitterModel = _otherConfigFileManager.GetOtherConfig<TwitterModel>() ?? TwitterModel;
            MainGrid.DataContext = TwitterModel;
        }

        private void BtnSave_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_otherConfigFileManager.SaveOtherConfig(TwitterModel))
                Dialog.ShowDialog("Success", "Twitter Configuration sucessfully saved !!");
        }
    }
}

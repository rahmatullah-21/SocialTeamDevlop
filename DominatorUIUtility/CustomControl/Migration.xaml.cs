using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Migration.xaml
    /// </summary>
    public partial class Migration
    {
        private MigrationProgress MigrationProgress;
        private Window window;
        public SocialNetworks SocialNetworks { get; set; }
        public Migration()
        {
            InitializeComponent();
        }
        public Migration(SocialNetworks networks) : this()
        {
            InitializeComponent();
            SocialNetworks = networks;

        }

        private void AccountClick(object sender, RoutedEventArgs e)
        {
            MigrationProgress = new MigrationProgress();
            Application.Current.Dispatcher.Invoke(() =>
            {

                var result = Dialog.ShowCustomDialog("Confirmation", "Are you sure?", "Yes", "No");
                if (result == MessageDialogResult.Affirmative)
                {
                    Dialog dialog = new Dialog();
                    window = dialog.GetMetroWindowWithOutClose(MigrationProgress, "Migration start");
                    window.ShowDialog();
                    account.IsEnabled = false;
                }
            });
           
        }
    }
}

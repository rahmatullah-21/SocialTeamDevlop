using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for Migration.xaml
    /// </summary>
    public partial class Migration : UserControl
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

        private void MigratingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MigrationProgress.ProgressRing.IsActive = false;
            
        }

        private void Migrating(object sender, DoWorkEventArgs e)
        {
            
        }
    }
}

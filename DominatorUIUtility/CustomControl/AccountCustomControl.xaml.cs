using System.Windows.Controls;
using DominatorHouseCore.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : UserControl
    {

        #region Property

        public DominatorAccountViewModel DominatorAccountViewModel { get; set; }

        #endregion


        public AccountCustomControl()
        {
            InitializeComponent();
            MainGrid.DataContext = DominatorAccountViewModel;
        }

        private void editContextMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void MangeblacklistedContextMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void MangewhitelistUserContextMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DeleteSingleContextMenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void MenuCheckAccount_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        
    }
}

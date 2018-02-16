using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : UserControl
    {

        #region Property

        public DominatorAccountViewModel DominatorAccountViewModel { get; set; } = new DominatorAccountViewModel();

        #endregion


        public AccountCustomControl()
        {
            InitializeComponent();
            MainGrid.DataContext = DominatorAccountViewModel;
        }

        private void editContextMenu_Click(object sender, RoutedEventArgs e)
        {
            DominatorAccountViewModel.EditAccount(sender);
        }

        private void MangeblacklistedContextMenu_Click(object sender, RoutedEventArgs e)
        {
            BlacklistUserControl objBlacklistUserControl = new BlacklistUserControl();

            var window = new Window()
            {
                Content = objBlacklistUserControl,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            window.ShowDialog();
        }

        private void MangewhitelistUserContextMenu_Click(object sender, RoutedEventArgs e)
        {
            WhitelistuserControl objWhitelistuserControl = new WhitelistuserControl();

            var window = new Window()
            {
                Content = objWhitelistuserControl,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowDialog();
        }

        private void DeleteSingleContextMenu_Click(object sender, RoutedEventArgs e)
        {
            DominatorAccountViewModel.DeleteAccountByContextMenu(sender);
        }

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            DominatorAccountViewModel.SelectAccountByGroup(sender);
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            DominatorAccountViewModel.SelectAccountByGroup(sender);
        }

        private void MenuCheckAccount_OnClick(object sender, RoutedEventArgs e)
        {
            DominatorAccountModel ObjDominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
            //DominatorAccountViewModel.UpdateAccount(ObjDominatorAccountModel);
        }


    }
}

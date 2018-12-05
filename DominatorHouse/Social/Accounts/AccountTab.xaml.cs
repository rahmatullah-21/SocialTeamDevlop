using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using DominatorHouseCore.Models;
namespace Socinator.Social.Accounts
{
    /// <summary>
    /// Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab : UserControl 
    {
        public AccountTab(AccessorStrategies strategies)
        {
            InitializeComponent();
            DominatorAccountTab.DataContext = this;
            SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, strategies);
        }

        public AccountCustomControl SelectedUserControl { get; set; }
    }
}

using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using System.Windows.Controls;

namespace DominatorHouse.Social.Accounts
{
    /// <summary>
    /// Interaction logic for AccountGrowthTab.xaml
    /// </summary>
    public partial class AccountGrowthTab : UserControl
    {
        private AccessorStrategies _strategies;
        public AccountGrowthTab(AccessorStrategies strategies)
        {
            this._strategies = strategies;
            InitializeComponent();
            DominatorAccountGrowthTab.DataContext = this;
            SelectedUserControl = ServiceLocator.Current.GetInstance<AccountGrowthControl>();
        }
        public AccountGrowthControl SelectedUserControl { get; set; }
    }
}

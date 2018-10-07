using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using DominatorHouseCore.Models;

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
            SelectedUserControl = AccountGrowthControl.GetAccountGrowthControl(SocialNetworks.Social, _strategies);
        }
        public AccountGrowthControl SelectedUserControl { get; set; }
    }
}

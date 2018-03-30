using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;

namespace DominatorHouse.Social.Accounts
{
    /// <summary>
    /// Interaction logic for AccountTab.xaml
    /// </summary>
    public partial class AccountTab : UserControl 
    {     
        public AccountTab()
        {
            InitializeComponent();
            DominatorAccountTab.DataContext = this;
            SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
        }

        public AccountCustomControl SelectedUserControl { get; set; }
    }
}

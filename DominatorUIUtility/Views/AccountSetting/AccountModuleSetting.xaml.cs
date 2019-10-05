using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel.Startup;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace DominatorUIUtility.Views.AccountSetting
{
    /// <summary>
    /// Interaction logic for AccountModuleSetting.xaml
    /// </summary>
    public partial class AccountModuleSetting : MetroWindow
    {
        public AccountModuleSetting(SocialNetworks AccountNetwork)
        {
            InitializeComponent();
            var viewModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            viewModel.SelectedNetwork = AccountNetwork.ToString();
        }
    }
}

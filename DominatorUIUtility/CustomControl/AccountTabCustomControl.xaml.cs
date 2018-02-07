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

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountTabCustomControl.xaml
    /// </summary>
    public partial class AccountTabCustomControl : UserControl
    {
        public AccountTabCustomControl()
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title =FindResource("langAccountsManager").ToString(),
                    Content = new Lazy<UserControl>(() => new AccountCustomControl())
                },
                //new TabItemTemplates
                //{
                //    Title =FindResource("langAccountsManager").ToString(),
                //    Content = new Lazy<UserControl>(()=> new AccountManager())
                //},
                //new TabItemTemplates
                //{
                //    Title =FindResource("langAccountStatsBeta").ToString(),
                //    Content = new Lazy<UserControl>(()=> new AccountStats())
                //}
            };

            AccountTabs.ItemsSource = items;
        }
    }
}

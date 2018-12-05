using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountTabCustomControl.xaml
    /// </summary>
    public partial class AccountTabCustomControl : UserControl
    {
        public AccountTabCustomControl(AccessorStrategies strategies)
        {
            InitializeComponent();
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title =FindResource("LangKeyAccountsManager").ToString(),
                    Content = new Lazy<UserControl>(() =>  AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, strategies))
                },
                //new TabItemTemplates
                //{
                //    Title =FindResource("LangKeyAccountsManager").ToString(),
                //    Content = new Lazy<UserControl>(()=> new AccountManager())
                //},
                //new TabItemTemplates
                //{
                //    Title =FindResource("LangKeyAccountStatsBeta").ToString(),
                //    Content = new Lazy<UserControl>(()=> new AccountStats())
                //}
            };

            AccountTabs.ItemsSource = items;
        }
    }
}

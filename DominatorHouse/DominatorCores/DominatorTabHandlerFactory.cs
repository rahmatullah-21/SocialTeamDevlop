using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using Socinator.Social.Accounts;
using Socinator.Social.AutoActivity.Views;
using Socinator.Social.OtherConfiguration;

//using EmbeddedBrowser;

namespace Socinator.DominatorCores
{
    public class DominatorTabHandlerFactory : ITabHandlerFactory
    {
        private DominatorAccountViewModel.AccessorStrategies _strategies;

        public List<TabItemTemplates> NetworkTabs { get; set; }

        public string NetworkName { get; set; }

        private static DominatorTabHandlerFactory _instance;

        public static DominatorTabHandlerFactory GetInstance(DominatorAccountViewModel.AccessorStrategies strategies)
            => _instance ?? (_instance = new DominatorTabHandlerFactory(strategies));

        private DominatorTabHandlerFactory(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            _strategies = strategies;
            NetworkTabs = new List<TabItemTemplates>();
            NetworkName = $"The {SocialNetworks.Social} Dominator";
            InitializeAllTabs();
        }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
        }

        private void InitializeAllTabs()
        {

            NetworkTabs= new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langAccountsManager") == null? "Account Manager" : Application.Current.FindResource("langAccountsManager")?.ToString(),
                    Content = new Lazy<UserControl>(() => new AccountTab(_strategies))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langDashBoard") == null? "Dash Board" : Application.Current.FindResource("langDashBoard")?.ToString(),
                    //Content=new Lazy<UserControl>(()=>new DashBoard())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langAutoActivity") == null? "Auto Activity" : Application.Current.FindResource("langAutoActivity")?.ToString(),
                    Content = new Lazy<UserControl>(() =>
                        DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langPublisher") == null? "Publisher" : Application.Current.FindResource("langPublisher")?.ToString(),
                    Content = new Lazy<UserControl>(()=> PublisherIndexPage.Instance)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langProxyManager") == null? "Proxy Manager" : Application.Current.FindResource("langProxyManager")?.ToString(),
                    Content = new Lazy<UserControl>(() => new ProxyManager(_strategies))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langSettings") == null? "Settings" : Application.Current.FindResource("langSettings")?.ToString(),
                    Content = new Lazy<UserControl>(() => new Social.Settings.View.Home())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langOtherConfigurations") == null? "Other Configuration" : Application.Current.FindResource("langOtherConfigurations")?.ToString(),
                      Content=new Lazy<UserControl>(()=>new OtherConfigurationTab())
                }
            };
        }




    }
}
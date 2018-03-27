using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.Publisher;
using EmbeddedBrowser;

namespace DominatorHouse.DominatorCores
{
    public class DominatorTabHandlerFactory : ITabHandlerFactory
    {
        public List<TabItemTemplates> NetworkTabs { get; set; }

        public string NetworkName { get; set; }

        private static DominatorTabHandlerFactory _instance;

        public static DominatorTabHandlerFactory Instance
            => _instance ?? (_instance = new DominatorTabHandlerFactory());

        private DominatorTabHandlerFactory()
        {
            NetworkTabs =  new List<TabItemTemplates>();
            NetworkName = $"The {SocialNetworks.Social} Dominator";
            NetworkTabs = InitializeAllTabs();
        }

        public void StartAccountCustomControl(SocialNetworks networks)
            => AccountCustomControl.GetAccountCustomControl(networks);

        private List<TabItemTemplates> InitializeAllTabs()
        {
            var accountCustomControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);

            return new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langAccountsManager") == null? "Account Manager" : Application.Current.FindResource("langAccountsManager")?.ToString(),
                    Content = new Lazy<UserControl>(() => accountCustomControl)
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
                    Content = new Lazy<UserControl>(Home.GetSingletonHome)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langProxyManager") == null? "Proxy Manager" : Application.Current.FindResource("langProxyManager")?.ToString(),                 
                    Content = new Lazy<UserControl>(() => new ProxyManager())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langSettings") == null? "Settings" : Application.Current.FindResource("langSettings")?.ToString(),        
                    Content = new Lazy<UserControl>(() => new Social.Settings.View.Home())
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("langOtherConfigurations") == null? "Other Configuration" : Application.Current.FindResource("langOtherConfigurations")?.ToString(),
                    //  Content=new Lazy<UserControl>(()=>new OtherConfiguration())
                }
            };
        }


       

    }
}
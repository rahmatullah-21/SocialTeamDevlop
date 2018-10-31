using DominatorHouse.Social;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;
using DominatorUIUtility.Views.SocioPublisher;
using Socinator.Social.AutoActivity.Views;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Unity;


//using EmbeddedBrowser;

namespace Socinator.DominatorCores
{
    public class DominatorTabHandlerFactory : ITabHandlerFactory
    {
        private AccessorStrategies _strategies;

        public List<TabItemTemplates> NetworkTabs { get; set; }

        private static DominatorTabHandlerFactory _instance;

        public static DominatorTabHandlerFactory GetInstance(AccessorStrategies strategies)
            => _instance ?? (_instance = new DominatorTabHandlerFactory(strategies));

        private DominatorTabHandlerFactory(AccessorStrategies strategies)
        {
            _strategies = strategies;
            NetworkTabs = new List<TabItemTemplates>();
            InitializeAllTabs();
        }

        public void UpdateAccountCustomControl(SocialNetworks networks)
        {
            //  NetworkTabs[0].Content = new Lazy<UserControl>(() => new AccountTab(_strategies));
            NetworkTabs[0].Content = new Lazy<UserControl>(() => AccountManager.GetSingletonAccountManager("AccountManager", null, SocialNetworks.Social));
        }

        private void InitializeAllTabs()
        {

            NetworkTabs = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccountsManager") == null? "Account Manager" : Application.Current.FindResource("LangKeyAccountsManager")?.ToString(),
                    Content = new Lazy<UserControl>(() => AccountManager.GetSingletonAccountManager("AccountManager",null,SocialNetworks.Social)),
                    ElementID="Account"

                },
                new TabItemTemplates
                {
                    Title="LangKeyAccountGrowth".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>  AccountGrowthControl.GetAccountGrowthControl(SocialNetworks.Social,_strategies)),
                     ElementID="AccountGrowth"
                },

                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDashboard") == null? "Dash Board" : Application.Current.FindResource("LangKeyDashboard")?.ToString(),
                   Content=new Lazy<UserControl>(()=> DominatorHouseCore.IoC.Container.Resolve<TablifiedContentControl>("Dashboard")),
                      ElementID="DashBoard"

                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccountsActivity") == null? "Accounts Activity" : Application.Current.FindResource("LangKeyAccountsActivity")?.ToString(),
                    Content = new Lazy<UserControl>(() => DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social)),
                     ElementID="AccountsActivity"
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("langPublisher") == null? "Publisher" : Application.Current.FindResource("langPublisher")?.ToString(),
                //    Content = new Lazy<UserControl>(()=> PublisherIndexPage.Instance)
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeySociopublisher") == null? "Socio Publisher" : Application.Current.FindResource("LangKeySociopublisher")?.ToString(),
                    Content = new Lazy<UserControl>(()=>PublisherHome.Instance),
                     ElementID="Sociopublisher"
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyProxyManager") == null? "Proxy Manager" : Application.Current.FindResource("LangKeyProxyManager")?.ToString(),
                    Content = new Lazy<UserControl>(() => ProxyManager.GetProxyManagerControl(_strategies))
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("langSettings") == null? "Settings" : Application.Current.FindResource("langSettings")?.ToString(),
                //    Content = new Lazy<UserControl>(() => new Social.Settings.View.Home())
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyOtherConfigurations") == null? "Other Configuration" : Application.Current.FindResource("LangKeyOtherConfigurations")?.ToString(),
                      Content=new Lazy<UserControl>(()=> DominatorHouseCore.IoC.Container.Resolve<TablifiedContentControl>("OtherConfiguration"))
                     
                },
                new TabItemTemplates
                {
                    Title = "LangKeyOtherTools".FromResourceDictionary(),
                    Content=new Lazy<UserControl>(()=> DominatorHouseCore.IoC.Container.Resolve<TablifiedContentControl>("OtherTools"))
                }
            };
        }




    }
}
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LegionUIUtility.CustomControl;
using LegionUIUtility.Views;
using LegionUIUtility.Views.SocioPublisher;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                    ImagePath= (Visual)Application.Current.FindResource("AccountManagerIcon") as Canvas
                },
                new TabItemTemplates
                {
                    Title="LangKeyAccountGrowth".FromResourceDictionary(),
                    Content = new Lazy<UserControl>(() =>  ServiceLocator.Current.GetInstance<AccountGrowthControl>()),
                     ImagePath= (Visual)Application.Current.FindResource("AccountGrowthIcon") as Canvas
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeyDashboard") == null? "Dash Board" : Application.Current.FindResource("LangKeyDashboard")?.ToString(),
                //   Content=new Lazy<UserControl>(()=> ServiceLocator.Current.GetInstance<TablifiedContentControl>("Dashboard")),
                //    ImagePath= (Visual)Application.Current.FindResource("DashBoardIcon") as Canvas
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyAccountsActivity") == null? "Accounts Activity" : Application.Current.FindResource("LangKeyAccountsActivity")?.ToString(),
                    Content = new Lazy<UserControl>(() => ServiceLocator.Current.GetInstance<DominatorAutoActivity>()),
                     ImagePath= (Visual)Application.Current.FindResource("AccountActivityIcon") as Canvas
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
                     ImagePath= (Visual)Application.Current.FindResource("SocioPublisherIcon") as Canvas
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyProxyManager") == null? "Proxy Manager" : Application.Current.FindResource("LangKeyProxyManager")?.ToString(),
                    Content = new Lazy<UserControl>(() => ServiceLocator.Current.GetInstance<ProxyManager>()),
                     ImagePath= (Visual)Application.Current.FindResource("ProxyManagerIcon") as Canvas
                },
                //new TabItemTemplates
                //{
                //    Title = Application.Current.FindResource("LangKeySettings") == null? "Settings" : Application.Current.FindResource("langSettings")?.ToString(),
                //    Content = new Lazy<UserControl>(() => new Home())
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyOtherConfigurations") == null? "Other Configuration" : Application.Current.FindResource("LangKeyOtherConfigurations")?.ToString(),
                    Content=new Lazy<UserControl>(()=> ServiceLocator.Current.GetInstance<TablifiedContentControl>("OtherConfiguration")),
                     ImagePath= (Visual)Application.Current.FindResource("SettingsIcon") as Canvas
                },
                new TabItemTemplates
                {
                    Title = "LangKeyOtherTools".FromResourceDictionary(),
                    Content=new Lazy<UserControl>(()=> ServiceLocator.Current.GetInstance<TablifiedContentControl>("OtherTools")),
                     ImagePath= (Visual)Application.Current.FindResource("OtherToolsIcon") as Canvas
                }
            };
        }




    }
}
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Windows;
using DominatorHouseCore.Diagnostics.Exceptions;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace DominatorHouseCore.Diagnostics
{
    public static class SocinatorInitialize
    {
        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, INetworkCollectionFactory> RegisteredNetworks { get; } =
            new Dictionary<SocialNetworks, INetworkCollectionFactory>();


        public static int MaximumAccountCount { get; set; } = 10000;

        public static HashSet<SocialNetworks> AvailableNetworks { get; set; } = new HashSet<SocialNetworks>();

        public static bool IsNetworkAvailable(SocialNetworks network)
            => AvailableNetworks.Contains(network);

        public static HashSet<SocinatorIntellisenseModel> Macros { get; set; } = new HashSet<SocinatorIntellisenseModel>();

        public static void InitializeMacros()
        {
            try
            {
                var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                var macros = genericFileManager.GetModuleDetails<SocinatorIntellisenseModel>(ConstantVariable.GetMacroDetails);
                Macros.Clear();
                macros?.ForEach(macro =>
                {
                    Macros.Add(new SocinatorIntellisenseModel { Key = @"{" + macro.Key + @"}", Value = macro.Value });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static Dictionary<SocialNetworks, string> NetworksNamespace { get; set; } = new Dictionary<SocialNetworks, string>()
        {
            {SocialNetworks.Social,ConfigurationManager.AppSettings["Social"] },
            {SocialNetworks.Facebook,ConfigurationManager.AppSettings["Facebook"]},
            {SocialNetworks.Twitter,ConfigurationManager.AppSettings["Twitter"]},
            {SocialNetworks.Gplus,ConfigurationManager.AppSettings["Gplus"]},
            {SocialNetworks.Instagram,ConfigurationManager.AppSettings["Instagram"]},
            {SocialNetworks.LinkedIn,ConfigurationManager.AppSettings["LinkedIn"]},
            {SocialNetworks.Quora,ConfigurationManager.AppSettings["Quora"]},
            {SocialNetworks.Pinterest,ConfigurationManager.AppSettings["Pinterest"]},
            {SocialNetworks.Tumblr,ConfigurationManager.AppSettings["Tumblr"]},
            {SocialNetworks.Youtube,ConfigurationManager.AppSettings["Youtube"]},
            {SocialNetworks.Reddit,ConfigurationManager.AppSettings["Reddit"]},
        };

        public static string GetNetworksNamespace(SocialNetworks networks)
        {
            var networksNamespace = NetworksNamespace[networks];

            return networksNamespace;
        }

        public static INetworkCollectionFactory ActiveNetwork { get; private set; }

        public static SocialNetworks ActiveSocialNetwork => GetActiveSocialNetwork();
        public static SocialNetworks AccountModeActiveSocialNetwork;

        private static SocialNetworks GetActiveSocialNetwork()
        {
            try
            {
                return ActiveNetwork.GetNetworkCoreFactory().Network;
            }
            catch (Exception)
            {
                return SocialNetworks.Social;
            }
        }

        public static INetworkCollectionFactory GetSocialLibrary(SocialNetworks networks)
        {
            return RegisteredNetworks.ContainsKey(networks) ? RegisteredNetworks[networks] : null;
        }

        public static void LogInitializer(Window mainWindow)
        {
            GlobalExceptionInitializer();
        }

        public static void SetAsActiveNetwork(SocialNetworks networks)
        {
            var activenetwork = RegisteredNetworks[networks];

            if (activenetwork != null)
            {
                ActiveNetwork = activenetwork;
            }
        }

        public static void SocialNetworkRegister(INetworkCollectionFactory networkCollectionFactory, SocialNetworks networks)
        {
            if (RegisteredNetworks.ContainsKey(networks))
                return;

            RegisteredNetworks.Add(networks, networkCollectionFactory);
        }

        public static IEnumerable<SocialNetworks> GetRegisterNetwork()
        {
            return RegisteredNetworks.Keys;
        }

        private static void GlobalExceptionInitializer()
        {
            if (_isInitialized)
                return;

            GlobusExceptionHandler.SetupGlobalExceptionHandlers();
            GlobusExceptionHandler.DisableErrorDialog();
            _isInitialized = true;
        }

        public static IGlobalDatabaseConnection GetGlobalDatabase()
        {
            return ServiceLocator.Current.GetInstance<IGlobalDatabaseConnection>();
        }

    }
}
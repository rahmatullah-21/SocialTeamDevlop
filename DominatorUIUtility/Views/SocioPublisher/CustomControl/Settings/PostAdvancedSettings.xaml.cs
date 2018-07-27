using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ConfigControl;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    /// Interaction logic for PostAdvancedSettings.xaml
    /// </summary>
    public partial class PostAdvancedSettings : UserControl
    {
        public PostAdvancedSettings()
        {
            InitializeComponent();
        }

        public PostAdvancedSettings(PublisherPostSettings PublisherPostSettings) : this()
        {
            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("LangKeyGeneralSettings").ToString(),
                    Content = new Lazy<UserControl>(() => new PostGeneralSettings(PublisherPostSettings))
                }              
            };

            var availableNetworks = SocinatorInitialize.AvailableNetworks;

            ////Todo: comment above on production and following initialize
            //var availableNetworks = new HashSet<SocialNetworks>
            //{
            //    SocialNetworks.Social,
            //    SocialNetworks.Twitter,
            //    SocialNetworks.Facebook,
            //    SocialNetworks.Gplus,
            //    SocialNetworks.Instagram,
            //    SocialNetworks.LinkedIn,
            //    SocialNetworks.Quora,
            //    SocialNetworks.Pinterest,
            //    SocialNetworks.Tumblr,
            //    SocialNetworks.Youtube,
            //    SocialNetworks.Reddit
            //};

            foreach (var network in availableNetworks)
            {
                switch (network)
                {
                    case SocialNetworks.Facebook:
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyFacebook").ToString(),
                            Content = new Lazy<UserControl>(() => new PostFacebookSettings(PublisherPostSettings))
                        });
                        break;
                    case SocialNetworks.Instagram:
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyInstagram").ToString(),
                            Content = new Lazy<UserControl>(() => new PostInstagramSettings(PublisherPostSettings))
                        });
                        break;
                    case SocialNetworks.Twitter:
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyTwitter").ToString(),
                            Content = new Lazy<UserControl>(() => new PostTwitterSettings(PublisherPostSettings))
                        });
                        break;
                    case SocialNetworks.LinkedIn:
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyLinkedIn").ToString(),
                            Content = new Lazy<UserControl>(() => new PostLinkedInSettings(PublisherPostSettings))
                        });
                        break;
                    case SocialNetworks.Tumblr:
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyTumblr").ToString(),
                            Content = new Lazy<UserControl>(() => new PostTumblrSettings(PublisherPostSettings))
                        });
                        break;
                  case SocialNetworks.Reddit:
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyReddit").ToString(),
                            Content = new Lazy<UserControl>(() => new PostRedditSettings(PublisherPostSettings))
                        });
                        break;
                    case SocialNetworks.Pinterest:
                    case SocialNetworks.Quora:
                    case SocialNetworks.Gplus:
                    case SocialNetworks.Youtube:
                        break;
                    default:
                        break;
                }
            }

            AdvancedPostSettings.ItemsSource = items;
        }
    }


}

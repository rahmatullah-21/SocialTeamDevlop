using CommonServiceLocator;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.Utility
{
    public interface ILockFileConfigProvider
    {
        R WithFile<T, R>(Func<string, R> act);

    }

    public class LockFileConfigProvider : ILockFileConfigProvider
    {

        private Dictionary<Type, Tuple<object, Func<string>>> __lockAndFileByType { get; set; }


        public LockFileConfigProvider()
        {

            var constantVariable = ServiceLocator.Current.GetInstance<IConstantVariable>();

            __lockAndFileByType = new Dictionary<Type, Tuple<object, Func<string>>>
            {
                {
                    typeof(CampaignDetails),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetIndexCampaignFile)
                },
                {
                    typeof(TemplateModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetTemplatesFile)
                },
                {
                    typeof(ProxyManagerModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetOtherProxyFile)
                },
                {
                    typeof(AddPostModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetOtherPostsFile)
                },
                {
                    typeof(Configuration),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetOtherConfigFile)
                },

                //Todo: Following line need to delete
                {
                    typeof(PublisherAccountDetails),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherFile)
                },

                {
                    typeof(PublisherPostlistModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherCreatePostlistFolder)
                },

                {
                    typeof(PublisherManageDestinationModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherDestinationsFile)
                },
                {
                    typeof(PublisherCreateDestinationModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherCreateDestinationsFolder)
                },
                {
                    typeof(PublisherPostlistSettingsModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherPostlistSettingsFile)
                },
                {
                    typeof(CampaignInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetConfigurationDir)
                },
                {
                    typeof(GlobalInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetConfigurationDir)
                },
                {
                    typeof(FacebookModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(GeneralModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(GooglePlusModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.InstagramModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.PinterestModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.TumblrModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.TwitterModel),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(object),
                    Tuple.Create(new object(), (Func<string>) constantVariable.GetIndexAccountFile)
                }
                ,
                {
                    typeof(ConfigFacebookModel),
                    Tuple.Create(new object(), (Func<string>)constantVariable.GetOtherFacebookSettingsFile)
                }
            };
        }

        /// <summary>
        /// Do something while locking the file that is the repository for the corresponding class
        /// </summary>
        /// <typeparam name="T">subject</typeparam>
        /// <typeparam name="R">return type</typeparam>
        /// <param name="act">action to perform</param>
        /// <returns>repeats the action returned value</returns>
        public R WithFile<T, R>(Func<string, R> act)
        {
            Tuple<object, Func<string>> typeConfig;
            // first, try the actual type
            if (!__lockAndFileByType.TryGetValue(typeof(T), out typeConfig))
            {
                // second, try to see if it's an assignable type
                var presentBaseClass = __lockAndFileByType.Keys.Except(new[] { typeof(object) }).FirstOrDefault(
                    candidateBase => candidateBase.IsAssignableFrom(typeof(T)));
                if (presentBaseClass == default(Type))
                {
                    presentBaseClass = typeof(object);
                }
                typeConfig = __lockAndFileByType[presentBaseClass];
            }
            lock (typeConfig.Item1)
            {
                return act(typeConfig.Item2());
            }
        }
    }
}

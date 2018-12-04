using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using FacebookModel = DominatorHouseCore.Models.FacebookModel;

namespace DominatorHouseCore.Utility
{
    public interface ILockFileConfigProvider
    {
        R WithFile<T, R>(Func<string, R> act);
    }

    public class LockFileConfigProvider : ILockFileConfigProvider
    {

        private Dictionary<Type, Tuple<object, Func<string>>> __lockAndFileByType =
            new Dictionary<Type, Tuple<object, Func<string>>>
            {
                {
                    typeof(CampaignDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexCampaignFile)
                },
                {
                    typeof(TemplateModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetTemplatesFile)
                },
                {
                    typeof(ProxyManagerModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherProxyFile)
                },
                {
                    typeof(AddPostModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherPostsFile)
                },
                {
                    typeof(Configuration),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherConfigFile)
                },

                //Todo: Following line need to delete
                {
                    typeof(PublisherAccountDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherFile)
                },

                {
                    typeof(PublisherPostlistModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreatePostlistFolder)
                },

                {
                    typeof(PublisherManageDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherDestinationsFile)
                },
                {
                    typeof(PublisherCreateDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreateDestinationsFolder)
                },
                {
                    typeof(PublisherPostlistSettingsModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherPostlistSettingsFile)
                },
                {
                    typeof(CampaignInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                },
                {
                    typeof(GlobalInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                },
                {
                    typeof(FacebookModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(GeneralModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(GooglePlusModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.InstagramModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.PinterestModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.TumblrModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(Models.Publisher.CampaignsAdvanceSetting.TwitterModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherOtherConfigDir)
                },
                {
                    typeof(object),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexAccountFile)
                }
            };

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
                var presentBaseClass = __lockAndFileByType.Keys.Except(new Type[] { typeof(object) }).FirstOrDefault(
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

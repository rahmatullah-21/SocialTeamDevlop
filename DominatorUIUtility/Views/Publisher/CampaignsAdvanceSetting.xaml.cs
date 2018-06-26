using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;
using FacebookModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.FacebookModel;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    /// Interaction logic for CampaignsAdvanceSetting.xaml
    /// </summary>
    public partial class CampaignsAdvanceSetting : UserControl
    {
        public CampaignsAdvanceSetting()
        {
            InitializeComponent();

            var TabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("langGeneral").ToString(),
                    Content=new Lazy<UserControl>(General.GetSingeltonGeneralObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langFacebook").ToString(),
                    Content=new Lazy<UserControl>(Facebook.GetSingeltonFacebookObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langGooglePlus").ToString(),
                    Content=new Lazy<UserControl>(GooglePlus.GetSingeltonGooglePlusObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langPinterest").ToString(),
                    Content=new Lazy<UserControl>(Pinterest.GetSingeltonPinterestObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langTwitter").ToString(),
                    Content=new Lazy<UserControl>(Twitter.GetSingletonTwitterObject)
                },
                new TabItemTemplates
                {
                    Title=FindResource("langInstagram").ToString(),
                    Content=new Lazy<UserControl>(Instagram.GetSingeltonInstagramObject)
                } ,
                new TabItemTemplates
                {
                    Title=FindResource("DHlangTumblr").ToString(),
                    Content=new Lazy<UserControl>(Tumblr.GetSingeltonTumblr)
                },
                new TabItemTemplates
                {
                    Title=FindResource("DHlangReddit").ToString(),
                    Content=new Lazy<UserControl>(Reddit.GetSingeltonRedditObject)
                }

            };
            CampaignsAdvanceSettingTab.ItemsSource = TabItems;
            AdvanceSetting = new AdvanceSetting();
        }

        private AdvanceSetting AdvanceSetting { get; set; }
        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            var campaignId = AdvanceSetting.CampaignId;

            #region General

            var oldGeneralModel = AdvanceSetting.GeneralModel;
            var newGeneralModel = General.GetSingeltonGeneralObject().GeneralViewModel.GeneralModel;
            newGeneralModel = ObjectComparer.CompareAndGetChangedObject<GeneralModel>(oldGeneralModel, newGeneralModel);
            if (newGeneralModel != null)
            {
                newGeneralModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social);
                var generalModels = GenericFileManager.GetModuleDetails<GeneralModel>(file);
                var moduleToUpdate = generalModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newGeneralModel, generalModels, file, SocialNetworks.Social);
            }

            #endregion

            #region FaceBook

            var oldFacebookModel = AdvanceSetting.FacebookModel;
            var newFacebookModel = Facebook.GetSingeltonFacebookObject().FacebookViewModel.FacebookModel;
            newFacebookModel = ObjectComparer.CompareAndGetChangedObject<FacebookModel>(oldFacebookModel, newFacebookModel);
            if (newFacebookModel != null)
            {
                newFacebookModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook);
                var lstFacebookModels = GenericFileManager.GetModuleDetails<FacebookModel>(file);
                var moduleToUpdate = lstFacebookModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newFacebookModel, lstFacebookModels, file, SocialNetworks.Facebook);
            }

            #endregion

            #region Google+

            var oldGooglePlusModel = AdvanceSetting.GooglePlusModel;
            var newGooglePlusModel = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel;
            newGooglePlusModel = ObjectComparer.CompareAndGetChangedObject<GooglePlusModel>(oldGooglePlusModel, newGooglePlusModel);

            if (newGooglePlusModel != null)
            {
                newGeneralModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Gplus);
                var lstGooglePlusModels = GenericFileManager.GetModuleDetails<GooglePlusModel>(file);
                var moduleToUpdate = lstGooglePlusModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newGooglePlusModel, lstGooglePlusModels, file, SocialNetworks.Gplus);
            }
            #endregion

            #region Instagram

            var oldInstagramModel = AdvanceSetting.InstagramModel;
            var newInstagramModel = Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel;
            newInstagramModel =
                ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel>(oldInstagramModel,
                    newInstagramModel);
            if (newInstagramModel != null)
            {
                newInstagramModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram);
                var lstInstagramModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel>(file);
                var moduleToUpdate = lstInstagramModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newInstagramModel, lstInstagramModels, file, SocialNetworks.Instagram);
            }

            #endregion

            #region Pinterest

            var oldPinterestModel = AdvanceSetting.PinterestModel;
            var newPinterestModel = Pinterest.GetSingeltonPinterestObject().PinterestViewModel.PinterestModel;
            newPinterestModel = ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel>(oldPinterestModel, newPinterestModel);

            if (newPinterestModel != null)
            {
                newPinterestModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest);
                var lstPinterestModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel>(file);
                var moduleToUpdate = lstPinterestModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newPinterestModel, lstPinterestModels, file, SocialNetworks.Pinterest);
            }
            #endregion

            #region Tumblr

            var oldTumblrModel = AdvanceSetting.TumblrModel;
            var newTumblrModel = Tumblr.GetSingeltonTumblr().TumblrViewModel.TumblrModel;
            newTumblrModel =
                ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel>(oldTumblrModel,
                    newTumblrModel);
            if (newTumblrModel != null)
            {
                newTumblrModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Tumblr);
                var lstTumblrModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel>(file);
                var moduleToUpdate = lstTumblrModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newTumblrModel, lstTumblrModels, file, SocialNetworks.Tumblr);
            }

            #endregion

            #region Twitter

            var oldTwitterModel = AdvanceSetting.TwitterModel;
            var newTwitterModel = Twitter.GetSingletonTwitterObject().TwitterViewModel.TwitterModel;
            newTwitterModel = ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel>(oldTwitterModel, newTwitterModel);

            if (newTwitterModel != null)
            {
                newTwitterModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Twitter);
                var lstTwitterModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel>(file);
                var moduleToUpdate = lstTwitterModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newTwitterModel, lstTwitterModels, file, SocialNetworks.Twitter);
            }

            #endregion

            #region Reddit

            var oldRedditModel = AdvanceSetting.RedditModel;
            var newRedditModel = Reddit.GetSingeltonRedditObject().RedditViewModel.RedditModel;
            newRedditModel = ObjectComparer.CompareAndGetChangedObject<RedditModel>(oldRedditModel, newRedditModel);

            if (newRedditModel != null)
            {
                newRedditModel.CampaignId = campaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Reddit);
                var lstRedditModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.RedditModel>(file);
                var moduleToUpdate = lstRedditModels.FirstOrDefault(x => x.CampaignId == campaignId);
                AddUpdateDetails(moduleToUpdate, newRedditModel, lstRedditModels, file, SocialNetworks.Reddit);
            }

            #endregion

            GlobusLogHelper.log.Info("Details successfully saved");
        }

        private void AddUpdateDetails<T>(T moduleToUpdate, T updatedModel,
            List<T> lstModels, string file, SocialNetworks networks) where T : class
        {
            if (moduleToUpdate == null)
                GenericFileManager.AddModule<T>(updatedModel,
                    ConstantVariable.GetPublisherOtherConfigFile(networks));
            else
            {
                var moduleToUpdateIndex = lstModels.IndexOf(moduleToUpdate);
                lstModels[moduleToUpdateIndex] = updatedModel;

                GenericFileManager.UpdateModuleDetails<T>(lstModels, file);
            }

        }
    }
}

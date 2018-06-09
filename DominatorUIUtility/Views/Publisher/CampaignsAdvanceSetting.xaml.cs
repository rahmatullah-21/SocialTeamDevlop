using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
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
                //new TabItemTemplates
                //{
                //    Title=FindResource("langErrorHandling").ToString(),
                //    Content=new Lazy<UserControl>(ErrorHandling.GetSingeltonErrorHandlingObject)
                //}

            };
            CampaignsAdvanceSettingTab.ItemsSource = TabItems;
            AdvanceSetting = new AdvanceSetting();
        }

        private AdvanceSetting AdvanceSetting { get; set; }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var CampaignId = AdvanceSetting.CampaignId;

            #region FaceBook

            var oldFacebookModel = AdvanceSetting.FacebookModel;
            var newFacebookModel = Facebook.GetSingeltonFacebookObject().FacebookViewModel.FacebookModel;
            newFacebookModel = ObjectComparer.CompareAndGetChangedObject<FacebookModel>(oldFacebookModel, newFacebookModel);
            if (newFacebookModel != null)
            {
                newFacebookModel.CampaignId = CampaignId;
                string file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook);
                var lstFacebookModels = GenericFileManager.GetPublisherOtherConfig<FacebookModel>(file);
                var moduleToUpdate = lstFacebookModels.FirstOrDefault(x => x.CampaignId == CampaignId);
                AddUpdateDetails(moduleToUpdate, newFacebookModel, lstFacebookModels, file, SocialNetworks.Facebook);
            } 

            #endregion

            //var oldGeneralModel = AdvanceSetting.GeneralModel;
            //var newGeneralModel = General.GetSingeltonGeneralObject().GeneralViewModel.GeneralModel;
            //oldGeneralModel = ObjectComparer.CompareAndGetChangedObject<GeneralModel>(oldGeneralModel, newGeneralModel);
            //if (oldGeneralModel != null)
            //{
            //    oldGeneralModel.CampaignId = AdvanceSetting.CampaignId;
            //    GenericFileManager.AddModule(oldGeneralModel, ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            //    GenericFileManager.GetModule<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            //    var data = GenericFileManager.GetModule<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            //}

            //var oldGooglePlusModel = AdvanceSetting.GooglePlusModel;
            //var newGooglePlusModel = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel;
            //oldGooglePlusModel = ObjectComparer.CompareAndGetChangedObject<GooglePlusModel>(oldGooglePlusModel, newGooglePlusModel);
            //if (oldGooglePlusModel != null)
            //{
            //    oldGooglePlusModel.CampaignId = AdvanceSetting.CampaignId;
            //    GenericFileManager.AddModule(oldGooglePlusModel, ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Gplus));
            //}
            //var oldInstagramModel = AdvanceSetting.InstagramModel;
            //var newInstagramModel = Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel;
            //oldInstagramModel =
            //    ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel>(oldInstagramModel,
            //        newInstagramModel);
            //if (oldInstagramModel != null)
            //{
            //    oldInstagramModel.CampaignId = AdvanceSetting.CampaignId;
            //    GenericFileManager.AddModule(oldInstagramModel, ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram));
            //}

            //var oldPinterestModel = AdvanceSetting.PinterestModel;
            //var newPinterestModel = Pinterest.GetSingeltonPinterestObject().PinterestViewModel.PinterestModel;
            //oldPinterestModel = ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel>(oldPinterestModel, newPinterestModel);
            //if (oldPinterestModel != null)
            //{
            //    oldPinterestModel.CampaignId = AdvanceSetting.CampaignId;
            //    GenericFileManager.AddModule(oldPinterestModel, ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest));
            //}

            //var oldTumblrModel = AdvanceSetting.TumblrModel;
            //var newTumblrModel = Tumblr.GetSingeltonTumblr().TumblrViewModel.TumblrModel;
            //oldTumblrModel =
            //    ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel>(oldTumblrModel,
            //        newTumblrModel);
            //if (oldTumblrModel != null)
            //{
            //    oldTumblrModel.CampaignId = AdvanceSetting.CampaignId;
            //    GenericFileManager.AddModule(oldTumblrModel, ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Tumblr));
            //}

            //var oldTwitterModel = AdvanceSetting.TwitterModel;
            //var newTwitterModel = Twitter.GetSingletonTwitterObject().TwitterViewModel.TwitterModel;
            //oldTwitterModel= ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel>(oldTwitterModel, newTwitterModel);
            //if (oldTwitterModel != null)
            //{
            //    oldTwitterModel.CampaignId = AdvanceSetting.CampaignId;
            //    GenericFileManager.AddModule(oldTwitterModel, ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Twitter));
            //}
        }

        private  void AddUpdateDetails<T>(T moduleToUpdate, T newFacebookModel,
            List<T> lstFacebookModels, string file, SocialNetworks networks) where T:class
        {
            if (moduleToUpdate == null)
                GenericFileManager.AddModule<T>(newFacebookModel,
                    ConstantVariable.GetPublisherOtherConfigFile(networks));
            else
            {
                lstFacebookModels.Remove(moduleToUpdate);
                lstFacebookModels.Add(newFacebookModel);

                GenericFileManager.SaveAll<T>(lstFacebookModels, file);
            }

            var data = GenericFileManager.GetPublisherOtherConfig<T>(
                ConstantVariable.GetPublisherOtherConfigFile(networks));
        }
    }
}

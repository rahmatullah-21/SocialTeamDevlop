using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.Publisher;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for OtherConfiguration.xaml
    /// </summary>
    public partial class OtherConfiguration : UserControl
    {
        public OtherConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }
        public OtherConfigurationModel OtherConfigurations
        {
            get { return (OtherConfigurationModel)GetValue(OtherConfigurationsProperty); }
            set { SetValue(OtherConfigurationsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunningTimes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OtherConfigurationsProperty =
            DependencyProperty.Register("OtherConfigurations", typeof(OtherConfigurationModel), typeof(OtherConfiguration), new FrameworkPropertyMetadata(OnAvailableItemsChanged)
            {
                BindsTwoWayByDefault = true
            });


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }

        private void btnAdvancedSettings_Click(object sender, RoutedEventArgs e)
        {
            CampaignsAdvanceSetting ObjCampaignsAdvanceSetting = new CampaignsAdvanceSetting();
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(ObjCampaignsAdvanceSetting, "Campaign - Advanced Settings");
        
            ObjCampaignsAdvanceSetting.BtnSave.Click += (senders, args) =>
            {
                try
                {
                    var campaignId = ObjCampaignsAdvanceSetting.AdvanceSetting.CampaignId;

                    #region General

                    var oldGeneralModel = ObjCampaignsAdvanceSetting.AdvanceSetting.GeneralModel;
                    var newGeneralModel = General.GetSingeltonGeneralObject().GeneralViewModel.GeneralModel;
                    newGeneralModel = ObjectComparer.CompareAndGetChangedObject<GeneralModel>(oldGeneralModel, newGeneralModel);
                    if (newGeneralModel != null)
                    {
                        newGeneralModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social);
                        var generalModels = GenericFileManager.GetModuleDetails<GeneralModel>(file);
                        var moduleToUpdate = generalModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newGeneralModel, generalModels, file, SocialNetworks.Social);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social,"Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }

                    #endregion

                    #region FaceBook

                    var oldFacebookModel = ObjCampaignsAdvanceSetting.AdvanceSetting.FacebookModel;
                    var newFacebookModel = Facebook.GetSingeltonFacebookObject().FacebookViewModel.FacebookModel;
                    newFacebookModel = ObjectComparer.CompareAndGetChangedObject<FacebookModel>(oldFacebookModel, newFacebookModel);
                    if (newFacebookModel != null)
                    {
                        newFacebookModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook);
                        var lstFacebookModels = GenericFileManager.GetModuleDetails<FacebookModel>(file);
                        var moduleToUpdate = lstFacebookModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newFacebookModel, lstFacebookModels, file, SocialNetworks.Facebook);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }

                    #endregion

                    #region Google+

                    var oldGooglePlusModel = ObjCampaignsAdvanceSetting.AdvanceSetting.GooglePlusModel;
                    var newGooglePlusModel = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel;
                    newGooglePlusModel = ObjectComparer.CompareAndGetChangedObject<GooglePlusModel>(oldGooglePlusModel, newGooglePlusModel);

                    if (newGooglePlusModel != null)
                    {
                        newGooglePlusModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Gplus);
                        var lstGooglePlusModels = GenericFileManager.GetModuleDetails<GooglePlusModel>(file);
                        var moduleToUpdate = lstGooglePlusModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newGooglePlusModel, lstGooglePlusModels, file, SocialNetworks.Gplus);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Gplus, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }
                    #endregion

                    #region Instagram

                    var oldInstagramModel = ObjCampaignsAdvanceSetting.AdvanceSetting.InstagramModel;
                    var newInstagramModel = Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel;
                    newInstagramModel =
                        ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel>(oldInstagramModel,
                            newInstagramModel);
                    if (newInstagramModel != null)
                    {
                        newInstagramModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram);
                        var lstInstagramModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.InstagramModel>(file);
                        var moduleToUpdate = lstInstagramModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newInstagramModel, lstInstagramModels, file, SocialNetworks.Instagram);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");
                    }

                    #endregion

                    #region Pinterest

                    var oldPinterestModel = ObjCampaignsAdvanceSetting.AdvanceSetting.PinterestModel;
                    var newPinterestModel = Pinterest.GetSingeltonPinterestObject().PinterestViewModel.PinterestModel;
                    newPinterestModel = ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel>(oldPinterestModel, newPinterestModel);

                    if (newPinterestModel != null)
                    {
                        newPinterestModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest);
                        var lstPinterestModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.PinterestModel>(file);
                        var moduleToUpdate = lstPinterestModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newPinterestModel, lstPinterestModels, file, SocialNetworks.Pinterest);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Pinterest, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }
                    #endregion

                    #region Tumblr

                    var oldTumblrModel = ObjCampaignsAdvanceSetting.AdvanceSetting.TumblrModel;
                    var newTumblrModel = Tumblr.GetSingeltonTumblr().TumblrViewModel.TumblrModel;
                    newTumblrModel =
                        ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel>(oldTumblrModel,
                            newTumblrModel);
                    if (newTumblrModel != null)
                    {
                        newTumblrModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Tumblr);
                        var lstTumblrModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TumblrModel>(file);
                        var moduleToUpdate = lstTumblrModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newTumblrModel, lstTumblrModels, file, SocialNetworks.Tumblr);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Tumblr, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }

                    #endregion

                    #region Twitter

                    var oldTwitterModel = ObjCampaignsAdvanceSetting.AdvanceSetting.TwitterModel;
                    var newTwitterModel = Twitter.GetSingletonTwitterObject().TwitterViewModel.TwitterModel;
                    newTwitterModel = ObjectComparer.CompareAndGetChangedObject<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel>(oldTwitterModel, newTwitterModel);

                    if (newTwitterModel != null)
                    {
                        newTwitterModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Twitter);
                        var lstTwitterModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.TwitterModel>(file);
                        var moduleToUpdate = lstTwitterModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newTwitterModel, lstTwitterModels, file, SocialNetworks.Twitter);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Twitter, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }

                    #endregion

                    #region Reddit

                    var oldRedditModel = ObjCampaignsAdvanceSetting.AdvanceSetting.RedditModel;
                    var newRedditModel = Reddit.GetSingeltonRedditObject().RedditViewModel.RedditModel;
                    newRedditModel = ObjectComparer.CompareAndGetChangedObject<RedditModel>(oldRedditModel, newRedditModel);

                    if (newRedditModel != null)
                    {
                        newRedditModel.CampaignId = campaignId;
                        var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Reddit);
                        var lstRedditModels = GenericFileManager.GetModuleDetails<DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting.RedditModel>(file);
                        var moduleToUpdate = lstRedditModels.FirstOrDefault(x => x.CampaignId == campaignId);
                        ObjCampaignsAdvanceSetting.AddUpdateDetails(moduleToUpdate, newRedditModel, lstRedditModels, file, SocialNetworks.Reddit);
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Reddit, "Publisher Campaign", "Campaign - Advanced settings", "Details successfully saved");

                    }

                    #endregion
                    
                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };
            window.ShowDialog();
        }
    }
}

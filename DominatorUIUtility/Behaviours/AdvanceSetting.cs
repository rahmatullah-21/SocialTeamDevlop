using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorUIUtility.ViewModel.SocioPublisher;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;
using ProtoBuf;

namespace DominatorUIUtility.Behaviours
{

    public class AdvanceSetting
    {
        public string CampaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
            .PublisherCreateCampaignModel.CampaignId;
        public FacebookModel FacebookModel { get; set; } = Facebook.GetSingeltonFacebookObject().FacebookViewModel.FacebookModel.Clone();

        public GeneralModel GeneralModel { get; set; } = General.GetSingeltonGeneralObject().GeneralViewModel.GeneralModel.Clone();

        public GooglePlusModel GooglePlusModel { get; set; } = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel.Clone();

        public InstagramModel InstagramModel { get; set; } = Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel.Clone();

        public PinterestModel PinterestModel { get; set; } = Pinterest.GetSingeltonPinterestObject().PinterestViewModel.PinterestModel.Clone();

        public TumblrModel TumblrModel { get; set; } = Tumblr.GetSingeltonTumblr().TumblrViewModel.TumblrModel.Clone();

        public TwitterModel TwitterModel { get; set; } = Twitter.GetSingletonTwitterObject().TwitterViewModel.TwitterModel.Clone();
    }
}

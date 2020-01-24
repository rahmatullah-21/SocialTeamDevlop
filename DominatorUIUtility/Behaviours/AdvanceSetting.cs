using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using LegionUIUtility.Views.Publisher.AdvancedSettings;
using LegionUIUtility.Views.SocioPublisher;

namespace LegionUIUtility.Behaviours
{

    public class AdvanceSetting
    {
        public AdvanceSetting()
        {
            GeneralModel = new GeneralModel();
            GeneralModel.InitializeGeneralModel();
        }
        
        public string CampaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
            .PublisherCreateCampaignModel.CampaignId;
        public FacebookModel FacebookModel { get; set; } = Facebook.GetSingeltonFacebookObject().FacebookViewModel.FacebookModel.Clone();

        public GeneralModel GeneralModel { get; set; } 

      //  public GooglePlusModel GooglePlusModel { get; set; } = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel.Clone();

        public InstagramModel InstagramModel { get; set; } = Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel.Clone();

        public PinterestModel PinterestModel { get; set; } = Pinterest.GetSingeltonPinterestObject().PinterestViewModel.PinterestModel.Clone();

        public TumblrModel TumblrModel { get; set; } = Tumblr.GetSingeltonTumblr().TumblrViewModel.TumblrModel.Clone();

        public TwitterModel TwitterModel { get; set; } = Twitter.GetSingletonTwitterObject().TwitterViewModel.TwitterModel.Clone();

        public RedditModel RedditModel { get; set; } =
            Reddit.GetSingeltonRedditObject().RedditViewModel.RedditModel.Clone();

    }
}

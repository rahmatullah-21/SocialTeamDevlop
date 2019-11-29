using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Behaviours
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
   
        public GeneralModel GeneralModel { get; set; } 

      //  public GooglePlusModel GooglePlusModel { get; set; } = GooglePlus.GetSingeltonGooglePlusObject().GooglePlusViewModel.GooglePlusModel.Clone();

        public InstagramModel InstagramModel { get; set; } = Instagram.GetSingeltonInstagramObject().InstagramViewModel.InstagramModel.Clone();

      
    }
}

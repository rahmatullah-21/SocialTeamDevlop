using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using ConstantVariable = DominatorHouseCore.Utility.ConstantVariable;

namespace DominatorHouseCore.Diagnostics
{
    public class PublisherInitialize
    {
        private PublisherInitialize()
        {
            PublishCampaignInitializer();
        }

        #region Properties

        private static PublisherInitialize _publisherInitialize;

        public static PublisherInitialize Instance 
        => _publisherInitialize ?? (_publisherInitialize = new PublisherInitialize());

        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels { get; set; } = new ObservableCollection<PublisherCampaignStatusModel>();

        #endregion

        private void PublishCampaignInitializer()
        {
            var allCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            allCampaign.ForEach(campaigns =>
            {
                var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                {
                    CampaignName = campaigns.CampaignName,
                    CampaignId = campaigns.CampaignId,
                    StartDate = campaigns.JobConfigurations.CampaignStartDate,
                    EndDate = campaigns.JobConfigurations.CampaignEndDate,
                    CreatedDate = campaigns.CreatedDate,
                    Status = campaigns.CampaignStatus,
                    DestinationCount = campaigns.LstDestinationId.Count,
                    IsRotateDayChecked = campaigns.JobConfigurations.IsRotateDayChecked,
                    TimeRange = campaigns.JobConfigurations.TimeRange,
                    SpecificRunningTime = campaigns.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                    ScheduledWeekday = campaigns.JobConfigurations.Weekday
                };

                ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
               
            });
        }
    }

   
}
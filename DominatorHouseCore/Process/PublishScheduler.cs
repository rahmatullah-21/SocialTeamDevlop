using System;
using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;

namespace DominatorHouseCore.Process
{
    public class PublishScheduler
    {
        public static void StartPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            try
            {
                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);

                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                    destinationDetails.AccountsWithNetwork.ForEach(networkWithAccount =>
                    {
                        var selectedGroupDestinations =
                            destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value).Select(x=> x.Value).ToList();

                        var selectedPageOrBoardDestinations =
                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var isPublishOnOwnWall =
                            destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, isPublishOnOwnWall);

                        if (campaignStatusModel.IsRunSingleAccountPerCampaign)
                        {
                            publisherJobProcess.StartPublishing(true);
                            //publisherJobProcess.StartPublish with synchronously
                        }
                        else
                        {
                            publisherJobProcess.StartPublishing(false);
                            //publisherJobProcess.StartPublish with Asynchronously
                        }
                    });
                });             
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void StopPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            // Todo : Stop publish
        }

        public static void ScheduleTodaysPublisherByCampaign(string campaignId)
        {
            // get the all campaigns which should be present in between 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            if (specificCampaign != null)
            {
                if (specificCampaign.IsRotateDayChecked)
                    SchedulePublisher(specificCampaign);
                else
                {
                    var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    SchedulePublisher(specificCampaign);
                }
            }
        }

        public static void ScheduleTodaysPublisher()
        {
            // get the all campaigns which should be present in between 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            campaignDetails.ForEach(campaign =>
            {
                if (campaign.IsRotateDayChecked)                
                    SchedulePublisher(campaign);              
                else
                {
                    var isCampaignSelected = campaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    SchedulePublisher(campaign);
                }
            });
        }

        private static void SchedulePublisher(PublisherCampaignStatusModel campaign)
        {
            StartPublishingPosts(campaign);
            // Todo : Need to uncomment following region in production
            #region Schedule

            //campaign.SpecificRunningTime.ForEach(runningTime =>
            //{
            //    var startTime = DateTime.Today.Add(new TimeSpan(runningTime.Hours, runningTime.Minutes, runningTime.Seconds));
            //    if (startTime > DateTime.Now)
            //    {
            //        JobManager.AddJob(() =>
            //        {
            //            StartPublishingPosts(campaign);
            //        }, s => s.WithName($"{campaign.CampaignId}-{ConstantVariable.GetDate()}").ToRunOnceAt(startTime));
            //    }
            //});

            #endregion
        }
    }
}
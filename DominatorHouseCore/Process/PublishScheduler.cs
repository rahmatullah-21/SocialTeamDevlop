using System;
using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using FluentScheduler;

namespace DominatorHouseCore.Process
{
    public class PublishScheduler
    {
        public static void StartPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            try
            {
                // Todo : Start publish
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

        public static void RevokeSchedulePublisher()
        {

        }




       
    }
}
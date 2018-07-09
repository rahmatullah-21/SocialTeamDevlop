using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using ConstantVariable = DominatorHouseCore.Utility.ConstantVariable;

namespace DominatorHouseCore.Diagnostics
{

    public class PublisherInitialize
    {
        private PublisherInitialize()
        { }

        #region Properties

        private static Dictionary<SocialNetworks, IPublisherCollectionFactory> NetworkWisePublishers { get; } =
            new Dictionary<SocialNetworks, IPublisherCollectionFactory>();

        private static PublisherInitialize _publisherInitialize;

        public static PublisherInitialize GetInstance => _publisherInitialize ?? (_publisherInitialize = new PublisherInitialize());

        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels { get; set; } = new ObservableCollection<PublisherCampaignStatusModel>();

        #endregion

        public static void SaveNetworkPublisher(IPublisherCollectionFactory publisherCollectionFactory, SocialNetworks networks)
        {
            if (NetworkWisePublishers.ContainsKey(networks))
                return;

            NetworkWisePublishers.Add(networks, publisherCollectionFactory);
        }

        public static IPublisherCollectionFactory GetPublisherLibrary(SocialNetworks networks)
        {
            return NetworkWisePublishers.ContainsKey(networks) ? NetworkWisePublishers[networks] : null;
        }

        public ObservableCollection<PublisherCampaignStatusModel> GetSavedCampaigns()
            => ListPublisherCampaignStatusModels;

        public void PublishCampaignInitializer()
        {
            var allCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            if (!Application.Current.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    allCampaign.ForEach(campaigns =>
                    {
                        var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                        {
                            CampaignName = campaigns.CampaignName,
                            CampaignId = campaigns.CampaignId,
                            StartDate = campaigns.JobConfigurations.CampaignStartDate,
                            EndDate = campaigns.JobConfigurations.CampaignEndDate,
                            CreatedDate = campaigns.CreatedDate,
                            Status = DateTime.Now < campaigns.JobConfigurations.CampaignEndDate ? campaigns.CampaignStatus : PublisherCampaignStatus.Completed,
                            DestinationCount = campaigns.LstDestinationId.Count,
                            IsRotateDayChecked = campaigns.JobConfigurations.IsRotateDayChecked,
                            TimeRange = campaigns.JobConfigurations.TimeRange,
                            SpecificRunningTime = campaigns.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                            ScheduledWeekday = campaigns.JobConfigurations.Weekday,                            
                            IsTakeRandomDestination = !campaigns.JobConfigurations.IsPublishPostOnDestinationsChecked,
                            TotalRandomDestination = campaigns.JobConfigurations.RandomDestinationCount,
                            MinRandomDestinationPerAccount = campaigns.JobConfigurations.PostBetween.EndValue,                          
                        };

                        ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);

                        GetPostStatus(publisherCampaignStatusModel);

                        if (DateTime.Now > campaigns.JobConfigurations.CampaignEndDate)
                            UpdateCampaignStatus(campaigns.CampaignId, PublisherCampaignStatus.Completed);

                    });
                });
            }
            else
            {
                allCampaign.ForEach(campaigns =>
                {
                    var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                    {
                        CampaignName = campaigns.CampaignName,
                        CampaignId = campaigns.CampaignId,
                        StartDate = campaigns.JobConfigurations.CampaignStartDate,
                        EndDate = campaigns.JobConfigurations.CampaignEndDate,
                        CreatedDate = campaigns.CreatedDate,
                        Status = DateTime.Now < campaigns.JobConfigurations.CampaignEndDate ? campaigns.CampaignStatus : PublisherCampaignStatus.Completed,
                        DestinationCount = campaigns.LstDestinationId.Count,
                        IsRotateDayChecked = campaigns.JobConfigurations.IsRotateDayChecked,
                        TimeRange = campaigns.JobConfigurations.TimeRange,
                        SpecificRunningTime = campaigns.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                        ScheduledWeekday = campaigns.JobConfigurations.Weekday,                       
                        IsTakeRandomDestination = campaigns.JobConfigurations.IsPublishPostOnDestinationsChecked,
                        TotalRandomDestination = campaigns.JobConfigurations.RandomDestinationCount,
                        MinRandomDestinationPerAccount = campaigns.JobConfigurations.PostBetween.EndValue,                     
                    };

                    GetPostStatus(publisherCampaignStatusModel);

                    ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);

                    if (DateTime.Now > campaigns.JobConfigurations.CampaignEndDate)
                        UpdateCampaignStatus(campaigns.CampaignId, PublisherCampaignStatus.Completed);
                });
            }
        }

        public void UpdateCampaignStatus(string campaignId, PublisherCampaignStatus status)
        {
            var campaignItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignId == campaignId);

            if (campaignItem == null)
                return;

            var currentCampaignIndex = ListPublisherCampaignStatusModels.IndexOf(campaignItem);

            ListPublisherCampaignStatusModels[currentCampaignIndex].Status = status;

            var allCampaign = GenericFileManager
                  .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            var currentCampaign = allCampaign.FirstOrDefault(x => x.CampaignId == campaignId);

            if (currentCampaign == null)
                return;

            var campaignIndex = allCampaign.IndexOf(currentCampaign);
            currentCampaign.CampaignStatus = status;
            allCampaign[campaignIndex] = currentCampaign;
            GenericFileManager.UpdateModuleDetails(allCampaign, ConstantVariable.GetPublisherCampaignFile());
        }

        public void UpdatePostStatus(string campaignId)
        {
            var campaignItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignId == campaignId);

            if (campaignItem == null)
                return;

            var currentCampaignIndex = ListPublisherCampaignStatusModels.IndexOf(campaignItem);

            GetPostStatus(ListPublisherCampaignStatusModels[currentCampaignIndex]);

        }

        public void GetPostStatus(PublisherCampaignStatusModel publisherCampaignStatusModel)
        {
            var postdetails = PostlistFileManager.GetAll(publisherCampaignStatusModel.CampaignId);

            publisherCampaignStatusModel.PendingCount =
                postdetails.Count(x => x.PostQueuedStatus == PostQueuedStatus.Pending);

            publisherCampaignStatusModel.PublishedCount =
                postdetails.Count(x => x.PostQueuedStatus == PostQueuedStatus.Published);

            publisherCampaignStatusModel.DraftCount =
                postdetails.Count(x => x.PostQueuedStatus == PostQueuedStatus.Draft);

        }

        public bool AddCampaignDetails(PublisherCampaignStatusModel publisherCampaignStatusModel)
        {

            if (ListPublisherCampaignStatusModels.Any(x => x.CampaignName == publisherCampaignStatusModel.CampaignName))
            {
                var currentItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignName == publisherCampaignStatusModel.CampaignName);

                var index = ListPublisherCampaignStatusModels.IndexOf(currentItem);

                GetPostStatus(publisherCampaignStatusModel);

                ListPublisherCampaignStatusModels[index] = publisherCampaignStatusModel;

                return true;
            }

            if (publisherCampaignStatusModel.ValidDateTime())
            {
                try
                {
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            GetPostStatus(publisherCampaignStatusModel);
                            ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
                        });
                    else
                    {
                        GetPostStatus(publisherCampaignStatusModel);
                        ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
                    }

                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void UpdateNewGroups(string destinationId)
        {
            var objPublisherCreateDestinationModel = new PublisherCreateDestinationModel();
            objPublisherCreateDestinationModel.UpdateNewGroup(destinationId);
        }

        public static void RemoveGroupsFromDestination(string destinationId, string accountId, SocialNetworks network, string groupUrl)
        {
            var objPublisherCreateDestinationModel = new PublisherCreateDestinationModel();
            objPublisherCreateDestinationModel.RemoveGroupsFromDestination(destinationId, accountId, network, groupUrl);
        }

    }

    public class PublisherCoreLibraryBuilder
    {
        public PublisherCoreLibraryBuilder()
        {
        }
        public PublisherCoreLibraryBuilder(IPublisherCoreFactory publisherCoreFactory)
        {
            PublisherCoreFactory = publisherCoreFactory;
        }

        public IPublisherCoreFactory PublisherCoreFactory { get; set; }

        public PublisherCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            PublisherCoreFactory.Network = networks;
            return this;
        }

        public PublisherCoreLibraryBuilder AddPublisherJobFactory(IPublisherJobProcessFactory jobFactory)
        {
            PublisherCoreFactory.PublisherJobFactory = jobFactory;
            return this;
        }

        public PublisherCoreLibraryBuilder AddPostScraper(IPublisherPostScraper postScraper)
        {
            PublisherCoreFactory.PostScraper = postScraper;
            return this;
        }
    }

}
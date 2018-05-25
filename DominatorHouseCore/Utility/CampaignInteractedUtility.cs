using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Utility
{
    public class CampaignInteractedUtility
    {

        public SocialNetworks SocialNetworks { get; set; }

        public CampaignInteractedUtility(SocialNetworks networks)
        {
            SocialNetworks = networks;
        }

        #region Unique Campaign Interaction functionality


        private static readonly object Synclock = new object();

        public CampaignInteractionDataModel CampaignInteractionDataModel { get; set; } = new CampaignInteractionDataModel();

        public CampaignInteractionDataModel GetInteractedData(string campaignId)
        {
            try
            {

                CampaignInteractionDataModel = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory()
                               .CampaignInteractionDetails.CampaignInteractedCollections[campaignId] ??
                           new CampaignInteractionDataModel();
                return CampaignInteractionDataModel;
            }
            catch
            {
                return new CampaignInteractionDataModel();
            }
        }


        public void CheckAndAddInteractedData(string campaignId, string interactedData)
        {
            try
            {
                lock (Synclock)
                {

                    if (CampaignInteractionDataModel.InteractedData.Count == 0)
                    {
                        var hashsetValue = new SortedList<string, DateTime> { { interactedData, DateTime.Now } };

                        var collections = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().CampaignInteractionDetails.CampaignInteractedCollections;

                        if (collections.ContainsKey(campaignId))
                        {
                            collections[campaignId].InteractedData.Add(interactedData, DateTime.Now);
                        }
                        else
                        {
                            collections.Add(campaignId,
                                new CampaignInteractionDataModel
                                {
                                    CampaignId = campaignId,
                                    InteractedData = hashsetValue
                                });
                        }

                    }
                    else
                    {
                        if (CampaignInteractionDataModel.InteractedData.All(x => x.Key != interactedData))
                            CampaignInteractionDataModel.InteractedData.Add(interactedData, DateTime.Now);
                    }
                    SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory()
                        .CampaignInteractionDetails.UpdateInteractedData();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //public bool CheckAndAddInteractedData(string campaignId, string interactedData)
        //{
        //    try
        //    {
        //        lock (Synclock)
        //        {
        //            return IsInteractedDataAvailable(campaignId, interactedData) &&
        //                   AddCurrentInteractedData(campaignId, interactedData);
        //        }
        //    }
        //    catch
        //    {

using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.Utility
{
    public class CampaignInteractedUtility
    {

        private readonly ICampaignInteractionDetails _campaignInteractionDetails;
        public SocialNetworks SocialNetworks { get; set; }

        public CampaignInteractedUtility(SocialNetworks networks)
        {
            _campaignInteractionDetails = ServiceLocator.Current.GetInstance<ICampaignInteractionDetails>();
            SocialNetworks = networks;
        }

        #region Unique Campaign Interaction functionality


        private static readonly object Synclock = new object();

        public CampaignInteractionDataModel CampaignInteractionDataModel { get; set; } = new CampaignInteractionDataModel();

        public CampaignInteractionDataModel GetInteractedData(string campaignId)
        {
            try
            {
                lock (Synclock)
                    return _campaignInteractionDetails.CampaignInteractedCollections[campaignId] ?? new CampaignInteractionDataModel();
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

                        var collections = _campaignInteractionDetails.CampaignInteractedCollections;

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
                        //if (CampaignInteractionDataModel.InteractedData.All(x => x.Key != interactedData))
                        CampaignInteractionDataModel.InteractedData.Add(interactedData, DateTime.Now);
                    }
                    _campaignInteractionDetails.UpdateInteractedData(SocialNetworks);

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
        //        return false;
        //    }
        //}


        private bool IsInteractedDataAvailable(string campaignId, string interactedData)
        {
            try
            {
                CampaignInteractionDataModel = GetInteractedData(campaignId);
                return CampaignInteractionDataModel.InteractedData.ContainsKey(interactedData);
            }
            catch
            {
                return false;
            }
        }


        public void RemoveInteractedData(string campaignId, string interactedData)
        {
            lock (Synclock)
            {
                try
                {
                    if (IsInteractedDataAvailable(campaignId, interactedData))
                    {
                        CampaignInteractionDataModel.InteractedData.Remove(interactedData);
                        _campaignInteractionDetails.UpdateInteractedData(SocialNetworks);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }
        }

        #endregion
    }
}
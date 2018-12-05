using System;
using System.Collections.Generic;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Utility
{
    public class GlobalInteractedUtility
    {

        public SocialNetworks SocialNetworks { get; set; }

        public GlobalInteractedUtility(SocialNetworks networks)
        {
            SocialNetworks = networks;
        }

        #region Unique Global Interaction functionality


        private static readonly object Synclock = new object();

        public GlobalInteractionDataModel GlobalInteractionDataModel { get; set; } = new GlobalInteractionDataModel();

        public GlobalInteractionDataModel GetInteractedData(ActivityType activityType)
        {
            try
            {

                GlobalInteractionDataModel = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory()
                                                 .GlobalInteractionDetails.GlobalInteractedCollections[activityType] ??
                                             new GlobalInteractionDataModel();
                return GlobalInteractionDataModel;
            }
            catch
            {
                return new GlobalInteractionDataModel();
            }
        }

        public void CheckAndAddInteractedData(string campaignId, string interactedData, ActivityType activityType)
        {
            lock (Synclock)
                {

                    if (GlobalInteractionDataModel.InteractedData.Count == 0)
                    {
                        var hashsetValue = new SortedList<string, DateTime> { { interactedData, DateTime.Now } };

                        var collections = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().GlobalInteractionDetails.GlobalInteractedCollections;

                        if (collections.ContainsKey(activityType))
                        {
                            collections[activityType].InteractedData.Add(interactedData, DateTime.Now);
                        }
                        else
                        {
                            collections.Add(activityType,
                                new GlobalInteractionDataModel
                                {
                                    InteractedData = hashsetValue
                                });
                        }

                    }
                    else
                    {
                        //if (GlobalInteractionDataModel.InteractedData.All(x => x.Key != interactedData))
                            GlobalInteractionDataModel.InteractedData.Add(interactedData, DateTime.Now);
                    }
                    SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory()
                        .GlobalInteractionDetails.UpdateInteractedData();

                }
        }

        private bool IsInteractedDataAvailable(ActivityType activityType, string interactedData)
        {
            try
            {
                GlobalInteractionDataModel = GetInteractedData(activityType);
                return GlobalInteractionDataModel.InteractedData.ContainsKey(interactedData);
            }
            catch
            {
                return false;
            }
        }


        public void RemoveInteractedData(ActivityType activityType, string interactedData)
        {
            lock (Synclock)
            {
                try
                {
                    if (IsInteractedDataAvailable(activityType, interactedData))
                    {
                        GlobalInteractionDataModel.InteractedData.Remove(interactedData);
                        SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().GlobalInteractionDetails.UpdateInteractedData();
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
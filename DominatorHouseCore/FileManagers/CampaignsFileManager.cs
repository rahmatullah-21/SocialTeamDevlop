using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public class CampaignsFileManager
    {
        // Updates Campaigns with applying action to it and writes changes back to file
        public static void ApplyAction(Action<CampaignDetails> actionToApply)
        {
            var campaigns = BinFileHelper.GetCampaignDetail().ToList();

            foreach (var c in campaigns)
                actionToApply(c);

            BinFileHelper.UpdateCampaigns(campaigns);
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public static void ApplyFunc(Func<CampaignDetails, bool> funcToApply)
        {
            var campaigns = BinFileHelper.GetCampaignDetail().ToList();
            bool updated = false;

            foreach (var c in campaigns)
                updated |= funcToApply(c);

            BinFileHelper.UpdateCampaigns(campaigns);
        }


        public static void DeleteSelectedAccount(string templateId, string accountName)
        {
            ApplyAction(campaign =>
            {
                if (campaign.TemplateId == templateId)
                    campaign.SelectedAccountList.Remove(accountName);
            });
        }
    }
}

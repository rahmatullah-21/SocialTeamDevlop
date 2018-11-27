using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{

    public interface ICampaignsFileManager : IEnumerable<CampaignDetails>
    {
        void ApplyAction(Action<CampaignDetails> actionToApply);
        void ApplyFunc(Func<CampaignDetails, bool> funcToApply);
        void DeleteSelectedAccount(string templateId, string accountName);
        CampaignDetails GetCampaignById(string id);
        void UpdateCampaigns(IList<CampaignDetails> libraryCampaign);
        void Add(CampaignDetails campaign);
        void Delete(CampaignDetails campaign);
        void Edit(CampaignDetails campaign);
        List<CampaignDetails> GetCampaignByNetwork(SocialNetworks network);
    }

    public class CampaignsFileManager : ICampaignsFileManager
    {
        private readonly List<CampaignDetails> _campaignDetailses;

        public CampaignsFileManager()
        {
            _campaignDetailses = BinFileHelper.GetCampaignDetail();
        }
        // Updates Campaigns with applying action to it and writes changes back to file
        public void ApplyAction(Action<CampaignDetails> actionToApply)
        {
            foreach (var c in _campaignDetailses)
                actionToApply(c);

            BinFileHelper.UpdateCampaigns(_campaignDetailses);
        }

        // Same as above, but Func must return true if file needs to be overwritten        
        public void ApplyFunc(Func<CampaignDetails, bool> funcToApply)
        {
            bool updated = false;

            foreach (var c in _campaignDetailses)
                updated |= funcToApply(c);

            BinFileHelper.UpdateCampaigns(_campaignDetailses);
        }


        public void DeleteSelectedAccount(string templateId, string accountName)
        {
            ApplyAction(campaign =>
            {
                if (campaign.TemplateId == templateId)
                    campaign.SelectedAccountList.Remove(accountName);
            });
        }

        public CampaignDetails GetCampaignById(string id)
        {
            return _campaignDetailses.FirstOrDefault(x => x.CampaignId == id);
        }

        public void UpdateCampaigns(IList<CampaignDetails> libraryCampaign)
        {
            var all = _campaignDetailses;

            // Update all entries that exists in libraryAccount, and add that does not exists
            for (int i = 0; i < libraryCampaign.Count; i++)
            {
                var campaign = libraryCampaign[i];
                var ix = all.FindIndex(a => campaign.CampaignId == a.CampaignId);
                if (ix == -1)
                    all.Add(campaign);
                else
                    all[ix] = campaign;
            }

            BinFileHelper.UpdateCampaigns(all);
        }

        public void Add(CampaignDetails campaign)
        {
            _campaignDetailses.Add(campaign);
            BinFileHelper.Append(campaign);
        }

        // finds by id and delete
        public void Delete(CampaignDetails campaign)
        {
            CampaignDetails toDelete = _campaignDetailses.FirstOrDefault(c => c.CampaignId == campaign.CampaignId);
            if (toDelete != null)
            {
                _campaignDetailses.Remove(toDelete);
                Save(_campaignDetailses);
            }
        }

        public void Edit(CampaignDetails campaign)
        {
            var index = _campaignDetailses.FindIndex(c => c.CampaignId == campaign.CampaignId);
            if (index != -1)
            {
                _campaignDetailses[index] = campaign;
                Save(_campaignDetailses);
            }
        }

        public List<CampaignDetails> GetCampaignByNetwork(SocialNetworks network)
        {
            return _campaignDetailses.Where(x => x.SocialNetworks == network).ToList();
        }

        public IEnumerator<CampaignDetails> GetEnumerator()
        {
            return _campaignDetailses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Save(List<CampaignDetails> campaigns)
        {
            BinFileHelper.UpdateCampaigns(campaigns);
        }
    }
}

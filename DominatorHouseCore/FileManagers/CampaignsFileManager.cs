using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{

    public interface ICampaignsFileManager : IEnumerable<CampaignDetails>
    {
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
        private readonly IBinFileHelper _binFileHelper;

        public CampaignsFileManager(IBinFileHelper binFileHelper)
        {
            _binFileHelper = binFileHelper;
            _campaignDetailses = _binFileHelper.GetCampaignDetail();
        }

        public void DeleteSelectedAccount(string templateId, string accountName)
        {
            this.ForEach(campaign =>
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
            foreach (var campaign in libraryCampaign)
            {
                var ix = all.FindIndex(a => campaign.CampaignId == a.CampaignId);
                if (ix == -1)
                    all.Add(campaign);
                else
                    all[ix] = campaign;
            }

            _binFileHelper.UpdateCampaigns(all);
        }

        public void Add(CampaignDetails campaign)
        {
            _campaignDetailses.Add(campaign);
            _binFileHelper.Append(campaign);
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
            _binFileHelper.UpdateCampaigns(campaigns);
        }
    }
}

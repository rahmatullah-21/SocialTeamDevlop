using CommonServiceLocator;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.FileManagers
{
    public static class PostListSettingsFileManager
    {
        private static readonly IBinFileHelper BinFileHelper;

        static PostListSettingsFileManager()
        {
            BinFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
        }

        internal static void SaveAll(List<PublisherPostlistSettingsModel> lstPublisherSettings)
        {
            // Warning: make sure lstPublisherSettings contains all settings            
            BinFileHelper.UpdateAllPostListSettings(lstPublisherSettings);
        }

        public static bool Add(PublisherPostlistSettingsModel settings)
        {
            var lst = GetAll() ?? new List<PublisherPostlistSettingsModel>();
            lst.Add(settings);
            BinFileHelper.UpdateAllPostListSettings(lst);
            return true;
        }

        public static void AddOrUpdateDestinations(PublisherPostlistSettingsModel settings)
        {
            var all = BinFileHelper.GetPublisherPostListSettingsModels() ?? new List<PublisherPostlistSettingsModel>();

            var ix = all.FindIndex(a => settings.CampaignId == a.CampaignId);
            if (ix == -1)
                all.Add(settings);
            else
                all[ix] = settings;

            BinFileHelper.UpdateAllPostListSettings(all);
        }


        public static List<PublisherPostlistSettingsModel> GetAll() => BinFileHelper.GetPublisherPostListSettingsModels();


        public static PublisherPostlistSettingsModel GetSettingsByCampaignId(string campaignId)
        {
            var lst = GetAll();
            return lst.FirstOrDefault(x => x.CampaignId == campaignId);
        }


        public static void DeleteSelected(List<PublisherPostlistSettingsModel> settings)
        {
            var all = GetAll().Where(a => settings.FirstOrDefault(p => p.CampaignId == a.CampaignId) == null).ToList();
            SaveAll(all);
        }

        public static void Delete(Predicate<PublisherPostlistSettingsModel> match)
        {
            var accs = GetAll();
            accs.RemoveAll(match);
            BinFileHelper.UpdateAllAccounts(accs);
        }
    }
}
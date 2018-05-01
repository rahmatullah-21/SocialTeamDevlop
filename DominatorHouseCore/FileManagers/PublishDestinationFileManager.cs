using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.FileManagers
{
    public static class PublishDestinationFileManager
    {

        private static List<PublisherDestinationDetails> _allDestinationsCache = new List<PublisherDestinationDetails>();

        // Same as above, but Func must return true if file needs to be overwritten        
        public static void ApplyFunc(Func<PublisherDestinationDetails, bool> funcToApply)
        {
            bool updated = false;
            var destinations = BinFileHelper.GetPublisherDestinationDetails();

            foreach (var a in destinations)
                updated |= funcToApply(a);

            if (updated)
                BinFileHelper.UpdateAllPublishDestination(destinations);
        }

        // Saves all destinations. Have to work Only in Social library. Otherwise use UpdateDestinations() method to update PublisherDestinations.bin
        // NOTE: make sure lstPublisherDetails contains all destinations
        internal static void SaveAll(List<PublisherDestinationDetails> lstPublisherDetails)
        {
            // Warning: make sure lstPublisherDetails contains all publisher            
            BinFileHelper.UpdateAllPublishDestination(lstPublisherDetails);
            GlobusLogHelper.log.Debug($"{lstPublisherDetails.Count} Destination successfully saved");
        }

        // Update publisher entries and save to PublisherDestinations.bin        
        public static void UpdateDestinations(IList<PublisherDestinationDetails> libraryDestinations)
        {
            var all = BinFileHelper.GetPublisherDestinationDetails();

            // Update all entries that exists in libraryDestinations, and add that does not exists
            for (int i = 0; i < libraryDestinations.Count; i++)
            {
                var acc = libraryDestinations[i];
                var ix = all.FindIndex(a => acc.DetailsUrl == a.DetailsUrl);
                if (ix == -1)
                    all.Add(acc);
                else
                    all[ix] = acc;
            }
            BinFileHelper.UpdateAllPublishDestination(all);
        }
       

        public static void FillList<T>(ObservableCollection<T> destinationList) where T : class
        {
            destinationList.Clear();
            ApplyFunc(a =>
            {
                destinationList.Add((T)(object)a);
                return false;
            });
        }

        public static List<PublisherDestinationDetails> GetAll() => BinFileHelper.GetPublisherDestinationDetails();
   
        internal static List<PublisherDestinationDetails> GetAll(string accountId, DestinationCategory category)
            => GetAll().Where(a => a.AccountId == accountId && a.Category == category).ToList();
         
        public static bool Add(PublisherDestinationDetails account)
        {
            var lst = GetAll() ?? new List<PublisherDestinationDetails>();
            lst.Add(account);
            BinFileHelper.UpdateAllPublishDestination(lst);
            return true;
        }

        public static bool AddRange(List<PublisherDestinationDetails> destinationList)
        {
            var lst = GetAll() ?? new List<PublisherDestinationDetails>();          
            lst.AddRange(destinationList);
            BinFileHelper.UpdateAllPublishDestination(lst);
            return true;
        }

        public static void DeleteSelected(List<PublisherDestinationDetails> accs)
        {
            var all = GetAll().Where(a => accs.FirstOrDefault(p => p.AccountId == a.AccountId && p.DetailsUrl == a.DetailsUrl && p.Category == a.Category) == null).ToList();
            SaveAll(all);
        }

        public static void Delete(Predicate<PublisherDestinationDetails> match)
        {
            var accs = GetAll();
            accs.RemoveAll(match);
            BinFileHelper.UpdateAllAccounts(accs);
        }
    }
}